using RMS.Entities;
using RMS.IService;

namespace RMS.Service;

public class RecipeHistoryService(RMSDbContext context) : BaseService<RecipeHistory>(context), IRecipeHistoryService
{
    
}