using BasketballStats.Domain.Entities;
using System.Collections.Concurrent;

namespace BasketballStats.Infrastructure.Repositories;

internal sealed class GameStatsInMemoryReadModelRepository : IGameStatsReadModel
{
    private static ConcurrentDictionary<long, GameStatsReadModel> ReadModelStatistics = new();

    public async Task Add(GameStatsReadModel statistics)
    {
        if (!ReadModelStatistics.ContainsKey(statistics.Id))
        {
            var uniqueId = await GetLastId();
            ReadModelStatistics.TryAdd(uniqueId + 1, statistics);
        }

        await Task.CompletedTask;
    }

    public async Task Update(GameStatsReadModel statistics)
    {
        ReadModelStatistics.Remove(statistics.Id, out var removedStats);
        ReadModelStatistics.TryAdd(statistics.Id, statistics);
        await Task.CompletedTask;
    }

    public async Task<IReadOnlyList<GameStatsReadModel>> Get(Guid gameId)
    {
        var response = ReadModelStatistics
            .Values
            .Where(gameStats => gameStats.GameId.Equals(gameId))
            .OrderBy(gameStats => gameStats.Id)
            .ToList();

        await Task.CompletedTask;

        return response;
    }

    private Task<long> GetLastId()
    {
        if (ReadModelStatistics.Any())
        {
            return Task.FromResult(ReadModelStatistics.Last().Key);
        }

        return Task.FromResult(0L);
    }
}
