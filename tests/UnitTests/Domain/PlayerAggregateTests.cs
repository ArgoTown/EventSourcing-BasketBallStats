using BasketballStats.Domain.Aggregate;
using Shouldly;
using System;
using Xunit;

namespace UnitTests.Domain;

public class PlayerAggregateTests
{
    [Fact]
    public void State_WithNullValue_ShouldThrowException()
    {
        // Arrange & Act & Assert
        Should.Throw<ArgumentNullException>(() => new PlayerAggregate(null));
    }

    [Theory, AutoMoqData]
    public void State_WithFakePlayer_ShouldCreateAggregate(Player player)
    {
        // Arrange & Act
        var aggregate = new PlayerAggregate(player);

        // Assert
        aggregate.State.TeamId.ShouldBe(player.TeamId);
        aggregate.State.GameId.ShouldBe(player.GameId);
        aggregate.State.Id.ShouldBe(player.Id);
    }
}
