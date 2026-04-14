namespace RMS.Entities;

public class TagForRecipe
{
    public Recipe Recipe { get; set; } = null!;
    public int RecipeID { get; set; }
    public Tag Tag { get; set; } = null!;
    public int TagID { get; set; }
}