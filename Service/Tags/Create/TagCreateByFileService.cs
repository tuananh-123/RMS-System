using System.Text.Json;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RMS.CustomDtoValidators.Tags;
using RMS.Dtos.Tags.Create;
using RMS.Entities;
using RMS.Helpers;

namespace RMS.Service.Tags.Create;

public class TagCreateByFileService(
    RMSDbContext context,
    ILogger<TagCreateByFileService> logger,
    TagCreateDtoValidator validator,
    IMapper mapper
) : BaseService(context, mapper)
{
    private readonly ILogger<TagCreateByFileService> _logger = logger;
    private readonly TagCreateDtoValidator _validator = validator;

    public async Task<(bool, int, string, object?)> ExecuteAsync(int userId, IFormFile file, CancellationToken ct)
    {
        try
        {
            // step 1: validate file type and size
            var (step1_success, step1_code, step1_message) = FileValidation(file);
            if (!step1_success) return new(step1_success, step1_code, step1_message, null);

            // step 2: read and convert to TagCreateDto
            var extension = file.FileName.GetExtension();
            var (step2_success, step2_code, step2_message, step2_data) = extension switch
            {
                ".json" => await ReadAndCreateObjectJSONFile(file),
                ".csv" => new(true, 200, "", null),
                ".xlsx" or ".xls" => new(true, 200, "", null),
                _ => new(true, 200, "", null)
            };

            // step 3: validate null and empty
            var (step3_success, step3_code, step3_message, step3_error) = await ValidateNullAndEmpty(step2_data!);
            if (!step3_success)
                return new(step3_success, step3_code, step3_message, step3_error);

            // step 4: build range entity
            var tags = BuildRangeEntity(userId, step2_data!);

            // step 5: persist to database
            return await PersistToDatabase(userId, tags, ct);
        }
        catch (JsonException jEx)
        {
            _logger.LogError(jEx, "Error with JSON");
            return new(false, StatusCodes.Status500InternalServerError, "Internal Server Error", null);
        }
        catch (DbUpdateException dbEx)
        {
            _logger.LogError(dbEx, "Database error occurred while creating ingredient.");
            if (dbEx.InnerException is Npgsql.PostgresException pgEx && pgEx.SqlState == "23505")
            {
                return new(false, StatusCodes.Status409Conflict, "An ingredient with the same title already exists.", null);
            }
            return new(false, StatusCodes.Status500InternalServerError, "Database operation failed. Please try again later.", null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error orcured while reading file");
            return new(false, StatusCodes.Status500InternalServerError, "Internal Server Error", null);
        }
    }

    private (bool, int, string) FileValidation(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            _logger.LogWarning("No file uploaded for ingredient creation.");
            return new(false, StatusCodes.Status400BadRequest, "No file uploaded.");
        }

        var allowedExtensions = new[] { ".csv", ".xlsx", ".xls", ".json" };
        var allowedContentTypes = new[]
        {
            "text/csv",
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "application/vnd.ms-excel",
            "application/json"
        };

        var extension = file.FileName.GetExtension();
        if (string.IsNullOrEmpty(extension) || !allowedExtensions.Contains(extension))
        {
            _logger.LogWarning("Unsupported file type {FileType} for ingredient creation.", extension);
            return new(false, StatusCodes.Status415UnsupportedMediaType, "Unsupported file type.");
        }

        if (!allowedContentTypes.Contains(file.ContentType))
        {
            _logger.LogWarning("Unsupported content type {ContentType} for ingredient creation.", file.ContentType);
            return new(false, StatusCodes.Status415UnsupportedMediaType, "Unsupported content type.");
        }

        return new(true, StatusCodes.Status200OK, "File hợp lệ");
    }

    private static async Task<(bool, int, string, List<TagCreateDto>?)> ReadAndCreateObjectJSONFile(IFormFile file)
    {
        // read
        var jsonContent = await file.ReadJsonFileContent();
        if (string.IsNullOrEmpty(jsonContent))
        {
            return new(false, StatusCodes.Status204NoContent, "File không có dữ liệu nhập liệu.", null);
        }

        // create
        var tagCreateDtos = jsonContent.DeserializeToListObject<TagCreateDto>();

        return (true, StatusCodes.Status200OK, "Khởi tạo dữ liệu thành công.", tagCreateDtos);
    }

    private async Task<(bool, int, string, object?)> ValidateNullAndEmpty(List<TagCreateDto> ingredientCreateDtos)
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
            return new(false, StatusCodes.Status400BadRequest, "Dữ liệu không hợp lệ.", JsonSerializer.Serialize(validateErrors));

        return new(true, StatusCodes.Status200OK, "Dữ liệu hợp lệ.", null);
    }

    private List<Tag> BuildRangeEntity(int userId, List<TagCreateDto> ingredientCreateDtos)
    {
        var tags = new List<Tag>();
        foreach (var item in ingredientCreateDtos)
        {
            var entity = BuildEntity(userId, item);
            tags.Add(entity);
        }
        return tags;
    }

    private Tag BuildEntity(int userId, TagCreateDto request)
    {
        var tag = _mapper.Map<Tag>(request);
        tag.Slug = tag.Title.ToSlug();
        tag.CreatedBy = userId.ToString();
        return tag;
    }

    private async Task<(bool, int, string, object?)> PersistToDatabase(int userId, List<Tag> tags, CancellationToken ct)
    {
        try
        {
            await _dbContext.AddRangeAsync(tags);
            await _dbContext.SaveChangesAsync(ct);
            return new(true, StatusCodes.Status201Created, "Tag created successfully.", null);
        }
        catch (DbUpdateException dbEx)
        {
            _logger.LogError(dbEx, "Database error occurred while creating tag");
            if (dbEx.InnerException is Npgsql.PostgresException pgEx)
            {
                if (pgEx.SqlState == "23505")
                {
                    return new(false, StatusCodes.Status409Conflict, $"A Tag with the same value already exists.", null);
                }
            }

            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occred");
            throw;
        }
    }
}