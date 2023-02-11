namespace BasketballStats.Domain.Entities;

public class Arena
{
    public long Id { get; init; }
    public Guid ArenaId { get; init; } = Guid.NewGuid();
    public string Name { get; init; } = null!;
    public int Capacity { get; init; }
    public string Address { get; init; } = null!;
    public DateOnly Built { get; init; } = DateOnly.MinValue;

    public virtual List<Team> Teams { get; init; } = new List<Team>();
}
