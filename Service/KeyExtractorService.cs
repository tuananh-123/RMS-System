using System.Data.Common;
using System.Text.Json;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RMS.Helpers;
using StackExchange.Redis;

namespace RMS.Service;

public class KeyExtractorService(
    RMSDbContext context,
    IMapper mapper,
    IConnectionMultiplexer mux,
    ILogger<KeyExtractorService> logger
) : BaseService(context, mapper)
{
    private const string TagsCacheKey = "tags-normalized";
    private const string FallbackTag = "all";
    private readonly IDatabase _redis = mux.GetDatabase();
    private readonly ILogger<KeyExtractorService> _logger = logger;
    public async Task<string[]> ExtractKeywords(string input, CancellationToken ct)
    {
        string normalizedInput = input.ToSlug();

        try
        {
            var allTags = await GetTagsAsync(ct);
            return MatchTags(normalizedInput, allTags);
        }
        catch (RedisConnectionException redisEx)
        {
            _logger.LogWarning(redisEx, "Redis down falling back to database.");
            var tags = await QueryDatabase(ct);
            return MatchTags(normalizedInput, tags);
        }
        catch (DbException dbEx)
        {
            _logger.LogError(dbEx, "Database down");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An Error has occured");
            throw;
        }
    }

    private string[] MatchTags(string normalizedInput, string[] tags)
    {
        var matched = tags
                    .Where(t => normalizedInput.Contains(t))
                    .OrderBy(t => t)
                    .ToArray();
        return matched.Length > 0 ? matched : [FallbackTag];
    }

    private async Task<string[]> GetTagsAsync(CancellationToken ct)
    {
        var cached_data = await _redis.StringGetAsync(TagsCacheKey);

        // hit
        if (cached_data.HasValue)
        {
            var deserialized_data = JsonSerializer.Deserialize<string[]>(cached_data.ToString());
            return deserialized_data!;
        }

        // miss
        var query_data = await QueryDatabase(ct);
        var serialized_data = JsonSerializer.Serialize(query_data);
        await _redis.StringSetAsync(TagsCacheKey, serialized_data, TimeSpan.FromMinutes(5));

        return query_data;
    }

    private async Task<string[]> QueryDatabase(CancellationToken ct)
    {
        var query = _dbContext.Tags.Select(t => t.Slug);
        return await query.ToArrayAsync(ct);
    }
}