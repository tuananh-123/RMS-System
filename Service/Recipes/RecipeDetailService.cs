using System.Data.Common;
using System.Text.Json;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using RMS.Contants;
using RMS.Dtos.Recipes;
using RMS.Entities;
using RMS.IService.IRecipes;
using StackExchange.Redis;

namespace RMS.Service.Recipes;

public class RecipeDetailService(
    RMSDbContext context,
    IConnectionMultiplexer mux,
    IMapper mapper,
    ILogger<RecipeDetailService> logger) : IRecipeDetailService
{
    private readonly RMSDbContext _context = context;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<RecipeDetailService> _logger = logger;
    private readonly IDatabase _redis = mux.GetDatabase();

    public async Task<(bool, string, RecipeDetailDto?)> GetRecipeDetailAsync(int recipeId)
    {
        try
        {
            var cachedKey = $"recipe:{recipeId}";
            var redisValue = await _redis.StringGetAsync(cachedKey);

            if (redisValue.HasValue)
            {
                string converted = redisValue.ToString();
                var deserialized = JsonSerializer.Deserialize<RecipeDetailDto>(converted);
                return (true, "Get recipe Detail successfully", deserialized);
            }
            var recipe = await GetFromDatabase(recipeId);

            if (recipe == null)
                return (false, "Recipe doesn't exists", null);

            var dto = _mapper.Map<RecipeDetailDto>(recipe);

            var serialized = JsonSerializer.Serialize(dto);
            await _redis.StringSetAsync(cachedKey, serialized, TimeSpan.FromMinutes(5));
    
            return (true, "Recipe details fetched successfully.", dto);
        }
        catch (RedisConnectionException connectionEx)
        {
            _logger.LogWarning(connectionEx, "Redis unavailable, falling back to DB");

            var recipe = await GetFromDatabase(recipeId);

            if (recipe == null)                    
                return (false, "Recipe doesn't exist", null);

            var dto = _mapper.Map<RecipeDetailDto>(recipe);
            return (true, "Recipe details fetched successfully.", dto);
        }
        catch (DbException dbEx)
        {
            _logger.LogError(dbEx, "A database error occurred while fetching recipe details.");
            return (false, "A database error occurred while fetching recipe details.", null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching recipe details for recipeId {recipeId}", recipeId);
            return (false, "An error occurred while fetching recipe details.", null);
        }
    }

    private async Task<Recipe?> GetFromDatabase(int recipeId) => await _context.Recipes
        .AsNoTracking()
        .Where(r => r.ID == recipeId && !r.Trash)
        .AsSplitQuery()
        // .Include(r => r.RecipeHistories)
        .Include(r => r.TagForRecipes)
            .ThenInclude(tr => tr.Tag)
        .Include(r => r.RecipeIngredients)
            .ThenInclude(ri => ri.Ingredient)
        .FirstOrDefaultAsync();

}