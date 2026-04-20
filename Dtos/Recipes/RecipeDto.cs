using RMS.Contants;

namespace RMS.Dtos.Recipes;

public readonly record struct RecipeDto
{
    public int ID { get; init; }
    public string Title { get; init; }
    public int CookingTime { get; init; }
    public double Rating { get; init; }
    public int Servings { get; init; }
    public Nation Nation { get; init; }
    public string ImageUrl { get; init; }
}