using RMS.Entities;
using RMS.IService;

namespace RMS.Service;

public class RecipeIngredientService(RMSDbContext context): BaseService<RecipeIngredient>(context), IRecipeIngredientService
{
}