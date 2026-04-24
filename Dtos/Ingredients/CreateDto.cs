using RMS.Entities;

namespace RMS.Dtos.Ingredients;

public class IngredientCreateDto
{
    public string Title { get; set; } = string.Empty;
    public string Information { get; set; } = string.Empty;
    public int CaloriePer100Gram { get; set; }
    public SearchKeyword? SearchKeyword { get; set; } = null;
}