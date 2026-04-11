using RMS.Contants;
using RMS.Dtos;
using RMS.Entities;

namespace RMS.IService;

public interface IRecipeService : IBaseService<Recipe>
{
    Task<ServiceResult> UpdateRecipeAsync(int userId, int id, RecipeUpdateDto recipe);
    Task<ServiceResult> AddRecipeAsync(int userId, RecipeCreateDto recipe);
}
