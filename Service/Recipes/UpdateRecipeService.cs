using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RMS.Contants;
using RMS.CustomDtoValidators.Recipes;
using RMS.Dtos;
using RMS.Entities;
using RMS.IService.IRecipes;

namespace RMS.Service.Recipes;

public class UpdateRecipeService(
    RMSDbContext context,
    IMapper mapper,
    ILogger<UpdateRecipeService> logger,
    UpdateRecipeDtoValidator validationRules,
    RecipeBuilder builder
) : IUpdateRecipeService
{
    private readonly RMSDbContext _context = context;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<UpdateRecipeService> _logger = logger;
    private readonly RecipeBuilder _builder = builder;
    private readonly UpdateRecipeDtoValidator _validationRules = validationRules;

    public async Task<ServiceResult> ExecuteSync(int userId, int recipeId, RecipeUpdateDto request)
    {
        // Use AsSplitQuery to avoid Cartesian explosion when including multiple collections
        // step 1: check recipe exists
        var existsRecipe = await _context.Recipes
            .AsSplitQuery()
            .Include(r => r.RecipeHistories)
            .Include(r => r.RecipeIngredients)
            .Include(r => r.TagForRecipes)
            .FirstOrDefaultAsync(r => r.ID == recipeId);

        if (existsRecipe == null)
        {
            _logger.LogWarning("Recipe with ID {recipeId} not found for update request by user {userId}", recipeId, userId);
            return new ServiceResult(false, StatusCodes.Status404NotFound, "Recipe not found");
        }

        // step 2: validate not null request
        var _isValidRequest = ValidateNotNullRequestAsync(request);
        if (_isValidRequest != null)
        {
            _logger.LogWarning("Validation failed for recipe update request by user {userId}: {error}", userId, _isValidRequest.Message);
            return _isValidRequest;
        }

        // step 3: validate title duplicate
        var isDuplicateTitle = await CheckTitleUpdateDuplicateAsync(request.Title, recipeId);
        if (isDuplicateTitle)
        {
            _logger.LogWarning("Duplicate title found for recipe update request by user {userId}: {title}", userId, request.Title);
            return new ServiceResult(false, StatusCodes.Status400BadRequest, "Duplicate title found");
        }

        // step 4: normalize request
        NormalizeRequest(request);

        // step 5: validate related entities (tags, ingredients)
        var relatedDataValidationResult = await ValidateRelatedDataAsync(request);
        if (relatedDataValidationResult != null)
        {
            _logger.LogWarning("Related data validation failed for recipe update request by user {userId}: {error}", userId, relatedDataValidationResult.Message);
            return relatedDataValidationResult;
        }

        // step 6: build recipe entity
        var buildedRecipeHistory = _builder.BuildRecipeHistory(existsRecipe);
        var updatedRecipe = BuildRecipe(userId, request);

        // map update data to tracking entity to update
        existsRecipe = _mapper.Map(updatedRecipe, existsRecipe);

        // update related entities using bulk operations for efficiency
        await UpdateRecipeIngredientsAsync(existsRecipe, updatedRecipe.RecipeIngredients);
        await UpdateRecipeTagsAsync(existsRecipe, updatedRecipe.TagForRecipes);

        // add history update version to recipe
        existsRecipe.RecipeHistories.Add(buildedRecipeHistory);


        // step 7: persist to database
        return await PersistRecipeAsync(existsRecipe);
    }



    private ServiceResult? ValidateNotNullRequestAsync(RecipeUpdateDto request)
    {
        var validationResult = _validationRules.Validate(request);
        if (!validationResult.IsValid)
        {
            var message = string.Join("; ", validationResult.Errors.Select(er => $"{er.PropertyName} - {er.ErrorMessage}"));
            return new ServiceResult(false, StatusCodes.Status400BadRequest, message);
        }

        return null;
    }

    private async Task<bool> CheckTitleUpdateDuplicateAsync(string title, int recipeId) => await _context.Recipes.Where(r => r.ID != recipeId).AnyAsync(r => r.Title.Trim().ToLower() == title.Trim().ToLower());

    private static void TrimStringProperties(RecipeUpdateDto request)
    {
        request.Title = request.Title.Trim();
        request.Description = request.Description.Trim();
    }

    private static void TrimKeywordProperties(RecipeUpdateDto request)
    {
        request.SearchKeyword!.Hashtags = [.. request.SearchKeyword.Hashtags.Select(h => h.Trim())];
        request.SearchKeyword!.Keywords = [.. request.SearchKeyword.Keywords.Select(kw => kw.Trim())];
    }

    private static T[] DistinctArray<T>(T[] array) where T : notnull => [.. array.Distinct()];

    private static void NormalizeRequest(RecipeUpdateDto request)
    {
        TrimStringProperties(request);
        TrimKeywordProperties(request);

        request.Tags = DistinctArray(request.Tags);

        request.RecipeIngredients = [.. request.RecipeIngredients.GroupBy(r => r.ID).Select(x => new RecipeIngredientCreateDto
        {
            ID = x.Key,
            Quantity = x.Sum(ri => ri.Quantity),
            Unit = x.First().Unit,
            CaloPer100Gram = x.First().CaloPer100Gram
        })];
    }

    private async Task<ServiceResult?> ValidateRelatedDataAsync(RecipeUpdateDto request)
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

    private async Task<HashSet<int>> GetExistingTagIdAsync(IEnumerable<int> tagIds) => await _context.Tags.AsNoTracking()
            .Where(t => tagIds.Contains(t.ID))
            .Select(t => t.ID)
            .ToHashSetAsync();

    private async Task<HashSet<int>> GetExistingIngredientIdAsync(IEnumerable<int> ingredientIds) => await _context.Ingredients.AsNoTracking()
            .Where(i => ingredientIds.Contains(i.ID))
            .Select(i => i.ID)
            .ToHashSetAsync();

    private Recipe BuildRecipe(int userId, RecipeUpdateDto request)
    {
        var recipe = MapRecipe(request);

        recipe.CreatedBy = GetCreatedBy(userId);

        return _builder.BuildUpdate(recipe, request);
    }

    private static string GetCreatedBy(int userId) => userId.ToString();

    private Recipe MapRecipe(RecipeUpdateDto request) => _mapper.Map<Recipe>(request);

    private async Task<ServiceResult> PersistRecipeAsync(Recipe recipe)
    {
        try
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            return new ServiceResult(true, StatusCodes.Status200OK, "Recipe updated successfully");
            
        }catch (DbUpdateConcurrencyException concEx)
        {
            _logger.LogWarning(concEx, "Concurrency conflict while updating recipe with ID {recipeId}. Recipe was modified by another user.", recipe.ID);
            return new ServiceResult(false, StatusCodes.Status409Conflict, "Recipe was modified by another user. Please refresh and try again.");
        }catch (DbUpdateException dbEx)
        {
            _logger.LogError(dbEx, "Database update error while updating recipe with ID {recipeId}", recipe.ID);
            return MapDbException(dbEx);
        }catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while updating recipe with ID {recipeId}", recipe.ID);
            return new ServiceResult(false, StatusCodes.Status500InternalServerError, $"An error occurred while updating the recipe: {ex.Message}");
        }
    }

    private async Task UpdateRecipeIngredientsAsync(Recipe existsRecipe, ICollection<RecipeIngredient> updatedIngredients)
    {
        // Find ingredients to remove (exist in DB but not in update request)
        var ingredientIdsToRemove = existsRecipe.RecipeIngredients
            .Where(ri => !updatedIngredients.Any(ui => ui.IngredientID == ri.IngredientID))
            .ToList();

        // Bulk delete using ExecuteDeleteAsync for efficiency
        if (ingredientIdsToRemove.Count > 0)
        {
            var ingredientIdsToDelete = ingredientIdsToRemove.Select(ri => ri.IngredientID).ToList();
            await _context.RecipeIngredients
                .Where(ri => ri.RecipeID == existsRecipe.ID && ingredientIdsToDelete.Contains(ri.IngredientID))
                .ExecuteDeleteAsync();
        }

        // Update existing or add new ingredients
        foreach (var updated in updatedIngredients)
        {
            var existing = existsRecipe.RecipeIngredients
                .FirstOrDefault(ri => ri.IngredientID == updated.IngredientID);

            if (existing != null)
            {
                // Update existing: only quantity and unit can change
                existing.Quantity = updated.Quantity;
                existing.Unit = updated.Unit;
            }
            else
            {
                // Add new ingredient to recipe
                existsRecipe.RecipeIngredients.Add(new RecipeIngredient
                {
                    IngredientID = updated.IngredientID,
                    Quantity = updated.Quantity,
                    Unit = updated.Unit,
                    RecipeID = existsRecipe.ID
                });
            }
        }
    }

    private async Task UpdateRecipeTagsAsync(Recipe existsRecipe, ICollection<TagForRecipe> updatedTags)
    {
        // Find tags to remove (exist in DB but not in update request)
        var tagIdsToRemove = existsRecipe.TagForRecipes
            .Where(tr => !updatedTags.Any(ut => ut.TagID == tr.TagID))
            .ToList();

        // Bulk delete using ExecuteDeleteAsync for efficiency
        if (tagIdsToRemove.Count > 0)
        {
            var tagIdsToDelete = tagIdsToRemove.Select(tr => tr.TagID).ToList();
            await _context.TagForRecipes
                .Where(tr => tr.RecipeID == existsRecipe.ID && tagIdsToDelete.Contains(tr.TagID))
                .ExecuteDeleteAsync();
        }

        // Add new tags
        foreach (var updated in updatedTags)
        {
            var exists = existsRecipe.TagForRecipes.Any(tr => tr.TagID == updated.TagID);
            if (!exists)
            {
                existsRecipe.TagForRecipes.Add(new TagForRecipe
                {
                    TagID = updated.TagID,
                    RecipeID = existsRecipe.ID
                });
            }
        }
    }

    private static ServiceResult MapDbException(DbUpdateException dbEx)
    {
        if (dbEx.InnerException is Npgsql.PostgresException pgEx)
        {
            if (pgEx.SqlState == "23505") // Unique violation
            {
                return new ServiceResult(false, StatusCodes.Status409Conflict, "A recipe with the same title already exists.");
            }
        }

        return new ServiceResult(false, StatusCodes.Status500InternalServerError, "An error occurred while updating the recipe.");
    }
}