using BasketballStats.Domain.Aggregate;

namespace BasketballStats.Api.Application.Commands
{
    public interface ICommand
    {
        Player Player { get; init; }
    }
}
