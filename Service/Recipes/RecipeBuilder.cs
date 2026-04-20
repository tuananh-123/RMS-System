using Microsoft.EntityFrameworkCore.Internal;
using RMS.Dtos;
using RMS.Entities;
using RMS.IService.IRecipes;

namespace RMS.Service.Recipes;

public class RecipeBuilder
{
    public Recipe BuildCreate(Recipe recipe, RecipeCreateDto request)
    {
        recipe.TotalCalories = CalculateTotalCalories([.. request.RecipeIngredients]);

        recipe.TagForRecipes = BuildTagForRecipes(request.Tags);

        recipe.RecipeIngredients = BuildRecipeIngredients([.. request.RecipeIngredients]);
        
        return recipe;
    }

    public Recipe BuildUpdate(Recipe recipe, RecipeUpdateDto request)
    {
        recipe.LastedVersion = UpdateVersion(recipe.LastedVersion);

        recipe.TotalCalories = CalculateTotalCalories([.. request.RecipeIngredients]);

        recipe.TagForRecipes = BuildTagForRecipes(request.Tags);

        recipe.RecipeIngredients = BuildRecipeIngredients([.. request.RecipeIngredients]);

        return recipe;
    }

    public RecipeHistory BuildRecipeHistory(Recipe recipe)
    {
        return new RecipeHistory
        {
            Description = recipe.Description,
            Nation = recipe.Nation,
            Cuisine = [.. recipe.Cuisine],
            Serving = recipe.Serving,
            TotalCalories = recipe.TotalCalories,
            CookingTime = recipe.CookingTime,
            Difficulty = recipe.Difficulty,
            ImageCover = recipe.ImageCover,
            VideoUrl = recipe.VideoUrl,
            VersionNumber = recipe.LastedVersion,
            Title = recipe.Title,
            CreatedAt = recipe.CreatedAt,
            CreatedBy = recipe.CreatedBy
        };
    }

    private static int UpdateVersion(int currentVersion) => currentVersion + 1;

    private static double CalculateTotalCalories(RecipeIngredientCreateDto[] data) => data.Sum(ri => ri.CaloPer100Gram * ri.Quantity / 100);

    private static ICollection<TagForRecipe> BuildTagForRecipes(int[] data) => [.. data.Select(tagId => new TagForRecipe { TagID = tagId })];
  
    private static ICollection<RecipeIngredient> BuildRecipeIngredients(RecipeIngredientCreateDto[] data)
    {
        return [.. data.Select(ri => new RecipeIngredient
        {
            IngredientID = ri.ID,
            Quantity = ri.Quantity,
            Unit = ri.Unit
        })];
    }

}