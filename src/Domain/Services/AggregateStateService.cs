using BasketballStats.Domain.Aggregate;
using BasketballStats.Domain.Entities;
using BasketballStats.Domain.Events;
using BasketballStats.Domain.Repositories;
using System.Collections.Immutable;
using System.Text.Json;

namespace BasketballStats.Domain.Services;

internal sealed class AggregateStateService : IAggregateStateService
{
    private readonly IEventStoreRepository _eventStoreRepository;
    private readonly ITypeResolverService _typeResolver;

    public AggregateStateService(IEventStoreRepository eventStoreRepository, ITypeResolverService typeResolver)
    {
        _eventStoreRepository = eventStoreRepository;
        _typeResolver = typeResolver;
    }

    public async Task ApplyCurrentState(PlayerAggregate aggregate)
    {
        await ApplyState(aggregate);
    }

    public async Task ApplyCurrentState(PlayerAggregate aggregate, PositiveStatistic statistic)
    {
        await ApplyState(aggregate);
        aggregate.AddStatistic(statistic);
    }

    public async Task ApplyCurrentState(PlayerAggregate aggregate, NegativeStatistic statistic)
    {
        await ApplyState(aggregate);
        aggregate.AddStatistic(statistic);
    }

    private async Task ApplyState(PlayerAggregate aggregate)
    {
        var streamEvents = await _eventStoreRepository.Get(aggregate.State.GameId);
        var playerStreams = streamEvents
            .Where(@event =>
            {
                var metadata = JsonSerializer.Deserialize<Metadata>(@event.MetaData)!;
                return metadata.TeamId.Equals(aggregate.State.TeamId) && metadata.PlayerId.Equals(aggregate.State.Id);
            })
            .ToList();

        if (playerStreams.Any())
        {
            aggregate.ApplyEvents(
             playerStreams
                .Select(stream => (IEvent)JsonSerializer.Deserialize(
                    stream.Data,
                    _typeResolver.GetTypeByEventName(stream.Type),
                    Constants.EnumSerializerOptions)!)
                .ToImmutableList());
        }
    }
}
