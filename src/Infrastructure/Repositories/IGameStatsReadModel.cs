using BasketballStats.Domain.Entities;

namespace BasketballStats.Infrastructure.Repositories
{
    public interface IGameStatsReadModel
    {
        Task Add(GameStatsReadModel statistics);
        Task<IReadOnlyList<GameStatsReadModel>> Get(Guid gameId);
        Task Update(GameStatsReadModel statistics);
    }
}
