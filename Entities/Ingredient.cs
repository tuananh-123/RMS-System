using System.ComponentModel.DataAnnotations;
using RMS.Entities;


public class Ingredient : BaseEntity
{
    [Required]
    public string Information { get; set; }
    [Required]
    public int CaloriePer100Gram { get; set; }
    public SearchKeyword SearchKeyword { get; set; }
    public ICollection<RecipeIngredient> RecipeIngredients { get; set; }
}