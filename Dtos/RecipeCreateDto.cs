using System.ComponentModel.DataAnnotations;
using RMS.Contants;
using RMS.Entities;

namespace RMS.Dtos;

public record RecipeCreateDto
{
    public string Title { get; set; }
    public string Description { get; set; }
    public Nation Nation { get; set; }
    public int[] Cuisine { get; set; }
    public int Serving { get; set; }
    public double TotalCalories { get; set; }
    public int CookingTime { get; set; }
    public Difficulty Difficulty { get; set; }
    public string ImageCover { get; set; }
    public string VideoUrl { get; set; }
    public SearchKeyword SearchKeyword { get; set; }
    public int[]? Tags { get; set; } = null;
    public List<RecipeIngredientCreateDto> RecipeIngredients { get; set; }
}

public class RecipeIngredientCreateDto
{
    public int ID { get; set; }
    public double Quantity { get; set; }
    public string Unit { get; set; }
    public int CaloPer100Gram { get; set; }
}