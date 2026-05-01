using StackExchange.Redis;

namespace RMS.Service.Recipes;

public class ViewCounterService(
    IConnectionMultiplexer mux
)
{
    private readonly IDatabase _redis = mux.GetDatabase();

    public async Task IncrementAsync(int recipeId) => await _redis.StringIncrementAsync($"recipe:views:{recipeId}");

    public async Task<long> GetPendingAsync(int recipeId)
    {
        var val = await _redis.StringGetAsync($"recipe:views:{recipeId}");
        return val.HasValue ? (long)val : 0;
    }
}