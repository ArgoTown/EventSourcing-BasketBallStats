using BasketballStats.Domain.Aggregate;

namespace BasketballStats.Domain.Services;

public interface IAggregateStateService
{
    Task ApplyCurrentState(PlayerAggregate aggregate);
}
