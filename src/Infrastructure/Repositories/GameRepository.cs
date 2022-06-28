using BasketballStats.Domain.Aggregate;
using BasketballStats.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BasketballStats.Infrastructure.Repositories
{
    internal sealed class GameRepository : IGameRepository
    {
        private readonly EventsContext _context;

        public GameRepository(EventsContext context)
        {
            _context = context;
        }

        public async Task Add(Game game)
        {
            var entity = new Domain.Entities.Game
            {
                GameId = game.Id,
                TeamAwayId = game.TeamAwayId,
                TeamHomeId = game.TeamHomeId,
                GameTime = game.GameTime
            };

            _context.Games.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<Game> Get(Guid gameId)
        {
            var gameEntity = await _context.Games.FirstOrDefaultAsync(game => game.GameId.Equals(gameId));
            if (gameEntity is null)
            {
                throw new ArgumentNullException($"Game with id {gameId} not exists in database");
            }

            return new Game(gameEntity.TeamHomeId, gameEntity.TeamAwayId, gameEntity.GameTime);
        }
    }
}
