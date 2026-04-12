using RMS.Dtos;
using RMS.Entities;
using RMS.IService.IRecipes;

namespace RMS.Service.Recipes;

public class RecipeBuilder : IRecipeBuilder
{
    public Recipe Build(Recipe recipe, RecipeCreateDto request)
    {
        recipe.TotalCalories = request.RecipeIngredients.Sum(ri => ri.CaloPer100Gram);
        recipe.TagForRecipes = [.. request.Tags!.Select(tagId => new TagForRecipe { TagID = tagId })];
        recipe.RecipeIngredients =[.. request.RecipeIngredients.Select(ri => new RecipeIngredient
        {
            IngredientID = ri.ID,
            Quantity = ri.Quantity,
            Unit = ri.Unit
        })];
        return recipe;
    }
}