using BasketballStats.Domain.Aggregate;
using BasketballStats.Domain.Events;

namespace BasketballStats.Api.Application.Commands;

public class AddPlayerNegativeStatisticCommand : ICommand
{
    public Player Player { get; init; } = null!;
    public NegativeStatistic NegativeStatistic { get; init; } = default!;
}
