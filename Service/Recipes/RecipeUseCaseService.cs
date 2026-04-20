using Microsoft.EntityFrameworkCore;
using RMS.Contants;

namespace RMS.Service.Recipes;

public class RecipeUseCaseService(RMSDbContext context)
{
    public async Task<bool> CheckTitleDuplicateAsync(string title)
    {
        return await context.Recipes.AnyAsync(r => r.Title.Trim().ToLower() == title.Trim().ToLower());
    }

    public async Task<HashSet<int>> GetExistingTagIdAsync(IEnumerable<int> tagIds)
    {
        return await context.Tags.AsNoTracking()
            .Where(t => tagIds.Contains(t.ID))
            .Select(t => t.ID)
            .ToHashSetAsync();
    }

    public async Task<HashSet<int>> GetExistingIngredientIdAsync(IEnumerable<int> ingredientIds)
    {
        return await context.Ingredients.AsNoTracking()
            .Where(i => ingredientIds.Contains(i.ID))
            .Select(i => i.ID)
            .ToHashSetAsync();
    }
}