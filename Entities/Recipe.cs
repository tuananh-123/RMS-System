using System.ComponentModel.DataAnnotations.Schema;
using RMS.Contants;

namespace RMS.Entities;

public class Recipe : BaseEntity
{
    public string  Description { get; set; } = string.Empty;
    public Nation Nation { get; set; } = Nation.Unknown;
    public int[] Cuisine { get; set; } = [];
    public int Serving { get; set; } = -1;
    // map ignore
    public double TotalCalories { get; set; } = -1;
    public int CookingTime { get; set; }  = -1;
    public Difficulty Difficulty { get; set; } = Difficulty.Unknown;
    // map ignore
    public int Views { get; set; } = 0;
    // map ignore
    public double Rating { get; set; } = 0.0;
    public string ImageCover { get; set; } = string.Empty;
    public string VideoUrl { get; set; } = string.Empty;
    // map ignore
    public int LastedVersion { get; set; } = 1;
    public SearchKeyword? SearchKeyword { get; set; } = null;
    // map ignore
    public ICollection<TagForRecipe> TagForRecipes { get; set; } = new List<TagForRecipe>();
    // map ignore
    public ICollection<RecipeIngredient> RecipeIngredients { get; set; } = new List<RecipeIngredient>();
    // map ignore
    public ICollection<RecipeHistory> RecipeHistories { get; set; } = new List<RecipeHistory>();
}