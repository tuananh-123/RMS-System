using RMS.Contants;
using RMS.Dtos.Recipes;
using RMS.Entities;

namespace RMS.IService.IRecipes;

public interface IRecipePagingService
{
    Task<ServiceResult> GetRecipePagingAsync(int pageNumber, int pageSize);
}