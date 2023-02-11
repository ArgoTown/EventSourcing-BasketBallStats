namespace BasketballStats.Domain.Entities;

public class Team
{
    public long Id { get; init; }
    public Guid TeamId { get; init; } = Guid.NewGuid();
    public string Name { get; init; } = null!;
    public DateOnly Founded { get; init; } = DateOnly.MinValue;
    public bool IsActive { get; init; } = true;

    public virtual List<Arena> Arenas { get; init; } = new List<Arena>();
    public virtual List<Player> Players { get; init; } = new List<Player>();
    public virtual Game? Game { get; init; }
}
