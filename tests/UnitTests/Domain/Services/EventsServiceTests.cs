using AutoFixture;
using AutoFixture.Xunit2;
using BasketballStats.Domain;
using BasketballStats.Domain.Aggregate;
using BasketballStats.Domain.Entities;
using BasketballStats.Domain.Events;
using BasketballStats.Domain.Repositories;
using BasketballStats.Domain.Services;
using Moq;
using Shouldly;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;
using Player = BasketballStats.Domain.Aggregate.Player;

namespace UnitTests.Domain.Services;

public class EventsServiceTests
{
    [Theory, AutoMoqData]
    public async Task GetExistingGameEvents_WithNoMatchingPlayerData_ShouldReturnEmptyArray(
        [Frozen] Mock<IEventStoreRepository> eventStoreRepository,
        [Frozen] Mock<ITypeResolverService> typeResolver,
        Player player,
        Metadata metadata)
    {
        // Arrange
        var streams = new Fixture()
            .Build<Stream>()
            .With(x => x.MetaData, JsonSerializer.Serialize(metadata))
            .CreateMany()
            .ToList();

        var aggregate = new PlayerAggregate(player);

        eventStoreRepository.Setup(x => x.Get(It.IsAny<Guid>())).ReturnsAsync(streams);

        var sut = new EventsService(eventStoreRepository.Object, typeResolver.Object);

        // Act
        var result = await sut.GetExistingGameEvents(aggregate);

        // Assert
        result.ShouldBeEmpty();
    }

    [Theory, AutoMoqData]
    public async Task GetExistingGameEvents_WithExistingPlayerData_ShouldReturnArrayWithEventData(
        [Frozen] Mock<IEventStoreRepository> eventStoreRepository,
        Player player,
        PositiveStatistic statistic,
        Guid eventId)
    {
        // Arrange
        var @event = new PositiveEventHappened(statistic, player.GameId, player.TeamId, player.Id, eventId);

        var streams = new Fixture()
            .Build<Stream>()
            .With(x => x.MetaData, JsonSerializer.Serialize(new Metadata(player.Id, player.TeamId)))
            .With(x => x.StreamId, player.GameId)
            .With(x => x.Type, nameof(PositiveEventHappened))
            .With(x => x.Data, JsonSerializer.Serialize(@event,
                    typeof(PositiveEventHappened),
                    Constants.EnumSerializerOptions))
            .CreateMany()
            .ToList();

        var aggregate = new PlayerAggregate(player);

        eventStoreRepository.Setup(x => x.Get(It.IsAny<Guid>())).ReturnsAsync(streams);

        var sut = new EventsService(eventStoreRepository.Object, new TypeResolverService());

        // Act
        var result = await sut.GetExistingGameEvents(aggregate);

        // Assert
        result.ShouldAllBe(x => x.EventId.Equals(@event.EventId));
        Equals(result.Count, streams.Count);
    }
}
