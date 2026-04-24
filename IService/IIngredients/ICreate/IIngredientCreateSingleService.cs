using RMS.Contants;
using RMS.Dtos.Ingredients;

namespace RMS.IService.IIngredients.ICreate;

public interface IIngredientCreateSingleService
{
    Task<ServiceResult> ExecuteAsync(int userId, IngredientCreateDto request);
}