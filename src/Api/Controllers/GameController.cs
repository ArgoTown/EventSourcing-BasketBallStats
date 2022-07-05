using BasketballStats.Api.Application.CommandHandlers;
using BasketballStats.Api.Application.Commands;
using BasketballStats.Api.Application.Queries;
using BasketballStats.Api.Application.QueryHandlers;
using BasketballStats.Api.Controllers.Models;
using BasketballStats.Domain.Aggregate;
using BasketballStats.Domain.Events;
using BasketballStats.Domain.Models;
using BasketballStats.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace BasketballStats.Api.Controllers
{
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly ILogger<GameController> _logger;
        private readonly IGameRepository _gameRepository;
        private readonly ICommandHandler _handler;
        private readonly IEventStoreRepository _eventStoreRepository;
        private readonly IQueryHandler<GetPlayerStatisticsQuery, PlayerStats> _playerStatsHandler;

        public GameController(
            ILogger<GameController> logger,
            IGameRepository gameRepository,
            ICommandHandler handler,
            IEventStoreRepository eventStoreRepository,
            IQueryHandler<GetPlayerStatisticsQuery, PlayerStats> playerStatsHandler)
        {
            _logger = logger;
            _gameRepository = gameRepository;
            _eventStoreRepository = eventStoreRepository;
            _handler = handler;
            _playerStatsHandler = playerStatsHandler;
        }

        [HttpPost("game/create")]
        public async Task<IActionResult> CreateGame(GameDto gameDto)
        {
            var game = new Game(gameDto.TeamHomeId, gameDto.TeamAwayId, gameDto.GameTime);
            await _gameRepository.Add(game);

            return CreatedAtAction(
                nameof(GetGame),
                new { game.Id },
                new
                {
                    game.Id,
                    gameDto.TeamHomeId,
                    gameDto.TeamAwayId,
                    gameDto.GameTime
                });
        }

        [HttpGet("game/{id}")]
        public async Task<IActionResult> GetGame(Guid id)
        {
            var game = await _gameRepository.Get(id);
            return Ok(game);
        }

        [HttpPost("game/{gameId}/team/{teamId}/player/{playerId}/positive/event")]
        public async Task<IActionResult> AddPositiveStatistic(Guid gameId, Guid teamId, Guid playerId, PositiveStatistic statistic)
        {
            var game = await _gameRepository.Get(gameId);
            if (game is null)
            {
                return BadRequest($"Game {gameId} not exists.");
            }

            if (!game.TeamAwayId.Equals(teamId) && !game.TeamHomeId.Equals(teamId))
            {
                return BadRequest($"Team {teamId} not exists.");
            }

            var player = new Player(gameId, teamId, playerId);
            await _handler.Handle(new AddPlayerPositiveStatisticCommand { Player = player, PositiveStatistic = statistic });

            return CreatedAtAction(nameof(GetPlayerStatistics), new { GameId = gameId, TeamId = teamId, PlayerId = playerId }, null);
        }

        [HttpPost("game/{gameId}/team/{teamId}/player/{playerId}/negative/event")]
        public async Task<IActionResult> AddNegativeStatistic(Guid gameId, Guid teamId, Guid playerId, NegativeStatistic statistic)
        {
            var game = await _gameRepository.Get(gameId);
            if (game is null)
            {
                return BadRequest($"Game {gameId} not exists.");
            }

            if (!game.TeamAwayId.Equals(teamId) && !game.TeamHomeId.Equals(teamId))
            {
                return BadRequest($"Team {teamId} not exists.");
            }

            var player = new Player(gameId, teamId, playerId);
            await _handler.Handle(new AddPlayerNegativeStatisticCommand { Player = player, NegativeStatistic = statistic });

            return CreatedAtAction(nameof(GetPlayerStatistics), new { GameId = gameId, TeamId = teamId, PlayerId = playerId }, null);
        }

        [HttpGet("game/{gameId}/team/{teamId}/player/{playerId}/stats")]
        public async Task<IActionResult> GetPlayerStatistics(Guid gameId, Guid teamId, Guid playerId)
        {
            var game = await _gameRepository.Get(gameId);
            if (game is null)
            {
                return BadRequest($"Game {gameId} not exists.");
            }

            if (!game.TeamAwayId.Equals(teamId) && !game.TeamHomeId.Equals(teamId))
            {
                return BadRequest($"Team {teamId} not exists.");
            }

            var result = await _playerStatsHandler.Query(new GetPlayerStatisticsQuery(new Player(gameId, teamId, playerId)));

            return Ok(new PlayerStatsDto
            {
                FreeThrowPercentage = result.FreeThrowPercentage,
                ThreePointPercentage = result.ThreePointPercentage,
                TotalPoints = result.TotalPoints,
                TwoPointPercentage = result.TwoPointPercentage
            });
        }

        [HttpGet("game/{gameId}/events")]
        public async Task<IActionResult> GetAllGameEvents(Guid gameId)
        {
            var game = await _gameRepository.Get(gameId);
            if (game is null)
            {
                return BadRequest($"Game {gameId} not exists.");
            }

            var gameStreamEvents = await _eventStoreRepository.Get(gameId);
            return Ok(gameStreamEvents);
        }
    }
}