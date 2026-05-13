using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RMS.Contants;
namespace RMS.Entities;

public class RecipeHistory : BaseEntity
{
    
    public string  Description { get; set; } = string.Empty;
    [Column(TypeName = "int")]
    public int Nation { get; set; } = -1;
    public int[] Cuisine { get; set; } = [];
    public int Serving { get; set; }
    public double TotalCalories { get; set; }
    public int CookingTime { get; set; }
    [Column(TypeName = "int")]
    public int Difficulty { get; set; } = -1;
    public string ImageCover { get; set; } = string.Empty;
    public string VideoUrl { get; set; } = string.Empty;
    public int VersionNumber { get; set; }
    public Recipe Recipe { get; set; } = null!;
    public int RecipeID { get; set; }
}