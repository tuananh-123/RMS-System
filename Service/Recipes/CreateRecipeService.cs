using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RMS.Contants;
using RMS.CustomDtoValidators;
using RMS.Dtos;
using RMS.Entities;
using RMS.IService.IRecipes;

namespace RMS.Service.Recipes;

public class CreateRecipeService(
    RMSDbContext context,
    IMapper mapper,
    ICreateRecipeValidator validator,
    IRecipeBuilder builder,
    CreateRecipeDtoValidator createRecipeDtoValidator,
    ILogger<CreateRecipeService> logger
) : ICreateRecipeService
{
    private readonly RMSDbContext _context = context;
    private readonly IMapper _mapper = mapper;
    private readonly ICreateRecipeValidator _validator = validator;
    private readonly IRecipeBuilder _builder = builder;
    private readonly ILogger<CreateRecipeService> _logger = logger;
    private readonly CreateRecipeDtoValidator _createRecipeDtoValidator = createRecipeDtoValidator;

    private async Task<bool> CheckTitleDuplicateAsync(string title)
    {
        return await _context.Recipes.AnyAsync(r => r.Title.Equals(title, StringComparison.OrdinalIgnoreCase));
    }

    private async Task<HashSet<int>> GetExistingTagIdAsync(IEnumerable<int> tagIds)
    {
        return await _context.Tags.AsNoTracking()
            .Where(t => tagIds.Contains(t.ID))
            .Select(t => t.ID)
            .ToHashSetAsync();
    }

    private async Task<HashSet<int>> GetExistingIngredientIdAsync(IEnumerable<int> ingredientIds)
    {
        return await _context.Ingredients.AsNoTracking()
            .Where(i => ingredientIds.Contains(i.ID))
            .Select(i => i.ID)
            .ToHashSetAsync();
    }

    private void NormalizeRequest(RecipeCreateDto request)
    {
        request.Tags = [..request.Tags!.Distinct()];
        request.RecipeIngredients = [.. request.RecipeIngredients.Distinct()];
    }

    private async Task<ServiceResult?> ValidateRelatedDataAsync(RecipeCreateDto request)
    {
        var tags = await GetExistingTagIdAsync(request.Tags);
        if (tags.Count != request.Tags.Length)
        {
            return new ServiceResult(false, StatusCodes.Status400BadRequest, "One or more tags do not exist.");
        }

        var ingredientIds = await GetExistingIngredientIdAsync(request.RecipeIngredients.Select(ri => ri.ID));
        if (ingredientIds.Count != request.RecipeIngredients.Count)
        {
            return new ServiceResult(false, StatusCodes.Status400BadRequest, "One or more ingredients do not exist.");
        }

        return null;
    }

    private ServiceResult? ValidateInputRequest(RecipeCreateDto request)
    {
        var validationErrors = _createRecipeDtoValidator.Validate(request);
        if (validationErrors.Errors.Count > 0)
        {
            var messageErrorr = string.Join("; ", validationErrors.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}"));
            return new ServiceResult(false, StatusCodes.Status400BadRequest, messageErrorr);
        }
        return null;
    }

    private ServiceResult? ValidateBusinessRules(Recipe recipe)
    {
        var validationErrors = _validator.ValidateBusinessRules(recipe);
        if (validationErrors.Length > 0)
        {
            var messageErrorr = string.Join("; ", validationErrors.Select(e => $"{e.Field}: {e.Message}"));
            return new ServiceResult(false, StatusCodes.Status400BadRequest, messageErrorr);
        }
        return null;
    }

    private Recipe BuildRecipe(int userId, RecipeCreateDto request)
    {
        var recipe = _mapper.Map<Recipe>(request);
        recipe.CreatedBy = userId.ToString();
        return _builder.Build(recipe, request);
    }

    private async Task<ServiceResult> PersistRecipeAsync(Recipe recipe)
    {
        try
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            await _context.Recipes.AddAsync(recipe);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
            return new ServiceResult(true, StatusCodes.Status201Created, "Recipe created successfully.", recipe.ID);
        }
        catch (DbUpdateException dbEx)
        {
            _logger.LogError(dbEx, "Database error occurred while creating recipe.");
            return MapDbException(dbEx);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while creating recipe.");
            return new ServiceResult(false, StatusCodes.Status500InternalServerError, "An unexpected error occurred. Please try again later.");
        }
    }

    private static ServiceResult MapDbException(DbUpdateException dbEx)
    {
        if (dbEx.InnerException is Npgsql.PostgresException pgEx)
        {
            if (pgEx.SqlState == "23505") // Unique violation
            {
                return new ServiceResult(false, StatusCodes.Status409Conflict, "A recipe with the same value already exists.");
            }
        }

        return new ServiceResult(false, StatusCodes.Status500InternalServerError, "Database operation failed.");
    }

    // https://www.coursera.org/courses?query=system%20design&irclickid=yzvWq%3AznlxyZRU6yAnyML2POUku30fyRfwGYxw0&irgwc=1&afsrc=1&utm_medium=partners&utm_source=impact&utm_campaign=6700588&utm_content=b2c&utm_campaignid=Coc%20Coc%20Search&utm_term=14726_SC_1164545_1631165870

    public async Task<ServiceResult> ExecuteAsync(int userId, RecipeCreateDto request)
    {
        // step 1: validate input request
        var validateInputRequestError = ValidateInputRequest(request);
        if (validateInputRequestError != null)
        {
            _logger.LogWarning("Validation failed for recipe creation request by user {userId}: {error}", userId, validateInputRequestError.Message);
            return validateInputRequestError;
        }

        // step 2: check title duplicate
        var checkTitleDuplicateResult = await CheckTitleDuplicateAsync(request.Title);
        if (checkTitleDuplicateResult)
        {
            _logger.LogWarning("Duplicate title detected for recipe creation request by user {userId}: {title}", userId, request.Title);
            return new ServiceResult(false, StatusCodes.Status409Conflict, "Title already exists.");
        }

        // step 3: validate related data (tags, ingredients)
        var validateRelatedDataResult = await ValidateRelatedDataAsync(request);
        if (validateRelatedDataResult != null)
        {
            _logger.LogWarning("Related data validation failed for recipe creation request by user {userId}: {error}", userId, validateRelatedDataResult.Message);
            return validateRelatedDataResult;
        }

        // step 4: normalize request data (remove duplicates in tags and ingredients)
        NormalizeRequest(request);

        // step 5: build recipe entity
        var recipe = BuildRecipe(userId, request);

        // step 6: validate business rules
        var validateBusinessRulesResult = ValidateBusinessRules(recipe);
        if (validateBusinessRulesResult != null)
        {
            _logger.LogWarning("Business rules validation failed for recipe creation request by user {userId}: {error}", userId, validateBusinessRulesResult.Message);
            return validateBusinessRulesResult;
        }

        // step 7: persist recipe to database
        return await PersistRecipeAsync(recipe);
    }

}