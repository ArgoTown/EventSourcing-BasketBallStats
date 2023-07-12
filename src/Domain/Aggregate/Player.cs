using BasketballStats.Domain.Aggregate.Base;
using BasketballStats.Domain.Events;

namespace BasketballStats.Domain.Aggregate;

public class Player : IBaseDomainEntity, IBaseAggregateRoot
{
    public Guid Id { get; }
    public Guid GameId { get; }
    public Guid TeamId { get; }
    public short MadeFreeThrows { get; private set; }
    public short MissedFreeThrows { get; private set; }
    public short MadeTwoPoints { get; private set; }
    public short MissedTwoPoints { get; private set; }
    public short MadeThreePoints { get; private set; }
    public short MissedThreePoints { get; private set; }
    public short DefensiveRebounds { get; private set; }
    public short OffensiveRebounds { get; private set; }
    public short Steals { get; private set; }
    public short Turnovers { get; private set; }
    public short Blocks { get; private set; }
    public short BlocksReceived { get; private set; }
    public short Fouls { get; private set; }
    public short FoulsProvoked { get; private set; }

    public Player(Guid gameId, Guid teamId, Guid playerId)
    {
        GameId = gameId;
        TeamId = teamId;
        Id = playerId;
    }

    public void Apply(PositiveEventHappened evt)
    {
        switch (evt.Statistic)
        {
            case PositiveStatistic.FreeThrowMade:
                MadeFreeThrows++;
                break;
            case PositiveStatistic.TwoPointsMade:
                MadeTwoPoints++;
                break;
            case PositiveStatistic.ThreePointsMade:
                MadeThreePoints++;
                break;
            case PositiveStatistic.DefensiveRebound:
                DefensiveRebounds++;
                break;
            case PositiveStatistic.OffensiveRebound:
                OffensiveRebounds++;
                break;
            case PositiveStatistic.Steal:
                Steals++;
                break;
            case PositiveStatistic.BlockMade:
                Blocks++;
                break;
            case PositiveStatistic.FouledAgainst:
                FoulsProvoked++;
                break;
            default:
                throw new ArgumentException("Action type not supported.", nameof(PositiveStatistic));
        }
    }

    public void Apply(NegativeEventHappened evt)
    {
        switch (evt.Statistic)
        {
            case NegativeStatistic.FreeThrowMissed:
                MissedFreeThrows++;
                break;
            case NegativeStatistic.TwoPointsMissed:
                MissedTwoPoints++;
                break;
            case NegativeStatistic.ThreePointsMissed:
                MissedThreePoints++;
                break;
            case NegativeStatistic.Turnover:
                Turnovers++;
                break;
            case NegativeStatistic.BlockReceived:
                BlocksReceived++;
                break;
            case NegativeStatistic.FoulMade:
                Fouls++;
                break;
            default:
                throw new ArgumentException("Action type not supported.", nameof(NegativeStatistic));
        }
    }
}
