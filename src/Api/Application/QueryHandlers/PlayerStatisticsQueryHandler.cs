using BasketballStats.Api.Application.Queries;
using BasketballStats.Domain.Aggregate;
using BasketballStats.Domain.Models;
using BasketballStats.Domain.Services;

namespace BasketballStats.Api.Application.QueryHandlers
{
    public class PlayerStatisticsQueryHandler : IQueryHandler<GetPlayerStatisticsQuery, PlayerStats>
    {
        private readonly IEventsService _eventsService;

        public PlayerStatisticsQueryHandler(IEventsService eventsService)
        {
            _eventsService = eventsService;
        }

        public async Task<PlayerStats> Query(GetPlayerStatisticsQuery query)
        {
            var aggregate = new PlayerAggregate(query.Player);
            var events = await _eventsService.GetExistingGameEvents(aggregate);

            aggregate.ApplyEvents(events);

            return aggregate.TotalStats();
        }
    }
}
