using AutoFixture.Xunit2;
using BasketballStats.Api.Application.Queries;
using BasketballStats.Api.Application.QueryHandlers;
using BasketballStats.Domain.Aggregate;
using BasketballStats.Domain.Events;
using BasketballStats.Domain.Services;
using Moq;
using Shouldly;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests.Api.Application.QueryHandler
{
    public class PlayerStatisticsQueryHandlerTests
    {
        [Theory, AutoMoqData]
        public async Task Do(
            [Frozen] Mock<IEventsService> eventsService, 
            PlayerStatisticsQueryHandler sut, 
            Player player, 
            PositiveEventHappened @event)
        {
            // Arrange
            eventsService.Setup(x => x.GetExistingGameEvents(It.IsAny<PlayerAggregate>())).ReturnsAsync(new List<IEvent> { @event });

            var aggregate = new PlayerAggregate(new Player(player.GameId, player.TeamId, player.Id));
            aggregate.ApplyEvent(@event);
            var aggregateResult = aggregate.TotalStats();

            // Act
            var result = await sut.Query(new GetPlayerStatisticsQuery(player));

            // Assert
            result.TwoPointPercentage.ShouldBe(aggregateResult.TwoPointPercentage);
            result.FreeThrowPercentage.ShouldBe(aggregateResult.FreeThrowPercentage);
            result.ThreePointPercentage.ShouldBe(aggregateResult.ThreePointPercentage);
            result.TotalPoints.ShouldBe(aggregateResult.TotalPoints);
        }
    }
}
