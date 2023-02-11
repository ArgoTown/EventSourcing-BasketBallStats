namespace BasketballStats.Domain.Entities;

public class GameStatsReadModel
{
    public long Id { get; set; }
    public Guid GameId { get; set; }
    public Guid TeamId { get; set; }
    public Guid PlayerId { get; set; }
    public short MadeFreeThrows { get; set; }
    public short MissedFreeThrows { get; set; }
    public short MadeTwoPoints { get; set; }
    public short MissedTwoPoints { get; set; }
    public short MadeThreePoints { get; set; }
    public short MissedThreePoints { get; set; }
    public short DefensiveRebounds { get; set; }
    public short OffensiveRebounds { get; set; }
    public short Steals { get; set; }
    public short Turnovers { get; set; }
    public short Blocks { get; set; }
    public short BlocksReceived { get; set; }
    public short Fouls { get; set; }
    public short FoulsProvoked { get; set; }
}
