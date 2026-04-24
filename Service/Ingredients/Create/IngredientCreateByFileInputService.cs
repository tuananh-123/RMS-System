using System.Text.Json;
using System.Text.Json.Schema;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RMS.Contants;
using RMS.CustomDtoValidators.Ingredients;
using RMS.Dtos.Ingredients;
using RMS.Entities;
using RMS.IService.IIngredients.ICreate;

namespace RMS.Service.Ingredients.Create;

public class IngredientCreateByFileInputService(
    RMSDbContext dbContext,
    ILogger<IngredientCreateByFileInputService> logger,
    IMapper mappers,
    IngredientCreationBuilder builder,
    IngredientCreateDtoValidator validator
) : IIngredientCreateByFileInputService
{
    private readonly RMSDbContext _dbContext = dbContext;
    private readonly ILogger<IngredientCreateByFileInputService> _logger = logger;
    private readonly IMapper _mappers = mappers;
    private readonly IngredientCreationBuilder _builder = builder;
    private readonly IngredientCreateDtoValidator _validator = validator;

    public async Task<ServiceResult> ExecuteAsync(int userId, IFormFile file)
    {
        try
        {
            // step 1: validate file type and size
            var validationFileResult = ValidateFile(file);
            if (validationFileResult != null) return validationFileResult;

            // step 2: read and convert to IngredientCreatDto
            var extension = GetExtension(file.FileName);
            var ingredientCreateDtos = extension switch
            {
                ".json" => await ReadAndCreateObjectJSONFile(file),
                ".csv" => null,
                ".xlsx" or ".xls" => null,
                _ => null
            };

            // step 3: validate null and empty
            await ValidateNullAndEmpty(ingredientCreateDtos!);

            // step 4: build range entity
            var ingredients = BuildRangeEntity(userId, ingredientCreateDtos!);

            // step 5: persist to database
            return await PersistToDatabase(ingredients);
        }
        catch (JsonException jEx)
        {
            _logger.LogError(jEx, "Error with JSON");
            return new ServiceResult(false, StatusCodes.Status500InternalServerError, "Internal Server Error");
        }
        catch (DbUpdateException dbEx)
        {
            _logger.LogError(dbEx, "Database error occurred while creating ingredient.");
            if (dbEx.InnerException is Npgsql.PostgresException pgEx && pgEx.SqlState == "23505")
            {
                return new ServiceResult(false, StatusCodes.Status409Conflict, "An ingredient with the same title already exists.");
            }
            return new ServiceResult(false, StatusCodes.Status500InternalServerError, "Database operation failed. Please try again later.");
        } 
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error orcured while reading file");
            return new ServiceResult(false, StatusCodes.Status500InternalServerError, "Internal Server Error");
        }
    }

    private ServiceResult? ValidateFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            _logger.LogWarning("No file uploaded for ingredient creation.");
            return new ServiceResult(false, StatusCodes.Status400BadRequest, "No file uploaded.");
        }

        var allowedExtensions = new[] { ".csv", ".xlsx", ".xls", ".json" };
        var allowedContentTypes = new[]
        {
            "text/csv",
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "application/vnd.ms-excel",
            "application/json"
        };

        var extension = GetExtension(file.FileName);
        if (string.IsNullOrEmpty(extension) || !allowedExtensions.Contains(extension))
        {
            _logger.LogWarning("Unsupported file type {FileType} for ingredient creation.", extension);
            return new ServiceResult(false, StatusCodes.Status415UnsupportedMediaType, "Unsupported file type.");
        }

        if (!allowedContentTypes.Contains(file.ContentType))
        {
            _logger.LogWarning("Unsupported content type {ContentType} for ingredient creation.", file.ContentType);
            return new ServiceResult(false, StatusCodes.Status415UnsupportedMediaType, "Unsupported content type.");
        }

        return null;
    }

    private static string GetExtension(string fileName) => Path.GetExtension(fileName).ToLowerInvariant();

    private async Task<List<IngredientCreateDto>> ReadAndCreateObjectJSONFile(IFormFile file)
    {
        // read
        var jsonContent = await ReadFileJSONContent(file);
        if (string.IsNullOrEmpty(jsonContent))
        {
            throw new Exception("File is empty");
        }

        // create
        var IngredientCreateDtos = new List<IngredientCreateDto>();
        var ObjectDeserialize = DeserializeToListObject<JSONDeserializeCreateIngredientDto>(jsonContent);
        foreach (var item in ObjectDeserialize)
        {
            var IngredientCreateDto = _mappers.Map<IngredientCreateDto>(item);
            if (item.SearchKeyword != null && item.SearchKeyword.ToString() != "{}")
            {
                string jsonSerialize = JsonSerializer.Serialize(item.SearchKeyword);
                if (!string.IsNullOrEmpty(jsonSerialize))
                    IngredientCreateDto.SearchKeyword = DeserializeToObject<SearchKeyword>(jsonSerialize);
            }

            IngredientCreateDtos.Add(IngredientCreateDto);
        }

        return IngredientCreateDtos;
    }

    private static async Task<string> ReadFileJSONContent(IFormFile file)
    {
        using Stream stream = file.OpenReadStream();
        using var reader = new StreamReader(stream);
        string jsonContent = await reader.ReadToEndAsync();
        return jsonContent;
    }

    private static List<T> DeserializeToListObject<T>(string jsonContent) where T : class
    {
        return JsonSerializer.Deserialize<List<T>>(jsonContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;
    }

    private static T DeserializeToObject<T>(string jsonContent) where T : class => JsonSerializer.Deserialize<T>(jsonContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;

    private async Task ValidateNullAndEmpty(List<IngredientCreateDto> ingredientCreateDtos)
    {
        string validateErrors = string.Empty;
        foreach (var item in ingredientCreateDtos)
        {
            var validateResult = await _validator.ValidateAsync(item);
            if (!validateResult.IsValid)
            {
                var error = validateResult.Errors.Select(e => $"{e.PropertyName} - {e.ErrorMessage}");
                validateErrors += string.Join("/n ", error);
            }

        }

        if (validateErrors.Length > 0)
            throw new Exception(validateErrors);
    }

    private List<Ingredient> BuildRangeEntity(int userId, List<IngredientCreateDto> ingredientCreateDtos)
    {
        var ingredients = new List<Ingredient>();
        foreach (var item in ingredientCreateDtos)
        {
            var entity = _builder.BuildEntity(userId, item);
            ingredients.Add(entity);
        }
        return ingredients;
    }

    private async Task<ServiceResult> PersistToDatabase(List<Ingredient> ingredients)
    {
        using var transaction = await _dbContext.Database.BeginTransactionAsync();

        await _dbContext.Ingredients.AddRangeAsync(ingredients);
        await _dbContext.SaveChangesAsync();

        await transaction.CommitAsync();

        return new ServiceResult(true, StatusCodes.Status200OK, "Created bunch of ingredient successfully");
    }
}