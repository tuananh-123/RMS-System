using RMS.Contants;

namespace RMS.IService.IIngredients.ICreate;

public interface IIngredientCreateByFileInputService
{
    Task<ServiceResult> ExecuteAsync(int userId, IFormFile file);
}