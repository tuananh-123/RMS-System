using System.ComponentModel.DataAnnotations;
using RMS.Contants;
using RMS.Entities;

namespace RMS.Dtos;

public record RecipeCreateDto
{
    public string Title { get; set { if (value != null) value = value.Trim(); }} = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Nation Nation { get; set; }
    public int[] Cuisine { get; set; } = [];
    public int Serving { get; set; }
    public double TotalCalories { get; set; }
    public int CookingTime { get; set; }
    public Difficulty Difficulty { get; set; }
    public string ImageCover { get; set; } = string.Empty;
    public string VideoUrl { get; set; } = string.Empty;
    // business rule
    public SearchKeyword? SearchKeyword { get; set; } = null;   
    public int[] Tags { get; set; } = [];
    public List<RecipeIngredientCreateDto> RecipeIngredients { get; set; } = [];
}

public class RecipeIngredientCreateDto
{
    public int ID { get; set; }
    public double Quantity { get; set; }
    public string Unit { get; set { if (value != null) value = value.Trim(); } } = string.Empty;
    public int CaloPer100Gram { get; set; }
}