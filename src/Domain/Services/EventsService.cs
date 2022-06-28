using BasketballStats.Domain.Aggregate;
using BasketballStats.Domain.Entities;
using BasketballStats.Domain.Events;
using BasketballStats.Domain.Repositories;
using System.Collections.Immutable;
using System.Text.Json;

namespace BasketballStats.Domain.Services
{
    internal sealed class EventsService : IEventsService
    {
        private readonly IEventStoreRepository _eventStoreRepository;
        private readonly ITypeResolverService _typeResolver;

        public EventsService(IEventStoreRepository eventStoreRepository, ITypeResolverService typeResolver)
        {
            _eventStoreRepository = eventStoreRepository;
            _typeResolver = typeResolver;
        }

        public async Task<IReadOnlyCollection<IEvent>> GetExistingGameEvents(PlayerAggregate aggregate)
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
                return playerStreams
                    .Select(stream => (IEvent)JsonSerializer.Deserialize(
                        stream.Data,
                        _typeResolver.GetTypeByEventName(stream.Type),
                        Constants.EnumSerializerOptions)!)
                    .ToImmutableList();
            }

            return Array.Empty<IEvent>();
        }
    }
}
