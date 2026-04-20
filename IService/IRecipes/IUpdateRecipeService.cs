using RMS.Contants;
using RMS.Dtos;

namespace RMS.IService.IRecipes;

public interface IUpdateRecipeService 
{
    Task<ServiceResult> ExecuteSync(int userId, int recipeId, RecipeUpdateDto request);
}