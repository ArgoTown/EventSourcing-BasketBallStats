using BasketballStats.Api.Application.Commands;
using BasketballStats.Domain.Aggregate;
using BasketballStats.Domain.Repositories;
using BasketballStats.Domain.Services;

namespace BasketballStats.Api.Application.CommandHandlers;

internal class StatisticCommandHandler : ICommandHandler
{
    private readonly IEventStoreRepository _eventStoreRepository;
    private readonly IEventsService _eventsService;

    public StatisticCommandHandler(IEventStoreRepository eventStoreRepository, IEventsService eventsService)
    {
        _eventStoreRepository = eventStoreRepository;
        _eventsService = eventsService;
    }

    public async Task Handle(AddPlayerNegativeStatisticCommand command)
    {
        var aggregate = new PlayerAggregate(command.Player);
        await ApplyAnyExistingEvents(aggregate);

        aggregate.AddStatistic(command.NegativeStatistic);
        await PersistHappenedEvents(aggregate);
    }

    public async Task Handle(AddPlayerPositiveStatisticCommand command)
    {
        var aggregate = new PlayerAggregate(command.Player);
        await ApplyAnyExistingEvents(aggregate);

        aggregate.AddStatistic(command.PositiveStatistic);
        await PersistHappenedEvents(aggregate);
    }

    private async Task ApplyAnyExistingEvents(PlayerAggregate aggregate)
    {
        var events = await _eventsService.GetExistingGameEvents(aggregate);
        aggregate.ApplyEvents(events);
    }

    private async Task PersistHappenedEvents(PlayerAggregate aggregate)
    {
        await _eventStoreRepository.Add(aggregate);
    }
}
