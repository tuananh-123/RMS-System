using System.ComponentModel.DataAnnotations;

namespace RMS.Entities;

public class RecipeIngredient
{
    public string Unit { get; set; } = string.Empty;
    public double Quantity { get; set; }
    public Recipe Recipe { get; set; } = null!;
    public int RecipeID { get; set; }
    public Ingredient Ingredient { get; set; } = null!;
    public int IngredientID { get; set; }
}