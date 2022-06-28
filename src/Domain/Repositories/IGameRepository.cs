using BasketballStats.Domain.Aggregate;

namespace BasketballStats.Domain.Repositories
{
    public interface IGameRepository
    {
        Task Add(Game game);
        Task<Game> Get(Guid gameId);
    }
}
