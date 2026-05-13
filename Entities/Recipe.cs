using System.ComponentModel.DataAnnotations;

namespace RMS.Entities;

public class Recipe : BaseEntity
{
    public string  Description { get; set; } = string.Empty;
    public int Nation { get; set; } = -1;
    public int[] Cuisine { get; set; } = [];
    public int Serving { get; set; } = -1;
    // map ignore
    public double TotalCalories { get; set; } = -1;
    public int CookingTime { get; set; }  = -1;
    public int Difficulty { get; set; } = -1;
    // map ignore
    public int Views { get; set; } = 0;
    // map ignore
    public double Rating { get; set; } = 0.0;
    public string ImageCover { get; set; } = string.Empty;
    public string VideoUrl { get; set; } = string.Empty;
    // map ignore
    public int LastedVersion { get; set; } = 1;
    // Optimistic locking for concurrent updates
    [Timestamp]
    public byte[]? RowVersion { get; set; }
    public SearchKeyword? SearchKeyword { get; set; } = null;
    // map ignore
    public ICollection<TagForRecipe> TagForRecipes { get; set; } = [];
    // map ignore
    public ICollection<RecipeIngredient> RecipeIngredients { get; set; } = [];
    // map ignore
    public ICollection<RecipeHistory> RecipeHistories { get; set; } = [];
}