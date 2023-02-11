using BasketballStats.Domain;
using BasketballStats.Domain.Aggregate;
using BasketballStats.Domain.Repositories;
using BasketballStats.Domain.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace BasketballStats.Infrastructure.Repositories;

internal sealed class EventStoreRepository : IEventStoreRepository
{
    private readonly EventsContext _context;
    private readonly ITypeResolverService _typeResolverService;

    public EventStoreRepository(EventsContext context, ITypeResolverService typeResolverService)
    {
        _context = context;
        _typeResolverService = typeResolverService;
    }

    public async Task Add(PlayerAggregate playerAggregate)
    {
        foreach (var evt in playerAggregate.UncommittedEvents)
        {
            var eventType = evt.GetType();

            var entity = new Domain.Entities.Stream
            {
                StreamId = playerAggregate.State.GameId,
                EventId = evt.EventId,
                Type = _typeResolverService.GetEventNameByType(eventType),
                Data = JsonSerializer.Serialize(evt, eventType, Constants.EnumSerializerOptions),
                MetaData = JsonSerializer.Serialize(new { playerAggregate.State.TeamId, PlayerId = playerAggregate.State.Id }, Constants.EnumSerializerOptions),
                CreatedAt = DateTime.UtcNow,
                Version = playerAggregate.Version
            };

            _context.Streams.Add(entity);
        }

        await _context.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<Domain.Entities.Stream>> Get(Guid gameId)
    {
        var response = await _context.Streams.Where(store => store.StreamId.Equals(gameId)).ToListAsync() ?? new List<Domain.Entities.Stream>();
        return response;
    }

    public async Task<IReadOnlyList<Domain.Entities.Stream>> GetAll(int skipEvents)
    {
        var response = await _context.Streams.Skip(skipEvents).ToListAsync();
        return response;
    }
}
