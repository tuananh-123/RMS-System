using RMS.Contants;

namespace RMS.IService.IRecipes;

public interface IRecipeDetailService
{
    Task<ServiceResult> GetRecipeDetailAsync(int recipeId);
}