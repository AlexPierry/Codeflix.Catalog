using Application;
using Application.Exceptions;
using Application.UseCases.CastMember;
using FluentAssertions;
using Infra.Data.EF;
using Infra.Data.EF.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace IntegrationTest.Application.UseCases.CastMember;

[Collection(nameof(UpdateCastMemberTestFixture))]
public class UpdateCastMemberTest
{
    private readonly UpdateCastMemberTestFixture _fixture;

    public UpdateCastMemberTest(UpdateCastMemberTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(UpdateCastMemberOk))]
    [Trait("Integration/Application", "UpdateCastMember - Use Cases")]
    public async Task UpdateCastMemberOk()
    {
        //Given        
        var exampleCastMembersList = _fixture.GetExampleCastMembersList(5);
        var dbContext = _fixture.CreateDbContext();
        await dbContext.CastMembers.AddRangeAsync(exampleCastMembersList);
        await dbContext.SaveChangesAsync();
        var exampleCastMember = exampleCastMembersList[3];

        var actDbContext = _fixture.CreateDbContext(true);
        var repository = new CastMemberRepository(actDbContext);

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging();
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var eventPublisher = new DomainEventPublisher(serviceProvider);
        var unitOfWork = new UnitOfWork(
            dbContext,
            eventPublisher,
            serviceProvider.GetRequiredService<ILogger<UnitOfWork>>()
        );

        var useCase = new UpdateCastMember(repository, unitOfWork);
        exampleCastMember.Update(_fixture.GetValidName(), _fixture.GetRandomCastMemberType());

        var input = _fixture.GetInput(exampleCastMember);

        //When
        var output = await useCase.Handle(input, CancellationToken.None);

        //Then
        output.Should().NotBeNull();
        output.Id.Should().Be(exampleCastMember.Id);
        output.Name.Should().Be(exampleCastMember.Name);
        output.Type.Should().Be(exampleCastMember.Type);
        output.CreatedAt.Should().BeSameDateAs(exampleCastMember.CreatedAt);

        var dbCastMember = await _fixture.CreateDbContext(true).CastMembers.FindAsync(exampleCastMember.Id);
        dbCastMember.Should().NotBeNull();
        dbCastMember!.Name.Should().Be(exampleCastMember.Name);
        dbCastMember.Type.Should().Be(exampleCastMember.Type);
        dbCastMember.CreatedAt.Should().BeSameDateAs(exampleCastMember.CreatedAt);
    }

    [Fact(DisplayName = nameof(UpdateThrowsWhenCastMemberDoesNotExist))]
    [Trait("Integration/Application", "UpdateCastMember - Use Cases")]
    public async void UpdateThrowsWhenCastMemberDoesNotExist()
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

        var input = new UpdateCastMemberInput(Guid.NewGuid(), _fixture.GetValidName(), _fixture.GetRandomCastMemberType());
        var useCase = new UpdateCastMember(repository, unitOfWork);

        // When
        var action = async () => await useCase.Handle(input, CancellationToken.None);

        // Then
        await action.Should().ThrowAsync<NotFoundException>().WithMessage($"CastMember '{input.Id}' not found.");
    }
}