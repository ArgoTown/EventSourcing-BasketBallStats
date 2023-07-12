using BasketballStats.Domain.Aggregate.Base;

namespace BasketballStats.Domain.Aggregate;

public record Game : IBaseDomainEntity
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid TeamHomeId { get; init; }
    public Guid TeamAwayId { get; init; }
    public DateTime GameTime { get; init; }

    public Game(Guid? teamHome, Guid? teamAway, DateTime gameTime)
    {
        TeamHomeId = teamHome ?? Guid.NewGuid();
        TeamAwayId = teamAway ?? Guid.NewGuid();
        GameTime = gameTime;
    }
}
