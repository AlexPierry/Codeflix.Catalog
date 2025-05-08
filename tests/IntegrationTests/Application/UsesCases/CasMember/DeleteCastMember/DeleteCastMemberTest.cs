using Application;
using Application.Exceptions;
using FluentAssertions;
using Infra.Data.EF;
using Infra.Data.EF.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using UseCase = Application.UseCases.CastMember;

namespace IntegrationTest.Application.UseCases.CastMember;

[Collection(nameof(DeleteCastMemberTestFixture))]
public class DeleteCastMemberTest
{
    private readonly DeleteCastMemberTestFixture _fixture;

    public DeleteCastMemberTest(DeleteCastMemberTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(DeleteCastMemberOk))]
    [Trait("Integration/Application", "DeleteCastMember - Use Cases")]
    public async void DeleteCastMemberOk()
    {
        // Given
        var dbContext = _fixture.CreateDbContext();
        var exampleList = _fixture.GetExampleCastMembersList(10);
        var exampleCastMember = exampleList[3];
        await dbContext.AddRangeAsync(exampleList);
        await dbContext.SaveChangesAsync();

        var actDbContext = _fixture.CreateDbContext(true);
        var repository = new CastMemberRepository(actDbContext);

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging();
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var eventPublisher = new DomainEventPublisher(serviceProvider);
        var unitOfWork = new UnitOfWork(
            actDbContext,
            eventPublisher,
            serviceProvider.GetRequiredService<ILogger<UnitOfWork>>()
        );

        var input = new UseCase.DeleteCastMemberInput(exampleCastMember.Id);
        var useCase = new UseCase.DeleteCastMember(repository, unitOfWork);

        // When
        await useCase.Handle(input, CancellationToken.None);

        // Then
        var dbContextToTest = _fixture.CreateDbContext(true);
        var deletedCastMember = await dbContextToTest.CastMembers.FindAsync(exampleCastMember.Id);
        deletedCastMember.Should().BeNull();
        var allCastMembers = await dbContext.CastMembers.ToListAsync();
        allCastMembers.Should().HaveCount(exampleList.Count - 1);
    }

    [Fact(DisplayName = nameof(DeleteThrowsWhenCastMemberDoesNotExist))]
    [Trait("Integration/Application", "DeleteCastMember - Use Cases")]
    public async void DeleteThrowsWhenCastMemberDoesNotExist()
    {
        // Given
        var dbContext = _fixture.CreateDbContext();
        var repository = new CastMemberRepository(dbContext);
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging();
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var eventPublisher = new DomainEventPublisher(serviceProvider);
        var unitOfWork = new UnitOfWork(
            dbContext,
            eventPublisher,
            serviceProvider.GetRequiredService<ILogger<UnitOfWork>>()
        );
        var exampleList = _fixture.GetExampleCastMembersList(10);
        await dbContext.AddRangeAsync(exampleList);
        dbContext.SaveChanges();

        var input = new UseCase.DeleteCastMemberInput(Guid.NewGuid());
        var useCase = new UseCase.DeleteCastMember(repository, unitOfWork);

        // When
        var action = async () => await useCase.Handle(input, CancellationToken.None);

        // Then
        await action.Should().ThrowAsync<NotFoundException>().WithMessage($"CastMember '{input.Id}' not found.");
    }
}