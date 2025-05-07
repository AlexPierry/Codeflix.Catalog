using System;
using FluentAssertions;

namespace UnitTests.Domain.SeedWork;

public class AggregateRootTest
{
    [Fact(DisplayName = nameof(RaiseEvent))]
    [Trait("Domain", "AggregateRoot - Unit Tests")]
    public void RaiseEvent()
    {
        // Arrange
        var aggregateRoot = new AggregateRootFake();
        var domainEvent = new DomainEventFake();

        // Act
        aggregateRoot.RaiseEvent(domainEvent);

        // Assert        
        aggregateRoot.DomainEvents.Should().HaveCount(1);
    }

    [Fact(DisplayName = nameof(ClearDomainEvents))]
    [Trait("Domain", "AggregateRoot - Unit Tests")]
    public void ClearDomainEvents()
    {
        // Arrange
        var aggregateRoot = new AggregateRootFake();
        var domainEvent = new DomainEventFake();
        aggregateRoot.RaiseEvent(domainEvent);

        // Act
        aggregateRoot.ClearDomainEvents();

        // Assert        
        aggregateRoot.DomainEvents.Should().BeEmpty();
    }
}
