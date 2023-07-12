using AutoFixture.Xunit2;
using BasketballStats.Api.Application.CommandHandlers;
using BasketballStats.Api.Application.Commands;
using BasketballStats.Domain.Aggregate;
using BasketballStats.Domain.Events;
using BasketballStats.Domain.Repositories;
using BasketballStats.Domain.Services;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests.Api.Application.CommandHandler;

public class StatisticCommandHandlerTests
{
    [Theory, AutoMoqData]
    public async Task Handle_PositiveStatistic_ShouldBeSaved(
        [Frozen] Mock<IEventStoreRepository> eventStoreRepository,
        [Frozen] Mock<IAggregateStateService> eventsService,
        AddPlayerPositiveStatisticCommand command
    )
    { // Arrange
        var aggregate = new PlayerAggregate(command.Player);

        var sut = new StatisticCommandHandler(eventStoreRepository.Object, eventsService.Object);

        eventStoreRepository.Setup(x => x.Add(It.IsAny<PlayerAggregate>()));

        eventsService.Setup(x => x.ApplyCurrentState(It.IsAny<PlayerAggregate>(), It.IsAny<PositiveStatistic>()));

        // Act
        await sut.Handle(command);

        // Assert
        eventStoreRepository.Verify(x => x.Add(It.IsAny<PlayerAggregate>()), Times.Once);
        eventsService.Verify(x => x.ApplyCurrentState(It.IsAny<PlayerAggregate>(), It.IsAny<PositiveStatistic>()), Times.Once);
    }

    [Theory, AutoMoqData]
    public async Task Handle_NegativeStatistic_ShouldBeSaved(
        [Frozen] Mock<IEventStoreRepository> eventStoreRepository,
        [Frozen] Mock<IAggregateStateService> eventsService,
        AddPlayerNegativeStatisticCommand command
    )
    {
        // Arrange
        var aggregate = new PlayerAggregate(command.Player);

        var sut = new StatisticCommandHandler(eventStoreRepository.Object, eventsService.Object);

        eventStoreRepository.Setup(x => x.Add(It.IsAny<PlayerAggregate>()));

        eventsService.Setup(x => x.ApplyCurrentState(It.IsAny<PlayerAggregate>(), It.IsAny<NegativeStatistic>()));

        // Act
        await sut.Handle(command);

        // Assert
        eventStoreRepository.Verify(x => x.Add(It.IsAny<PlayerAggregate>()), Times.Once);
        eventsService.Verify(x => x.ApplyCurrentState(It.IsAny<PlayerAggregate>(), It.IsAny<NegativeStatistic>()), Times.Once);
    }
}
