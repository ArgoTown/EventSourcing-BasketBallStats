namespace BasketballStats.Domain.Events;

public enum NegativeStatistic
{
    FreeThrowMissed = 1,
    TwoPointsMissed,
    ThreePointsMissed,
    FoulMade,
    BlockReceived,
    Turnover
}

public enum PositiveStatistic
{
    FreeThrowMade = 1,
    TwoPointsMade,
    ThreePointsMade,
    FouledAgainst,
    BlockMade,
    DefensiveRebound,
    OffensiveRebound,
    Steal
}
