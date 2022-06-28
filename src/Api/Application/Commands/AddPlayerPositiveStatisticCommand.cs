using BasketballStats.Domain.Aggregate;
using BasketballStats.Domain.Events;

namespace BasketballStats.Api.Application.Commands
{
    public class AddPlayerPositiveStatisticCommand : ICommand
    {
        public Player Player { get; init; } = null!;
        public PositiveStatistic PositiveStatistic { get; init; } = default!;
    }
}
