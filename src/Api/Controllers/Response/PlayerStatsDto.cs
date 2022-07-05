namespace BasketballStats.Api.Controllers.Response
{
    /// <summary>
    /// Player total game statistics.
    /// </summary>
    public class PlayerStatsDto
    {
        /// <summary>
        /// Total points.
        /// </summary>
        public short TotalPoints { get; init; }

        /// <summary>
        /// Free throw percentage.
        /// </summary>
        public short FreeThrowPercentage { get; init; }

        /// <summary>
        /// Two points percentage.
        /// </summary>
        public short TwoPointPercentage { get; init; }

        /// <summary>
        /// Three points percentage.
        /// </summary>
        public short ThreePointPercentage { get; init; }
    }
}