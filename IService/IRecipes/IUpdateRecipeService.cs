using RMS.Contants;
using RMS.Dtos;

namespace RMS.IService.IRecipes;

public interface IUpdateRecipeService 
{
    Task<ServiceResult> ExecuteSync(string userId, int recipeId, RecipeUpdateDto request);
}