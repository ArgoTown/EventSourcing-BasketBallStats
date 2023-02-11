using BasketballStats.Api.Application.CommandHandlers;
using BasketballStats.Api.Application.Commands;
using BasketballStats.Api.Application.Queries;
using BasketballStats.Api.Application.QueryHandlers;
using BasketballStats.Api.Controllers.Request;
using BasketballStats.Api.Controllers.Response;
using BasketballStats.Domain.Aggregate;
using BasketballStats.Domain.Events;
using BasketballStats.Domain.Models;
using BasketballStats.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace BasketballStats.Api.Controllers;

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

    /// <summary>
    /// An action to create details about the basketball game.
    /// </summary>
    /// <param name="gameDto">Requires GameDto object as an input.</param>
    /// <returns>Returns created details about the game.</returns>
    [HttpPost("game/create")]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(GameResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateGame(GameRequestDto gameDto)
    {
        var game = new Game(gameDto.TeamHomeId, gameDto.TeamAwayId, gameDto.GameTime);
        await _gameRepository.Add(game);

        return CreatedAtAction(
            nameof(GetGame),
            new { game.Id },
            new GameResponseDto(game.Id,
                gameDto.TeamHomeId!.Value,
                gameDto.TeamAwayId!.Value,
                gameDto.GameTime)
            );
    }

    /// <summary>
    /// An acion to retrieve details about existing game.
    /// </summary>
    /// <param name="id">Existing game id.</param>
    /// <returns>Returns details about the game.</returns>
    [HttpGet("game/{id}")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(GameResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetGame(Guid id)
    {
        var game = await _gameRepository.Get(id);
        return Ok(new GameResponseDto(game.Id, game.TeamHomeId, game.TeamAwayId, game.GameTime));
    }

    /// <summary>
    /// A game action to register positive statistic event for a player from current team.
    /// </summary>
    /// <param name="gameId">Requires game id.</param>
    /// <param name="teamId">Requires team id.</param>
    /// <param name="playerId">Requires player id.</param>
    /// <param name="statistic">Requires positive statistic happened.</param>
    /// <returns>Returns created resource with location to retrieve it.</returns>
    [HttpPost("game/{gameId}/team/{teamId}/player/{playerId}/positive/event")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

    /// <summary>
    /// A game action to register negative statistic event for a player from current team.
    /// </summary>
    /// <param name="gameId">Requires game id.</param>
    /// <param name="teamId">Requires team id.</param>
    /// <param name="playerId">Requires player id.</param>
    /// <param name="statistic">Requires negative statistic happened.</param>
    /// <returns>Returns created resource with location to retrieve it.</returns>
    [HttpPost("game/{gameId}/team/{teamId}/player/{playerId}/negative/event")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

    /// <summary>
    /// An action to return player statistics from certain team of existing game.
    /// </summary>
    /// <param name="gameId">Requires game id.</param>
    /// <param name="teamId">Requires team id.</param>
    /// <param name="playerId">Requires player id.</param>
    /// <returns>Returns player statistics.</returns>
    [HttpGet("game/{gameId}/team/{teamId}/player/{playerId}/stats")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(PlayerStatsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

    /// <summary>
    /// Return all happened events for particular game.
    /// </summary>
    /// <param name="gameId">Provide game id as input.</param>
    /// <returns>Return all events.</returns>
    [HttpGet("game/{gameId}/events")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(IEnumerable<StreamDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllGameEvents(Guid gameId)
    {
        var game = await _gameRepository.Get(gameId);
        if (game is null)
        {
            return BadRequest($"Game {gameId} not exists.");
        }

        var gameStreamEvents = await _eventStoreRepository.Get(gameId);
        return Ok(gameStreamEvents.Select(stream => new StreamDto(stream.Id, stream.StreamId, stream.EventId, stream.Type, stream.Data, stream.MetaData, stream.CreatedAt, stream.Version)));
    }
}