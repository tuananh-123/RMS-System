using RMS.Contants;
using RMS.Entities;

namespace RMS.Dtos.Recipes;

public readonly struct RecipeDetailDto
{
    public int ID { get; init; }
    public string Title { get; init; }
    public string Description { get; init; }
    public Nation Nation { get; init; }
    public int[] Cuisine { get; init; }
    public int Servings { get; init; }
    public double TotalCalories { get; init; }
    public int CookingTime { get; init; }
    public Difficulty Difficulty { get; init; }
    public int Views { get; init; }
    public double Rating { get; init; }
    public string ImageCover { get; init; }
    public string VideoUrl { get; init; }
    public int LastedVersion { get; init; }
    public SearchKeyword SearchKeyword { get; init; }
    public DateTime CreatedAt { get; init; }
    public string CreatedBy { get; init; }
    public List<TagDto> Tags { get; init; }
    public List<IngredientDto> Ingredients { get; init; }

}

public readonly struct TagDto
{
    public int ID { get; init; }
    public string Title { get; init; }
    public string Color { get; init; }
}

public readonly struct IngredientDto
{
    public int ID { get; init; }
    public string Title { get; init; }
    public string Unit { get; init; }
    public double Quantity { get; init; }
}

public readonly struct HistoryDto
{
    
}