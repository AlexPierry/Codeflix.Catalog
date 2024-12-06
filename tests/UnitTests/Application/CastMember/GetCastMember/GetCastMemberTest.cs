using Application.Exceptions;
using FluentAssertions;
using Moq;
using UseCase = Application.UseCases.CastMember;

namespace UnitTests.Application.CastMember;

[Collection(nameof(GetCastMemberTestFixture))]
public class GetCastMemberTest
{
    private readonly GetCastMemberTestFixture _fixture;

    public GetCastMemberTest(GetCastMemberTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(GetCastMemberOk))]
    [Trait("Application", "GetCastMember - Use Cases")]
    public async void GetCastMemberOk()
    {
        // Given
        var repositoryMock = _fixture.GetRepositoryMock();
        var exampleCastMember = _fixture.GetExampleCastMember();
        repositoryMock.Setup(x => x.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(exampleCastMember);
        var input = new UseCase.GetCastMemberInput(exampleCastMember.Id);
        var useCase = new UseCase.GetCastMember(repositoryMock.Object);

        // When
        var output = await useCase.Handle(input, CancellationToken.None);

        // Then
        output.Should().NotBeNull();
        output.Id.Should().Be(exampleCastMember.Id);
        output.Name.Should().Be(exampleCastMember.Name);
        output.Type.Should().Be(exampleCastMember.Type);
        output.CreatedAt.Should().Be(exampleCastMember.CreatedAt);
        repositoryMock.Verify(x => x.Get(It.Is<Guid>(x => x == input.Id), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = nameof(NotFoundExceptionWhenCastMemberDoesNotExist))]
    [Trait("Application", "GetCastMember - Use Cases")]
    public async void NotFoundExceptionWhenCastMemberDoesNotExist()
    {
        // Given
        var repositoryMock = _fixture.GetRepositoryMock();
        var exampleGuid = Guid.NewGuid();
        repositoryMock.Setup(x => x.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException($"CastMember '{exampleGuid}' not found."));

        var input = new UseCase.GetCastMemberInput(exampleGuid);
        var useCase = new UseCase.GetCastMember(repositoryMock.Object);

        // When
        var task = async () => await useCase.Handle(input, CancellationToken.None);

        // Then
        await task.Should().ThrowAsync<NotFoundException>();
        repositoryMock.Verify(x => x.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}