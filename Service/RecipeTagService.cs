using AutoMapper;
using RMS.Entities;
using RMS.IService;

namespace RMS.Service;

public class RecipeTagService(RMSDbContext context): BaseService<TagForRecipe>(context), IRecipeTagService
{
}