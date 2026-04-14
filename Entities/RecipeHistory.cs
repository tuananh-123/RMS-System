using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RMS.Contants;
namespace RMS.Entities;

public class RecipeHistory : BaseEntity
{
    
    public string  Description { get; set; } = string.Empty;
    [Column(TypeName = "int")]
    public Nation Nation { get; set; } = Nation.Unknown;
    public int[] Cuisine { get; set; } = [];
    public int Serving { get; set; }
    public double TotalCalories { get; set; }
    public int CookingTime { get; set; }
    [Column(TypeName = "int")]
    public Difficulty Difficulty { get; set; } = Difficulty.Unknown;
    public string ImageCover { get; set; } = string.Empty;
    public string VideoUrl { get; set; } = string.Empty;
    public int VersionNumber { get; set; }
    public Recipe Recipe { get; set; } = null!;
    public int RecipeID { get; set; }
}