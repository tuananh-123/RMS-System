using RMS.Contants;
using RMS.Entities;

namespace RMS.Dtos.Recipes;

public readonly record struct RecipeDetailDto
{
    public int ID { get; init; }
    public string Title { get; init; }
    public DateTime CreatedAt { get; init; }
    public string CreatedBy { get; init; }  
    public string Description { get; init; }
    public string Nation { get; init; }
    public int[] Cuisine { get; init; }
    public int Servings { get; init; }
    public double TotalCalories { get; init; }
    public int CookingTime { get; init; }
    public string Difficulty { get; init; }
    public int Views { get; init; }
    public double Rating { get; init; }
    public string ImageUrl { get; init; }
    public string VideoUrl { get; init; }
    public int NumberOfVersions { get; init; }
    public SearchKeyword SearchKeyword { get; init; }
}