using BasketballStats.Domain.Aggregate;
using BasketballStats.Domain.Events;

namespace BasketballStats.Domain.Services;

public interface IEventsService
{
    Task<IReadOnlyCollection<IEvent>> GetExistingGameEvents(PlayerAggregate aggregate);
}
