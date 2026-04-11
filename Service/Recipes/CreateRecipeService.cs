using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RMS.Contants;
using RMS.Dtos;
using RMS.Entities;

namespace RMS.Service.Recipes;

public class CreateRecipeService(
    RMSDbContext context,
    IMapper mapper,
    CreateRecipeValidator validator,
    RecipeBuilder builder
) 
{
    private readonly RMSDbContext _context = context;
    private readonly IMapper _mapper = mapper;
    private readonly CreateRecipeValidator _validator = validator;
    private readonly RecipeBuilder _builder = builder;

    private async Task<bool> IsTitleDuplicateAsync(string title)
    {
        return await _context.Recipes.AnyAsync(r => r.Title.Trim().ToLower() == title.Trim().ToLower());
    }

    private async Task<HashSet<int>> GetExistingTagIdAsync(IEnumerable<int> tagIds)
    {
        return await _context.Tags
            .Where(t => tagIds.Contains(t.ID))
            .Select(t => t.ID)
            .ToHashSetAsync();
    }

    private async Task<HashSet<int>> GetExistingIngredientIdAsync(IEnumerable<int> ingredientIds)
    {
        return await _context.Ingredients
            .Where(i => ingredientIds.Contains(i.ID))
            .Select(i => i.ID)
            .ToHashSetAsync();
    }

    private async Task<ServiceResult?> ValidateRelaatedDataAsync(RecipeCreateDto request)
    {
          if (request.Tags == null || request.Tags.Length == 0)
        {
            return await Task.FromResult(new ServiceResult(false, ErrorCode.BadRequest.GetHashCode(), "At least one tag is required."));
        }
        var tags = await GetExistingTagIdAsync(request.Tags);

        if (tags.Count != request.Tags.Length)
        {
            return await Task.FromResult(new ServiceResult(false, ErrorCode.BadRequest.GetHashCode(), "One or more tags do not exist."));
        }

        if (request.RecipeIngredients == null || request.RecipeIngredients.Count == 0)
        {
            return await Task.FromResult(new ServiceResult(false, ErrorCode.BadRequest.GetHashCode(), "At least one ingredient is required."));
        }

        var ingredientIds = await GetExistingIngredientIdAsync(request.RecipeIngredients.Select(ri => ri.ID));
        
        if (ingredientIds.Count != request.RecipeIngredients.Count)
        {
            return await Task.FromResult(new ServiceResult(false, ErrorCode.BadRequest.GetHashCode(), "One or more ingredients do not exist."));
        }

        return await Task.FromResult<ServiceResult?>(null);
    }

    private ServiceResult? ValidateBusinessRulesAsync(RecipeCreateDto request)
    {
        var validationErrors = _validator.RecipeBusinessRules(request);
        if (validationErrors.Length > 0)
        {
            var messageErrorr = string.Join("\n", validationErrors.Select(e => $"{e.Field}: {e.Message}"));
            return new ServiceResult(false, ErrorCode.BadRequest.GetHashCode(), messageErrorr);
        }
        return null;
    }

    private Recipe BuildRecipe(int userId, RecipeCreateDto request)
    {
        var recipe = _mapper.Map<Recipe>(request);
        recipe.CreatedBy = userId.ToString();
        return _builder.Build(recipe, request);
    }

    public async Task<ServiceResult> ExecuteAsync(int userId, RecipeCreateDto request)
    {
        request.Title = request.Title.Trim();
        request.Description = request.Description.Trim();
        request.SearchKeyword.Keywords = [..request.SearchKeyword.Keywords.Select(k => k.Trim())];
        request.SearchKeyword.Hashtags = [..request.SearchKeyword.Hashtags.Select(h => h.Trim())];

        var IsTitleDuplicateResult = await IsTitleDuplicateAsync(request.Title);
        if (IsTitleDuplicateResult)
        {
            return new ServiceResult(false, ErrorCode.BadRequest.GetHashCode(), "Title already exists.");
        }

        var validateRelaatedDataResult = await ValidateRelaatedDataAsync(request);
        if (validateRelaatedDataResult != null)
        {
            return validateRelaatedDataResult;
        }

        request.Tags = [..request.Tags!.Distinct()];
        request.RecipeIngredients = [.. request.RecipeIngredients.Distinct()];

        var validateBusinessRulesResult = ValidateBusinessRulesAsync(request);
        if (validateBusinessRulesResult != null)
        {
            return validateBusinessRulesResult;
        }

        var recipe = BuildRecipe(userId, request);
        await _context.Recipes.AddAsync(recipe);
        await _context.SaveChangesAsync();
        return new ServiceResult(true, SuccessCode.Created.GetHashCode(), "Recipe created successfully.", recipe.ID);
    }

}