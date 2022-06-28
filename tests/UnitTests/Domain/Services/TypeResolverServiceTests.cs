using BasketballStats.Domain.Events;
using BasketballStats.Domain.Services;
using Shouldly;
using System;
using System.Collections.Generic;
using Xunit;

namespace UnitTests.Domain.Services
{
    public class TypeResolverServiceTests
    {
        [Theory,
            InlineData("NegativeEventHappened", typeof(NegativeEventHappened)),
            InlineData("PositiveEventHappened", typeof(PositiveEventHappened))]
        public void GetTypeByEventName_WithProvidedEventName_ShouldReturnExpectedType(string eventName, Type expectedType)
        {
            // Arrange
            var sut = new TypeResolverService();

            // Act
            var result = sut.GetTypeByEventName(eventName);

            // Assert
            result.ShouldBe(expectedType);
        }

        [Theory,
            InlineData(typeof(NegativeEventHappened), "NegativeEventHappened"),
            InlineData(typeof(PositiveEventHappened), "PositiveEventHappened")]
        public void GetEventNameByType_WithProvidedEventType_ShouldReturnExpectedEventName(Type eventType, string expectedName)
        {
            // Arrange
            var sut = new TypeResolverService();

            // Act
            var result = sut.GetEventNameByType(eventType);

            // Assert
            result.ShouldBe(expectedName);
        }

        [Theory, InlineData("")]
        public void GetTypeByEventName_WithProvidedEmptyEventName_ShouldThrowKeyNotFoundException(string eventName)
        {
            // Arrange
            var sut = new TypeResolverService();

            // Act & Assert
            Should.Throw<KeyNotFoundException>(() => sut.GetTypeByEventName(eventName));
        }

        [Fact]
        public void GetTypeByEventName_WithProvidedNullEventName_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            var sut = new TypeResolverService();

            // Act & Assert
            Should.Throw<ArgumentNullException>(() => sut.GetTypeByEventName(null));
        }
    }
}
