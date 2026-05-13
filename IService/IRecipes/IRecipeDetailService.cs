using RMS.Contants;
using RMS.Dtos.Recipes;
using RMS.Entities;

namespace RMS.IService.IRecipes;

public interface IRecipeDetailService
{
    Task<(bool, string, RecipeDetailDto?)> GetRecipeDetailAsync(int recipeId);
}