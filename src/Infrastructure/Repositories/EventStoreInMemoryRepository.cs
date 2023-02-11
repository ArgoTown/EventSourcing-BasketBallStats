using BasketballStats.Domain;
using BasketballStats.Domain.Aggregate;
using BasketballStats.Domain.Repositories;
using BasketballStats.Domain.Services;
using System.Collections.Concurrent;
using System.Text.Json;

namespace BasketballStats.Infrastructure.Repositories;

internal sealed class EventStoreInMemoryRepository : IEventStoreRepository
{
    private static ConcurrentDictionary<Guid, List<Domain.Entities.Stream>> EventStore = new();
    private long _eventStoreId;
    private ITypeResolverService _typeResolverService;

    public EventStoreInMemoryRepository(ITypeResolverService typeResolverService)
    {
        _typeResolverService = typeResolverService;
    }

    public async Task Add(PlayerAggregate playerAggregate)
    {
        var bucket = new List<Domain.Entities.Stream>();

        if (EventStore.ContainsKey(playerAggregate.State.GameId))
        {
            bucket = EventStore[playerAggregate.State.GameId];
        }
        else
        {
            EventStore.TryAdd(playerAggregate.State.GameId, bucket);
        }

        foreach (var @event in playerAggregate.UncommittedEvents)
        {
            var eventType = @event.GetType();

            var entity = new Domain.Entities.Stream
            {
                Id = _eventStoreId++,
                StreamId = playerAggregate.State.GameId,
                EventId = @event.EventId,
                Type = _typeResolverService.GetEventNameByType(eventType),
                Data = JsonSerializer.Serialize(@event, eventType, Constants.EnumSerializerOptions),
                MetaData = JsonSerializer.Serialize(new { playerAggregate.State.TeamId, PlayerId = playerAggregate.State.Id }, Constants.EnumSerializerOptions),
                CreatedAt = DateTime.UtcNow,
                Version = playerAggregate.Version
            };

            bucket.Add(entity);
        }

        await Task.CompletedTask;
    }

    public Task<IReadOnlyList<Domain.Entities.Stream>> Get(Guid gameId)
    {
        return Task.FromResult(
            (IReadOnlyList<Domain.Entities.Stream>)EventStore
            .FirstOrDefault(store => store.Key.Equals(gameId)).Value ?? new List<Domain.Entities.Stream>());
    }

    public Task<IReadOnlyList<Domain.Entities.Stream>> GetAll(int skipEvents)
    {
        return Task.FromResult(
            (IReadOnlyList<Domain.Entities.Stream>)EventStore
            .SelectMany(x => x.Value)
            .Skip(skipEvents)
            .ToList() ?? new List<Domain.Entities.Stream>());
    }
}
