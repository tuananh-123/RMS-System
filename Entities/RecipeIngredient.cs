using System.ComponentModel.DataAnnotations;

namespace RMS.Entities;

public class RecipeIngredient
{
    [Required]
    public string Unit { get; set; }
    [Required]
    public double Quantity { get; set; }
    public Recipe Recipe { get; set; }
    public int RecipeID { get; set; }
    public Ingredient Ingredient { get; set; }
    public int IngredientID { get; set; }
}