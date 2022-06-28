using BasketballStats.Api.Application.Commands;

namespace BasketballStats.Api.Application.CommandHandlers
{
    public interface ICommandHandler
    {
        Task Handle(AddPlayerNegativeStatisticCommand command);
        Task Handle(AddPlayerPositiveStatisticCommand command);
    }
}
