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
    CreateRecipeValidator validator,
    RecipeBuilder builder,
    CreateRecipeDtoValidator createRecipeDtoValidator
) : ICreateRecipeService
{
    private readonly RMSDbContext _context = context;
    private readonly IMapper _mapper = mapper;
    private readonly CreateRecipeValidator _validator = validator;
    private readonly RecipeBuilder _builder = builder;
    private readonly CreateRecipeDtoValidator _createRecipeDtoValidator = createRecipeDtoValidator;

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
        var tags = await GetExistingTagIdAsync(request.Tags);
        if (tags.Count != request.Tags.Length)
        {
            return await Task.FromResult(new ServiceResult(false, ErrorCode.BadRequest.GetHashCode(), "One or more tags do not exist."));
        }

        var ingredientIds = await GetExistingIngredientIdAsync(request.RecipeIngredients.Select(ri => ri.ID));
        if (ingredientIds.Count != request.RecipeIngredients.Count)
        {
            return await Task.FromResult(new ServiceResult(false, ErrorCode.BadRequest.GetHashCode(), "One or more ingredients do not exist."));
        }

        return await Task.FromResult<ServiceResult?>(null);
    }

    private ServiceResult? ValidateBusinessRule(RecipeCreateDto request)
    {
        var validationErrors = _createRecipeDtoValidator.Validate(request);
        if (validationErrors.Errors.Count > 0)
        {
            var messageErrorr = string.Join("\n", validationErrors.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}"));
            return new ServiceResult(false, ErrorCode.BadRequest.GetHashCode(), messageErrorr);
        }
        return null;
    }

    private ServiceResult? ValidateBusinessRules(Recipe recipe)
    {
        var validationErrors = _validator.RecipeBusinessRules(recipe);
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


    // https://www.coursera.org/courses?query=system%20design&irclickid=yzvWq%3AznlxyZRU6yAnyML2POUku30fyRfwGYxw0&irgwc=1&afsrc=1&utm_medium=partners&utm_source=impact&utm_campaign=6700588&utm_content=b2c&utm_campaignid=Coc%20Coc%20Search&utm_term=14726_SC_1164545_1631165870

    public async Task<ServiceResult> ExecuteAsync(int userId, RecipeCreateDto request)
    {
        var cleanRequestContextResultError = ValidateBusinessRule(request);
        if (cleanRequestContextResultError != null)
        {
            return cleanRequestContextResultError;
        }

        var IsTitleDuplicateResult = await IsTitleDuplicateAsync(request.Title);
        if (IsTitleDuplicateResult)
        {
            return new ServiceResult(false, ErrorCode.Conflict.GetHashCode(), "Title already exists.");
        }

        var validateRelaatedDataResult = await ValidateRelaatedDataAsync(request);
        if (validateRelaatedDataResult != null)
        {
            return validateRelaatedDataResult;
        }

        request.Tags = [..request.Tags!.Distinct()];
        request.RecipeIngredients = [.. request.RecipeIngredients.Distinct()];

        var recipe = BuildRecipe(userId, request);
        var validateBusinessRulesResult = ValidateBusinessRules(recipe);
        if (validateBusinessRulesResult != null)
        {
            return validateBusinessRulesResult;
        }

        await _context.Recipes.AddAsync(recipe);
        await _context.SaveChangesAsync();
        return new ServiceResult(true, SuccessCode.Created.GetHashCode(), "Recipe created successfully.", recipe.ID);
    }

}