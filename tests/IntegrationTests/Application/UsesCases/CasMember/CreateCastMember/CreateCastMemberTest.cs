using Application;
using FluentAssertions;
using Infra.Data.EF;
using Infra.Data.EF.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using UseCase = Application.UseCases.CastMember;

namespace IntegrationTest.Application.UseCases.CastMember;

[Collection(nameof(CreateCastMemberTestFixture))]
public class CreateCastMemberTest
{
    private readonly CreateCastMemberTestFixture _fixture;

    public CreateCastMemberTest(CreateCastMemberTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(CreateCastMember))]
    [Trait("Integration/Application", "CreateCastMember - Use Cases")]
    public async void CreateCastMember()
    {
        // Given
        var dbContext = _fixture.CreateDbContext();
        var repository = new CastMemberRepository(dbContext);
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging();
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var eventPublisher = new DomainEventPublisher(serviceProvider);
        var unitOfWorkMock = new UnitOfWork(
            dbContext,
            eventPublisher,
            serviceProvider.GetRequiredService<ILogger<UnitOfWork>>()
        );

        var useCase = new UseCase.CreateCastMember(repository, unitOfWorkMock);
        var input = _fixture.GetInput();

        // When
        var output = await useCase.Handle(input, CancellationToken.None);

        // Then
        output.Should().NotBeNull();
        output.Id.Should().NotBe(Guid.Empty);
        output.Name.Should().Be(input.Name);
        output.Type.Should().Be(input.Type);
        output.CreatedAt.Should().NotBe(default);

        var dbCastMember = await _fixture.CreateDbContext(true).CastMembers.FindAsync(output.Id);
        dbCastMember.Should().NotBeNull();
        dbCastMember!.Name.Should().Be(input.Name);
        dbCastMember.Type.Should().Be(input.Type);
        dbCastMember.CreatedAt.Should().Be(output.CreatedAt);
    }
}