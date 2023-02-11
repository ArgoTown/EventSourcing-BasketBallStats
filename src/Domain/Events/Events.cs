namespace BasketballStats.Domain.Events;

public interface IEvent
{
    Guid EventId { get; init; }
}

public record NegativeEventHappened(NegativeStatistic Statistic, Guid StreamId, Guid TeamId, Guid PlayerId, Guid EventId) : IEvent;
public record PositiveEventHappened(PositiveStatistic Statistic, Guid StreamId, Guid TeamId, Guid PlayerId, Guid EventId) : IEvent;
