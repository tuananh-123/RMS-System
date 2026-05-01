using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace RMS.Background;

public class ViewFlushJob(
    IConnectionMultiplexer mux,
    IServiceScopeFactory scopeFactory
) : BackgroundService
{
    private readonly IConnectionMultiplexer _mux = mux;
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromSeconds(60), ct);
            await FlushAsync();
        }
    }

    private async Task FlushAsync()
    {
        var db = _mux.GetDatabase();
        var server = _mux.GetServer(_mux.GetEndPoints()[0]);

        // Quét tất cả key có pattern recipe:views:*
        var keys = server.Keys(pattern: "recipe:views:*").ToList();
        
        using var scope = _scopeFactory.CreateScope();
        var _dbContext = scope.ServiceProvider.GetRequiredService<RMSDbContext>();

        foreach (var key in keys)
        {
            // GetDelete = lấy giá trị rồi xoá key (atomic)
            var count = (long?)await db.StringGetDeleteAsync(key);
            if (count is null or 0) continue;

            var id = int.Parse(key.ToString().Split(':')[2]);
            await _dbContext.Recipes
                .Where(r => r.ID == id)
                .ExecuteUpdateAsync(ex => ex.SetProperty(r => r.Views, r => r.Views + count));
        }

    }
}