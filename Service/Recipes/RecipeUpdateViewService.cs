using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RMS.Entities;

namespace RMS.Service.Recipes;

public class RecipeUpdateViewService(
    RMSDbContext dbContext, IMapper mapper) : BaseService(dbContext, mapper)
{
    public async Task<(bool, string)> ExecuteAsync(int id)
    {
        var affected = await _dbContext.Recipes
            .Where(r => r.ID == id)
            .ExecuteUpdateAsync(s => s.SetProperty(r => r.Views, r => r.Views + 1));

        return affected > 0 
            ? (true, "Update views completed")
            : (false, "Recipe doesn't exist");
    }
}