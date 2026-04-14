using System.ComponentModel.DataAnnotations;
using RMS.Entities;

namespace RMS.Entities;

public class Ingredient : BaseEntity
{
    public string Information { get; set; } = string.Empty;
    public int CaloriePer100Gram { get; set; }
    public SearchKeyword SearchKeyword { get; set; } = null!;
    public ICollection<RecipeIngredient> RecipeIngredients { get; set; } = null!;
}