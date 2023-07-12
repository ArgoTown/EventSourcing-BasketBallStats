using AutoFixture.Xunit2;
using BasketballStats.Api.Application.Queries;
using BasketballStats.Api.Application.QueryHandlers;
using BasketballStats.Domain.Aggregate;
using BasketballStats.Domain.Services;
using Moq;
using Shouldly;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests.Api.Application.QueryHandler;

public class PlayerStatisticsQueryHandlerTests
{
    [Theory, AutoMoqData]
    public async Task Query_PlayerStatisticsAndApplyToAggregate_ShouldStatisticsMatch(
        [Frozen] Mock<IAggregateStateService> eventsService, 
        Player player)
    {
        // Arrange
        var sut = new PlayerStatisticsQueryHandler(eventsService.Object);

        eventsService.Setup(x => x.ApplyCurrentState(It.IsAny<PlayerAggregate>()));

        var aggregate = new PlayerAggregate(new Player(player.GameId, player.TeamId, player.Id));

        // Act
        var result = await sut.Query(new GetPlayerStatisticsQuery(player));

        // Assert
        eventsService.Verify(x => x.ApplyCurrentState(It.IsAny<PlayerAggregate>()), Times.Once);

        var aggregateStats = aggregate.TotalStats();

        result.TwoPointPercentage.ShouldBe(aggregateStats.TwoPointPercentage);
        result.FreeThrowPercentage.ShouldBe(aggregateStats.FreeThrowPercentage);
        result.ThreePointPercentage.ShouldBe(aggregateStats.ThreePointPercentage);
        result.TotalPoints.ShouldBe(aggregateStats.TotalPoints);
    }
}
