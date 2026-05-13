using RMS.Contants;

namespace RMS.Dtos.Recipes;

public readonly struct RecipeListDto
{
    public int ID { get; init; }
    public string Title { get; init; }
    public int CookingTime { get; init; }
    public double Rating { get; init; }
    public int Serving { get; init; }
    public string Nation { get; init; }
    public string ImageCover { get; init; }
}