using BasketballStats.Domain.Aggregate;
using BasketballStats.Domain.Events;

namespace BasketballStats.Domain.Services;

public interface IAggregateStateService
{
    Task ApplyCurrentState(PlayerAggregate aggregate);
    Task ApplyCurrentState(PlayerAggregate aggregate, PositiveStatistic statistic);
    Task ApplyCurrentState(PlayerAggregate aggregate, NegativeStatistic statistic);
}
