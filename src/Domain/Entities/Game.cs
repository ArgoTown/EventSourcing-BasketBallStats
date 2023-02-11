namespace BasketballStats.Domain.Entities;

public class Game
{
    public long Id { get; init; }
    public Guid GameId { get; init; }
    public Guid TeamHomeId { get; init; }
    public Guid TeamAwayId { get; init; }
    public DateTime GameTime { get; init; }

    public virtual List<Team> Teams { get; init; } = default!;
}
