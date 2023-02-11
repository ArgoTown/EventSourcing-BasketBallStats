using BasketballStats.Domain.Repositories;

namespace BasketballStats.Infrastructure.Repositories;

internal sealed class GameInMemoryRepository : IGameRepository
{
    private static readonly List<Domain.Entities.Game> GameDatabase = new();
    private static readonly IList<Domain.Entities.Game> ReadOnlyGameDatabase = GameDatabase.AsReadOnly();

    public Task Add(Domain.Aggregate.Game game)
    {
        var lastEntity = ReadOnlyGameDatabase.LastOrDefault();

        var entity = new Domain.Entities.Game
        {
            Id = lastEntity is null ? 1 : lastEntity.Id + 1,
            GameId = game.Id,
            TeamAwayId = game.TeamAwayId,
            TeamHomeId = game.TeamHomeId,
            GameTime = game.GameTime
        };

        GameDatabase.Add(entity);
        return Task.CompletedTask;
    }

    public Task<Domain.Aggregate.Game> Get(Guid gameId)
    {
        var gameEntity = ReadOnlyGameDatabase.FirstOrDefault(game => game.GameId.Equals(gameId));
        if (gameEntity is null)
        {
            throw new ArgumentNullException($"Game with id {gameId} not exists in database");
        }

        return Task.FromResult(new Domain.Aggregate.Game(gameEntity.TeamHomeId, gameEntity.TeamAwayId, gameEntity.GameTime));
    }
}
