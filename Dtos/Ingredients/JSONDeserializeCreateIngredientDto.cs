namespace RMS.Dtos.Ingredients;

public class JSONDeserializeCreateIngredientDto
{
    public string Title { get; set; } = string.Empty;

    public string Information { get; set; } = string.Empty;

    public int CaloriePer100Gram { get; set; }

    public object? SearchKeyword { get; set; } = null;
}