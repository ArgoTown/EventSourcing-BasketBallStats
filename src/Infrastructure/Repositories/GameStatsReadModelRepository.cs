using BasketballStats.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BasketballStats.Infrastructure.Repositories;

internal sealed class GameStatsReadModelRepository : IGameStatsReadModel
{
    private readonly EventsContext _context;

    public GameStatsReadModelRepository(EventsContext context)
    {
        _context = context;
    }

    public async Task Add(GameStatsReadModel statistics)
    {
        await _context.ReadModelStatistics.AddAsync(statistics);
        await _context.SaveChangesAsync();
    }

    public async Task Update(GameStatsReadModel statistics)
    {
        _context.Update(statistics);
        await _context.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<GameStatsReadModel>> Get(Guid gameId) => await _context
            .ReadModelStatistics
            .Where(stats => stats.GameId.Equals(gameId))
            .ToListAsync();
}
