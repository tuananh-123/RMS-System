using System.ComponentModel.DataAnnotations;
using RMS.Entities;

namespace RMS.Entities;

public class Tag : BaseEntity
{
    public int Usage_Count { get; set; } = 0;
    public string Color { get; set; } = string.Empty;
    public string Color_Hex { get; set; } = string.Empty;
    public ICollection<TagForRecipe> TagForRecipes { get; set; } = null!;
}