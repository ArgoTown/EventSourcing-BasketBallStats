namespace BasketballStats.Api.Controllers.Response;

/// <summary>
/// Game details response.
/// </summary>
/// <param name="Id">Game id.</param>
/// <param name="TeamHomeId">Home team of a game.</param>
/// <param name="TeamAwayId">Away team of a game.</param>
/// <param name="GameTime">Game, when happens, date and time.</param>
public record GameResponseDto(Guid Id, Guid TeamHomeId, Guid TeamAwayId, DateTime GameTime);
