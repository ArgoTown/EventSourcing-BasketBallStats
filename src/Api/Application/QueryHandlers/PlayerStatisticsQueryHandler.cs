using BasketballStats.Api.Application.Queries;
using BasketballStats.Domain.Aggregate;
using BasketballStats.Domain.Models;
using BasketballStats.Domain.Services;

namespace BasketballStats.Api.Application.QueryHandlers;

internal class PlayerStatisticsQueryHandler : IQueryHandler<GetPlayerStatisticsQuery, PlayerStats>
{
    private readonly IAggregateStateService _eventsService;

    public PlayerStatisticsQueryHandler(IAggregateStateService eventsService)
    {
        _eventsService = eventsService;
    }

    public async Task<PlayerStats> Query(GetPlayerStatisticsQuery query)
    {
        var aggregate = new PlayerAggregate(query.Player);
        await _eventsService.ApplyCurrentState(aggregate);
        return aggregate.TotalStats();
    }
}
