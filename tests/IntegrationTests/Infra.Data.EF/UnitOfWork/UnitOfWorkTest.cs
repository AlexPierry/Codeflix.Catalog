using Application;
using Domain.SeedWork;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using UnitOfWorkInfra = Infra.Data.EF;

namespace IntegrationTest.Infra.Data.EF.UnitOfWork;

[Collection(nameof(UnitOfWorkTestFixture))]
public class UnitOfWorkTest
{
    private readonly UnitOfWorkTestFixture _fixture;

    public UnitOfWorkTest(UnitOfWorkTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(CommitOk))]
    [Trait("Integration/Infra.Data", "UnitOfWork - Persistence")]
    public async void CommitOk()
    {
        // Given
        var dbId = Guid.NewGuid().ToString();
        var dbContext = _fixture.CreateDbContext(false, dbId);
        var exampleCategoriesList = _fixture.GetExampleCategoriesList();
        var categoryWithEvent = exampleCategoriesList.First();
        var @event = new DomainEventFake();
        categoryWithEvent.RaiseEvent(@event);
        var eventHandlerMock = new Mock<IDomainEventHandler<DomainEventFake>>();
        await dbContext.AddRangeAsync(exampleCategoriesList);
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging();
        serviceCollection.AddSingleton(eventHandlerMock.Object);
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var eventPublisher = new DomainEventPublisher(serviceProvider);
        var unitOfWork = new UnitOfWorkInfra.UnitOfWork(
            dbContext,
            eventPublisher,
            serviceProvider.GetRequiredService<ILogger<UnitOfWorkInfra.UnitOfWork>>()
        );

        // When
        await unitOfWork.Commit(CancellationToken.None);

        // Then
        var assertDbContext = _fixture.CreateDbContext(true, dbId);
        var savedCategories = assertDbContext.Categories.AsNoTracking().ToList();

        savedCategories.Should().HaveCount(exampleCategoriesList.Count);
        eventHandlerMock.Verify(x => x.HandleAsync(@event, It.IsAny<CancellationToken>()), Times.Once);
        categoryWithEvent.DomainEvents.Should().BeEmpty();
    }

    [Fact(DisplayName = nameof(RollbackOk))]
    [Trait("Integration/Infra.Data", "UnitOfWork - Persistence")]
    public async void RollbackOk()
    {
        // Given
        var dbId = Guid.NewGuid().ToString();
        var dbContext = _fixture.CreateDbContext(false, dbId);
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging();
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var eventPublisher = new DomainEventPublisher(serviceProvider);
        var unitOfWork = new UnitOfWorkInfra.UnitOfWork(
            dbContext,
            eventPublisher,
            serviceProvider.GetRequiredService<ILogger<UnitOfWorkInfra.UnitOfWork>>()
        );

        // When
        var task = async () => await unitOfWork.Rollback(CancellationToken.None);

        // Then
        await task.Should().NotThrowAsync();
    }
}