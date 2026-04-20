using System.ComponentModel.DataAnnotations;
using RMS.Contants;
using RMS.Entities;

namespace RMS.Dtos;

public record RecipeUpdateDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Nation Nation { get; set; }
    public int[] Cuisine { get; set; } = [];
    public int Serving { get; set; }
    public double TotalCalories { get; set; }
    public int CookingTime { get; set; }
    public Difficulty Difficulty { get; set; }
    public string ImageCover { get; set; } = string.Empty;
    public string VideoUrl { get; set; } = string.Empty;
    public int LastedVersion { get; set; } = -1;
    public SearchKeyword? SearchKeyword { get; set; } = null;   
    public int[] Tags { get; set; } = [];
    public List<RecipeIngredientCreateDto> RecipeIngredients { get; set; } = [];
}
