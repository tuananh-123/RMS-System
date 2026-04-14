using System.ComponentModel.DataAnnotations;
using RMS.Entities;

public class Tag : BaseEntity
{
    [Required]
    public string Color { get; set; } = string.Empty;
    public ICollection<TagForRecipe> TagForRecipes { get; set; } = null!;
}