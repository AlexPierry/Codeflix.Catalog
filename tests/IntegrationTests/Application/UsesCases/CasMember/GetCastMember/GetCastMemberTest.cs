using Application.Exceptions;
using FluentAssertions;
using Infra.Data.EF.Repositories;
using UseCase = Application.UseCases.CastMember;

namespace IntegrationTest.Application.UseCases.CastMember;

[Collection(nameof(GetCastMemberTestFixture))]
public class GetCastMemberTest
{
    private readonly GetCastMemberTestFixture _fixture;

    public GetCastMemberTest(GetCastMemberTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(GetCastMemberOk))]
    [Trait("Integration/Application", "GetCastMember - Use Cases")]
    public async Task GetCastMemberOk()
    {
        // Given
        var dbContext = _fixture.CreateDbContext();
        var repository = new CastMemberRepository(dbContext);
        var exampleCastMember = _fixture.GetExampleCastMember();
        dbContext.Add(exampleCastMember);
        dbContext.SaveChanges();

        var input = new UseCase.GetCastMemberInput(exampleCastMember.Id);
        var useCase = new UseCase.GetCastMember(repository);

        // When
        var output = await useCase.Handle(input, CancellationToken.None);

        // Then
        output.Should().NotBeNull();
        output.Id.Should().Be(exampleCastMember.Id);
        output.Name.Should().Be(exampleCastMember.Name);
        output.Type.Should().Be(exampleCastMember.Type);
        output.CreatedAt.Should().Be(exampleCastMember.CreatedAt);
    }

    [Fact(DisplayName = nameof(NotFoundExceptionWhenCastMemberDoesNotExist))]
    [Trait("Integration/Application", "GetCastMember - Use Cases")]
    public async Task NotFoundExceptionWhenCastMemberDoesNotExist()
    {
        // Given
        var dbContext = _fixture.CreateDbContext();
        var repository = new CastMemberRepository(dbContext);
        var exampleCastMember = _fixture.GetExampleCastMember();
        dbContext.Add(exampleCastMember);
        dbContext.SaveChanges();

        var input = new UseCase.GetCastMemberInput(Guid.NewGuid());
        var useCase = new UseCase.GetCastMember(repository);

        // When
        var task = async () => await useCase.Handle(input, CancellationToken.None);

        // Then
        await task.Should().ThrowAsync<NotFoundException>().WithMessage($"CastMember '{input.Id}' not found.");
    }    
}