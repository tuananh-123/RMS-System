using System.ComponentModel.DataAnnotations;
using RMS.Contants;
using RMS.Entities;

namespace RMS.Dtos;

public record RecipeUpdateDto
{
    [Required]
    public string Title { get; set; }

    [Required]
    public string Description { get; set; }

    [Required]
    public Nation Nation { get; set; }

    [Required]
    public int[] Cuisine { get; set; }

    [Required]
    public int Serving { get; set; }

    public double TotalCalories { get; set; }

    [Required]
    public int CookingTime { get; set; }

    [Required]
    public Difficulty Difficulty { get; set; }

    public string ImageCover { get; set; }
    public string VideoUrl { get; set; }
    public SearchKeyword SearchKeyword { get; set; }
}
