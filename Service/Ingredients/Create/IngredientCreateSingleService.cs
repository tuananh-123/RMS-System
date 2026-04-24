using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RMS.Contants;
using RMS.CustomDtoValidators.Ingredients;
using RMS.Dtos.Ingredients;
using RMS.Entities;
using RMS.IService.IIngredients.ICreate;

namespace RMS.Service.Ingredients.Create;

public class IngredientCreateSingleService(
    RMSDbContext dbContext,
    ILogger<IngredientCreateSingleService> logger,
    IMapper mappers,
    IngredientCreateDtoValidator validator,
    IngredientCreationBuilder builder
) : IIngredientCreateSingleService
{
    private readonly RMSDbContext _dbContext = dbContext;
    private readonly ILogger<IngredientCreateSingleService> _logger = logger;
    private readonly IMapper _mappers = mappers;
    private readonly IngredientCreateDtoValidator _validator = validator;
    private readonly IngredientCreationBuilder _builder = builder;

    public async Task<ServiceResult> ExecuteAsync(int userId, IngredientCreateDto request)
    {
        // step 1: validate request
        var notNullValidationResult = await _validator.ValidateAsync(request);
        if (!notNullValidationResult.IsValid)
        {
            _logger.LogWarning("Validation failed for ingredient creation request: {Errors}", notNullValidationResult.Errors);
            var errors = notNullValidationResult.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}").ToList();
            return new ServiceResult(false, StatusCodes.Status400BadRequest, "Validation failed: " + string.Join("; ", errors));
        }

        // step 2: title duplicate check
        var isDuplicate = await CheckTitleDuplicateAsync(request.Title);
        if (isDuplicate)
        {
            _logger.LogWarning("Ingredient creation failed due to duplicate title: {Title}", request.Title);
            return new ServiceResult(false, StatusCodes.Status409Conflict, "An ingredient with the same title already exists.");
        }

        // step 3: build entity
        var ingredient = _builder.BuildEntity(userId, request);

        // step 4: persist to database
        return await PersistIngredientAsync(ingredient);
    }

    private async Task<bool> CheckTitleDuplicateAsync(string title)
    {
        return await _dbContext.Ingredients.AsNoTracking().AnyAsync(i => i.Title.Trim().ToLower() == title.Trim().ToLower());
    }

    private async Task<ServiceResult> PersistIngredientAsync(Ingredient ingredient)
    {
        try
        {
            var transaction = await _dbContext.Database.BeginTransactionAsync();

            await _dbContext.Ingredients.AddAsync(ingredient);
            await _dbContext.SaveChangesAsync();

            await transaction.CommitAsync();
            return new ServiceResult(true, StatusCodes.Status201Created, "Ingredient created successfully.");
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
            _logger.LogError(ex, "Error occurred while saving the new ingredient to the database.");
            return new ServiceResult(false, StatusCodes.Status500InternalServerError, "An error occurred while creating the ingredient. Please try again later.");
        }
    }
}