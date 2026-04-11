namespace RMS.Entities;

public class TagForRecipe
{
    public Recipe Recipe { get; set; }
    public int RecipeID { get; set; }
    public Tag Tag { get; set; }
    public int TagID { get; set; }
}