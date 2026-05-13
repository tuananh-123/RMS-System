using System;
using System.Data.Common;
using System.Text.Json;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation.Validators;
using Microsoft.EntityFrameworkCore;
using RMS.Contants;
using RMS.Dtos;
using RMS.Dtos.Recipes;
using RMS.Entities;
using RMS.Helpers;
using RMS.IService.IRecipes;
using StackExchange.Redis;

namespace RMS.Service.Recipes;

public class RecipePagingService(
    RMSDbContext context,
    IConnectionMultiplexer mux,
    IMapper mapper,
    ILogger<RecipePagingService> logger
) : BaseService(context, mapper), IRecipePagingService
{
    private readonly ILogger<RecipePagingService> _logger = logger;
    private readonly IDatabase _redis = mux.GetDatabase();

    public async Task<(bool, string, PageResult<RecipeListDto>?)> Execute(string? sortBy, RecipeFilterDto filter, CancellationToken ct, int page = 1, int pageSize = 15)
    {
        try
        {
            string cacheKey = BuildCacheKey(page, pageSize, sortBy, filter);
            var cached = await _redis.StringGetAsync(cacheKey).WaitAsync(ct);
            
            // hit
            if (cached.HasValue)
            {
                var pageResult = JsonSerializer.Deserialize<PageResult<RecipeListDto>>(cached.ToString());
                return (true, "Fetch page successfully", pageResult);
            }

            // miss
            var result = await GetFromDatabase(page, pageSize, sortBy, filter, ct);

            var serialized = JsonSerializer.Serialize(result);

            await _redis.StringSetAsync(cacheKey, serialized, TimeSpan.FromMinutes(5)).WaitAsync(ct);

            return (true, "Recipes fetched successfully.", result);
       
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Request was cancelled.");
            throw; // Để ASP.NET Core tự xử lý
        }
        catch (RedisConnectionException connectionEx)
        {
            _logger.LogError("Redis connection failed with error: {error}.\nfalling back to database", connectionEx);
            
            var result = await GetFromDatabase(page, pageSize, sortBy, filter, ct);
            return (true, "Recipes fetched successfully.", result);
        }
        catch (DbException dbEx)
        {
            _logger.LogError(dbEx, "A database error occurred while fetching paginated recipes.");
            return (false, "A database error occurred while fetching recipes.", null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching paginated recipes.");
            return (false, "An error occurred while fetching recipes.", null);
        }
    }

    private static string BuildCacheKey(int page, int pageSize, string? sortBy, RecipeFilterDto filter) => 
    $"recipe:list:{page}:{pageSize}:" +
    $"{string.Join(',', filter.Categories ?? [])}:" +
    $"{string.Join(',', filter.Ingredients ?? [])}:" +
    $"{filter.CookingTime ?? 0}:" +
    $"{filter.Difficulty ?? 0}:" + 
    $"{filter.Keyword?.ToSlug() ?? "_"}:" + 
    $"{sortBy}";

    private async Task<PageResult<RecipeListDto>?> GetFromDatabase(int page, int pageSize, string? sortBy, RecipeFilterDto filter, CancellationToken ct)
    {
        var query = _dbContext.Recipes
                .AsNoTracking()
                .AsSplitQuery()
                .Where(r => !r.Trash);

        if (filter.Categories is {Length: > 0}) query = query.Where(r => r.TagForRecipes.Any(t => filter.Categories.Contains(t.TagID)));
        
        if (filter.Ingredients is {Length: > 0}) query = query.Where(r => r.RecipeIngredients.Any(i => filter.Ingredients.Contains(i.IngredientID)));

        if (filter.CookingTime != null) query = query.Where(r => r.CookingTime == filter.CookingTime);

        if (filter.Difficulty != null) query = query.Where(r => r.Difficulty == filter.Difficulty);

        if (!string.IsNullOrWhiteSpace(filter.Keyword)) query = query.Where(r => EF.Functions.ILike(r.Title, $"%{filter.Keyword}%"));

        query = sortBy switch
        {
            "views"  => query.OrderByDescending(r => r.Views),
            "rating" => query.OrderByDescending(r => r.Rating),
            _        => query.OrderByDescending(r => r.CreatedAt)
        };

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ProjectTo<RecipeListDto>(_mapper.ConfigurationProvider)
            .ToListAsync(ct);

        var totalPage = (int)Math.Ceiling((double)totalCount / pageSize);

        return new PageResult<RecipeListDto>(items, totalCount, page, pageSize, totalPage);
    }

}