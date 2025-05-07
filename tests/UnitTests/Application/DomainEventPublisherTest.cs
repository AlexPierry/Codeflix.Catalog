
using Application;
using Domain.SeedWork;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace UnitTests.Application;

public class DomainEventPublisherTest
{
    [Fact(DisplayName = nameof(PublishAsync))]
    [Trait("Application", "DomainEventPublisher - Unit Tests")]
    public async Task PublishAsync()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        var eventHandlerMock1 = new Mock<IDomainEventHandler<DomainEventToBeHandled>>();
        var eventHandlerMock2 = new Mock<IDomainEventHandler<DomainEventToBeHandled>>();
        var eventHandlerMock3 = new Mock<IDomainEventHandler<DomainEventToNotBeHandled>>();
        serviceCollection.AddSingleton(eventHandlerMock1.Object);
        serviceCollection.AddSingleton(eventHandlerMock2.Object);
        serviceCollection.AddSingleton(eventHandlerMock3.Object);
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var domainEventPublisher = new DomainEventPublisher(serviceProvider);
        DomainEvent @event = new DomainEventToBeHandled();

        // Act
        // (dynamic) hack para forçar o compilador a enxergar o tipo correto
        await domainEventPublisher.PublishAsync((dynamic)@event, CancellationToken.None);

        // Assert
        eventHandlerMock1.Verify(x => x.HandleAsync((DomainEventToBeHandled)@event, It.IsAny<CancellationToken>()), Times.Once);
        eventHandlerMock2.Verify(x => x.HandleAsync((DomainEventToBeHandled)@event, It.IsAny<CancellationToken>()), Times.Once);
        eventHandlerMock3.Verify(x => x.HandleAsync(
            It.IsAny<DomainEventToNotBeHandled>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact(DisplayName = nameof(NoActionWhenThereIsNoSubscriber))]
    [Trait("Application", "DomainEventPublisher - Unit Tests")]
    public async Task NoActionWhenThereIsNoSubscriber()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        var eventHandlerMock1 = new Mock<IDomainEventHandler<DomainEventToNotBeHandled>>();
        var eventHandlerMock2 = new Mock<IDomainEventHandler<DomainEventToNotBeHandled>>();
        serviceCollection.AddSingleton(eventHandlerMock1.Object);
        serviceCollection.AddSingleton(eventHandlerMock2.Object);
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var domainEventPublisher = new DomainEventPublisher(serviceProvider);
        var @event = new DomainEventToBeHandled();

        // Act
        await domainEventPublisher.PublishAsync(@event, CancellationToken.None);

        // Assert
        eventHandlerMock1.Verify(x => x.HandleAsync(
            It.IsAny<DomainEventToNotBeHandled>(),
            It.IsAny<CancellationToken>()), Times.Never);

        eventHandlerMock2.Verify(x => x.HandleAsync(
            It.IsAny<DomainEventToNotBeHandled>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

}
