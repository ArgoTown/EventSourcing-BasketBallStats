namespace BasketballStats.Api.Controllers.Models
{
    public class PlayerStatsDto
    {
        public short TotalPoints { get; init; }
        public short FreeThrowPercentage { get; init; }
        public short TwoPointPercentage { get; init; }
        public short ThreePointPercentage { get; init; }
    }
}