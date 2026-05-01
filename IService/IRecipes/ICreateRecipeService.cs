using RMS.Contants;
using RMS.Dtos;

namespace RMS.IService.IRecipes;

public interface ICreateRecipeService
{
    Task<ServiceResult> ExecuteAsync(string userId, RecipeCreateDto request);
}