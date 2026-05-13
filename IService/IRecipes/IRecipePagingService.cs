using RMS.Contants;
using RMS.Dtos;
using RMS.Dtos.Recipes;
using RMS.Entities;

namespace RMS.IService.IRecipes;

public interface IRecipePagingService
{
    Task<(bool, string, PageResult<RecipeListDto>?)> Execute(string? sortBy, RecipeFilterDto filter, CancellationToken ct, int pageNumber = 1, int pageSize = 15);
}