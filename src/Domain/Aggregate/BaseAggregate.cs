using BasketballStats.Domain.Aggregate.Base;
using BasketballStats.Domain.Events;

namespace BasketballStats.Domain.Aggregate;

public abstract class BaseAggregate<TAggregate> where TAggregate : IBaseAggregateRoot
{
    private readonly List<IEvent> _events = new();
    public IReadOnlyList<IEvent> UncommittedEvents => _events;
    public TAggregate State { get; private set; }
    public int Version { get; private set; }

    protected BaseAggregate(TAggregate state)
    {
        State = state ?? throw new ArgumentNullException("Aggregate state must be provided.");
    }

    protected void Raise(IEvent @event)
    {
        Version++;
        _events.Add(@event);
    }

    public void ApplyEvent(IEvent @event)
    {
        When(@event);
        Version++;
    }

    public void ApplyEvents(IReadOnlyCollection<IEvent> events)
    {
        foreach (var @event in events)
        {
            ApplyEvent(@event);
        }
    }

    protected abstract void When(IEvent @event);
}
