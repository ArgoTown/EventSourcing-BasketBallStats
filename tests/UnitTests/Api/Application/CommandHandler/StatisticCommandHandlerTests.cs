using AutoFixture.Xunit2;
using BasketballStats.Api.Application.CommandHandlers;
using BasketballStats.Api.Application.Commands;
using BasketballStats.Domain.Aggregate;
using BasketballStats.Domain.Events;
using BasketballStats.Domain.Repositories;
using BasketballStats.Domain.Services;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests.Api.Application.CommandHandler;

public class StatisticCommandHandlerTests
{
    [Theory, AutoMoqData]
    public async Task Handle_PositiveStatistic_ShouldBeSaved(
        [Frozen] Mock<IEventStoreRepository> eventStoreRepository,
        [Frozen] Mock<IEventsService> eventsService,
        StatisticCommandHandler sut,
        AddPlayerPositiveStatisticCommand command,
        PositiveEventHappened positiveEvent
    )
    {
        // Arrange
        eventStoreRepository.Setup(x => x.Add(It.IsAny<PlayerAggregate>()));

        eventsService
            .Setup(x => x.GetExistingGameEvents(It.IsAny<PlayerAggregate>()))
            .ReturnsAsync(new List<IEvent> { positiveEvent });

        // Act
        await sut.Handle(command);

        // Assert
        eventStoreRepository.Verify(x => x.Add(It.IsAny<PlayerAggregate>()), Times.Once);
        eventsService.Verify(x => x.GetExistingGameEvents(It.IsAny<PlayerAggregate>()), Times.Once);
    }

    [Theory, AutoMoqData]
    public async Task Handle_NegativeStatistic_ShouldBeSaved(
        [Frozen] Mock<IEventStoreRepository> eventStoreRepository,
        [Frozen] Mock<IEventsService> eventsService,
        StatisticCommandHandler sut,
        AddPlayerNegativeStatisticCommand command,
        NegativeEventHappened negativeEvent
    )
    {
        // Arrange
        eventStoreRepository
            .Setup(x => x.Add(It.IsAny<PlayerAggregate>()));

        eventsService
            .Setup(x => x.GetExistingGameEvents(It.IsAny<PlayerAggregate>()))
            .ReturnsAsync(new List<IEvent> { negativeEvent });

        // Act
        await sut.Handle(command);

        // Assert
        eventStoreRepository.Verify(x => x.Add(It.IsAny<PlayerAggregate>()), Times.Once);
        eventsService.Verify(x => x.GetExistingGameEvents(It.IsAny<PlayerAggregate>()), Times.Once);
    }
}
