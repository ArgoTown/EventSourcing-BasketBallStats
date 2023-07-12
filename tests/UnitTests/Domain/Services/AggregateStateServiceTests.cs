using AutoFixture;
using AutoFixture.Xunit2;
using BasketballStats.Domain;
using BasketballStats.Domain.Aggregate;
using BasketballStats.Domain.Entities;
using BasketballStats.Domain.Events;
using BasketballStats.Domain.Repositories;
using BasketballStats.Domain.Services;
using Moq;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;
using Player = BasketballStats.Domain.Aggregate.Player;

namespace UnitTests.Domain.Services;

public class AggregateStateServiceTests
{
    [Theory, AutoMoqData]
    public async Task ApplyCurrentState_WithStreamsContainingPositiveEvent_ShouldAllEventsBeApplied(
        [Frozen] Mock<IEventStoreRepository> eventStoreRepository,
        [Frozen] Mock<ITypeResolverService> typeResolver,
        Player player)
    {
        // Arrange
        var fixture = new Fixture();

        var @event = fixture
            .Build<PositiveEventHappened>()
            .With(x => x.Statistic, PositiveStatistic.FreeThrowMade)
            .With(x => x.PlayerId, player.Id)
            .With(x => x.TeamId, player.TeamId)
            .Create();

        var eventType = @event.GetType();

        var streams = fixture
            .Build<Stream>()
            .With(x => x.MetaData, JsonSerializer.Serialize(new Metadata(player.Id, player.TeamId)))
            .With(x => x.Type, eventType.Name)
            .With(x => x.Data, JsonSerializer.Serialize(@event, eventType, Constants.EnumSerializerOptions))
            .With(x => x.StreamId, @event.StreamId)
            .With(x => x.EventId, @event.EventId)
            .CreateMany()
            .ToList();

        var aggregate = new PlayerAggregate(player);

        eventStoreRepository.Setup(x => x.Get(It.IsAny<Guid>())).ReturnsAsync(streams);
        typeResolver.Setup(x => x.GetTypeByEventName(It.IsAny<string>())).Returns(eventType);

        var sut = new AggregateStateService(eventStoreRepository.Object, typeResolver.Object);

        // Act
        await sut.ApplyCurrentState(aggregate);

        // Assert
        typeResolver.Verify(x => x.GetTypeByEventName(It.IsAny<string>()), Times.Exactly(streams.Count));
    }
}
