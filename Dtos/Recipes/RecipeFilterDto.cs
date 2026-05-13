using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace RMS.Dtos.Recipes;


public record RecipeFilterDto
{
    [FromQuery(Name = "Categories")]
    public int[]? Categories { get; set; }
    
    [FromQuery(Name = "Ingredients")]
    public int[]? Ingredients { get; set; }

    [FromQuery(Name = "CookingTime")]
    public int? CookingTime { get; set; }

    [FromQuery(Name = "Difficulty")]
    public int? Difficulty { get; set; }

    [FromQuery(Name = "Keyword")]
    public string? Keyword { get; set; }
}