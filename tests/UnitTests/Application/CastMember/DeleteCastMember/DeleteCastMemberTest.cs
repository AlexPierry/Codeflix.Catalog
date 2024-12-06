using Application.Exceptions;
using FluentAssertions;
using Moq;
using UseCase = Application.UseCases.CastMember;
using Entities = Domain.Entity;

namespace UnitTests.Application.CastMember;

[Collection(nameof(DeleteCastMemberTestFixture))]
public class DeleteCastMemberTest
{
    private readonly DeleteCastMemberTestFixture _fixture;

    public DeleteCastMemberTest(DeleteCastMemberTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(DeleteCastMemberOk))]
    [Trait("Application", "DeleteCastMember - Use Cases")]
    public async void DeleteCastMemberOk()
    {
        // Given
        var repositoryMock = _fixture.GetRepositoryMock();
        var unitOfWork = _fixture.GetUnitOfWorkMock();
        var castMemberExample = _fixture.GetExampleCastMember();
        repositoryMock.Setup(x => x.Get(castMemberExample.Id, It.IsAny<CancellationToken>())).ReturnsAsync(castMemberExample);
        var input = new UseCase.DeleteCastMemberInput(castMemberExample.Id);
        var useCase = new UseCase.DeleteCastMember(repositoryMock.Object, unitOfWork.Object);

        // When
        await useCase.Handle(input, CancellationToken.None);

        // Then        
        repositoryMock.Verify(x => x.Get(castMemberExample.Id, It.IsAny<CancellationToken>()), Times.Once);
        repositoryMock.Verify(x => x.Delete(castMemberExample, It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = nameof(ThrowWhenCastMemberNotFound))]
    [Trait("Application", "DeleteCastMember - Use Cases")]
    public async void ThrowWhenCastMemberNotFound()
    {
        // Given
        var repositoryMock = _fixture.GetRepositoryMock();
        var unitOfWork = _fixture.GetUnitOfWorkMock();
        var exampleGuid = Guid.NewGuid();
        repositoryMock.Setup(x => x.Get(exampleGuid, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException($"CastMember '{exampleGuid}' not found"));
        var input = new UseCase.DeleteCastMemberInput(exampleGuid);
        var useCase = new UseCase.DeleteCastMember(repositoryMock.Object, unitOfWork.Object);

        // When
        var task = async () => await useCase.Handle(input, CancellationToken.None);

        // Then
        await task.Should().ThrowAsync<NotFoundException>();
        repositoryMock.Verify(x => x.Get(exampleGuid, It.IsAny<CancellationToken>()), Times.Once);
        repositoryMock.Verify(x => x.Delete(It.IsAny<Entities.CastMember>(), It.IsAny<CancellationToken>()), Times.Never);
        unitOfWork.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Never);
    }
}