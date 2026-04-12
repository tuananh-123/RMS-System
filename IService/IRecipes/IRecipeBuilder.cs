using RMS.Dtos;
using RMS.Entities;

namespace RMS.IService.IRecipes;

public interface IRecipeBuilder
{
    Recipe Build(Recipe recipe, RecipeCreateDto request);
}