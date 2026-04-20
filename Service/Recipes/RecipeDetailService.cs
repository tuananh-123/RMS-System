using System.Data.Common;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RMS.Contants;
using RMS.Dtos.Recipes;
using RMS.IService.IRecipes;

namespace RMS.Service.Recipes;

public class RecipeDetailService(
    RMSDbContext context,
    IMapper mapper,
    ILogger<RecipeDetailService> logger) : IRecipeDetailService
{
    private readonly RMSDbContext _context = context;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<RecipeDetailService> _logger = logger;

    public async Task<ServiceResult> GetRecipeDetailAsync(int recipeId)
    {
        try
        {
            if (recipeId <= 0)
            {
                _logger.LogWarning("Invalid recipeId {recipeId} provided for fetching recipe details", recipeId);
                return new ServiceResult(false, StatusCodes.Status400BadRequest, "Invalid recipe ID provided.");
            }

            var query = _context.Recipes
                .AsNoTracking()
                .Where(r => r.ID == recipeId && !r.Trash);

            var recipeCount = await query.CountAsync();
            if (recipeCount == 0)
            {
                _logger.LogInformation("Recipe with ID {recipeId} not found or is trashed", recipeId);
                return new ServiceResult(false, StatusCodes.Status404NotFound, "Recipe not found.");
            }

            var recipe = await query
                .AsSplitQuery()
                .Include(r => r.TagForRecipes)
                .ThenInclude(tr => tr.Tag)
                .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
                .FirstOrDefaultAsync();

            var recipeDetailDto = _mapper.Map<RecipeDetailDto>(recipe);

            return new ServiceResult(true, StatusCodes.Status200OK, "Recipe details fetched successfully.", recipeDetailDto);
        }
        catch (DbException dbEx)
        {
            _logger.LogError(dbEx, "A database error occurred while fetching recipe details.");
            return new ServiceResult(false, StatusCodes.Status500InternalServerError, "A database error occurred while fetching recipe details.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching recipe details for recipeId {recipeId}", recipeId);
            return new ServiceResult(false, StatusCodes.Status500InternalServerError, "An error occurred while fetching recipe details.");
        }
    }
}