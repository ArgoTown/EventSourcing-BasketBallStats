using BasketballStats.Api.Application.Commands;
using BasketballStats.Domain.Aggregate;
using BasketballStats.Domain.Repositories;
using BasketballStats.Domain.Services;

namespace BasketballStats.Api.Application.CommandHandlers;

internal class StatisticCommandHandler : ICommandHandler
{
    private readonly IEventStoreRepository _eventStoreRepository;
    private readonly IAggregateStateService _eventsService;

    public StatisticCommandHandler(IEventStoreRepository eventStoreRepository, IAggregateStateService eventsService)
    {
        _eventStoreRepository = eventStoreRepository;
        _eventsService = eventsService;
    }

    public async Task Handle(AddPlayerNegativeStatisticCommand command)
    {
        var aggregate = new PlayerAggregate(command.Player);

        await _eventsService.ApplyCurrentState(aggregate);

        aggregate.AddStatistic(command.NegativeStatistic);
        await _eventStoreRepository.Add(aggregate);
    }

    public async Task Handle(AddPlayerPositiveStatisticCommand command)
    {
        var aggregate = new PlayerAggregate(command.Player);

        await _eventsService.ApplyCurrentState(aggregate);

        aggregate.AddStatistic(command.PositiveStatistic);
        await _eventStoreRepository.Add(aggregate);
    }
}
