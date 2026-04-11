using AutoMapper;
using RMS.Dtos;
using RMS.Contants;
using RMS.Entities;
using RMS.IService;


namespace RMS.Service;

public class RecipeService(
    RMSDbContext context,
    IMapper mapper,
    IRecipeHistoryService historyService,
    IIngredientService ingredientService,
    ITagService tagService) : BaseService<Recipe>(context), IRecipeService
{
    private readonly IMapper _mapper = mapper;
    private readonly IRecipeHistoryService _historyService = historyService;
    private readonly ITagService _tagService = tagService;
    private readonly IIngredientService _ingredientService = ingredientService;

    public async Task<ServiceResult> AddRecipeAsync(int userId, RecipeCreateDto recipe) 
    {
        var transaction = await BeginTransactionAsync();
        try
        {
            // Kiểm tra tên công thức đã tồn tại chưa
            if (await ExistsAsync(r => r.Title.ToLower().Trim() == recipe.Title.ToLower().Trim()))
            {   
                var data = new
                {
                    Field = "Title",
                    // Message = "Recipe title already exists."
                };
                return new ServiceResult(false, ErrorCode.Conflict.GetHashCode(), data: data); 
            }           
                 
            var newRecipe = _mapper.Map<Recipe>(recipe);
            newRecipe.CreatedBy = userId.ToString();

            // Recipe tags
            if (recipe.Tags != null)
            {
                recipe.Tags = [.. recipe.Tags.Distinct()]; // Loại bỏ trùng lặp
                recipe.Tags = [.. recipe.Tags.Where(id => _tagService.ExistsAsync(t => t.ID == id).Result)]; // Loại bỏ tag không tồn tại
                if (recipe.Tags.Length == 0)
                {
                    var data = new []
                    {
                        new ValidationResult("Tags", "These tags do not exist."),
                    };
                    return new ServiceResult(false, ErrorCode.BadRequest.GetHashCode(), data: data);    
                }
                newRecipe.TagForRecipes = [.. recipe.Tags.Select(id => new TagForRecipe
                {
                    TagID = id
                })];
            }

            // Recipe ingredients
            if (recipe.RecipeIngredients != null)
            {
                recipe.RecipeIngredients = [.. recipe.RecipeIngredients.Where(ri => _ingredientService.ExistsAsync(i => i.ID == int.Parse(ri.ID)).Result)]; // Loại bỏ ingredient không tồn tại
                if (recipe.RecipeIngredients.Count == 0)
                {
                    var data = new []
                    {
                        new ValidationResult("RecipeIngredients", "These ingredients do not exist."),
                    };
                    return new ServiceResult(false, ErrorCode.BadRequest.GetHashCode(), data: data);    
                }
                newRecipe.RecipeIngredients = [.. recipe.RecipeIngredients.Select(ri => new RecipeIngredient
                {
                    IngredientID = int.Parse(ri.ID),
                    Quantity = ri.Quantity,
                    Unit = ri.Unit
                })];
                newRecipe.TotalCalories = recipe.RecipeIngredients.Sum(ri => ri.CaloPer100Gram);
            }

            if (recipe.RecipeIngredients is null || recipe.Tags is null)
            {
                var data = new []
                {
                    new ValidationResult("RecipeIngredients", "A recipe should have at least one ingredient."),
                    new ValidationResult("Tags", "A recipe should have at least one tag.")
                };
                return new ServiceResult(false, ErrorCode.BadRequest.GetHashCode(), data: data);
            }
          

            // strong validate recipe
            var validationResult = newRecipe.StrongValidation();
            if (validationResult.Length > 0)
            {
              
                return new ServiceResult(false, ErrorCode.BadRequest.GetHashCode(), data: validationResult);
            }

            await AddAsync(newRecipe);
            await SaveChangesAsync();
            await transaction.CommitAsync();

            return new ServiceResult(true, SuccessCode.Created.GetHashCode());
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return new ServiceResult(false, ErrorCode.InternalServerError.GetHashCode(), ex.Message);
        }
    }

    public async Task<ServiceResult> UpdateRecipeAsync(int userId, int id, RecipeUpdateDto recipe)
    {
        // get by id -> check exist -> map to history -> map recipe to currentRecipe -> strong validate -> add history -> saveChanges
        await using var transaction = await BeginTransactionAsync();
        try
        {
            var currentRecipe = await GetByIdAsync(id);
            if (currentRecipe == null)
            {
                return new ServiceResult(false, ErrorCode.NotFound.GetHashCode());
            }

            var recipeHistory = _mapper.Map<RecipeHistory>(currentRecipe);
            recipeHistory.CreatedBy = userId.ToString();
            recipeHistory.RecipeID = currentRecipe.ID;

            _mapper.Map(recipe, currentRecipe);
            currentRecipe.LastedVersion++;

            // strong validate recipe
            var validationResult = currentRecipe.StrongValidation();
            if (validationResult.Length > 0)
            {
                return new ServiceResult(false, ErrorCode.BadRequest.GetHashCode(), data: validationResult);
            }

            await _historyService.AddAsync(recipeHistory);
            await SaveChangesAsync();

            await transaction.CommitAsync();
            return new ServiceResult(true, SuccessCode.OK.GetHashCode());
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return new ServiceResult(false, ErrorCode.BadRequest.GetHashCode(), ex.Message);
        }
    }
}