using Application.Exceptions;
using FluentAssertions;
using Moq;
using UseCase = Application.UseCases.Genre.DeleteGenre;
using Entities = Domain.Entity;

namespace UnitTests.Application.Genre;

[Collection(nameof(DeleteGenreTestFixture))]
public class DeleteGenreTest
{
    private readonly DeleteGenreTestFixture _fixture;

    public DeleteGenreTest(DeleteGenreTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(DeleteGenreOk))]
    [Trait("Application", "DeleteGenre - Use Cases")]
    public async void DeleteGenreOk()
    {
        // Given
        var repositoryMock = _fixture.GetRepositoryMock();
        var unitOfWork = _fixture.GetUnitOfWorkMock();
        var genreExample = _fixture.GetExampleGenre();
        repositoryMock.Setup(x => x.Get(genreExample.Id, It.IsAny<CancellationToken>())).ReturnsAsync(genreExample);
        var input = new UseCase.DeleteGenreInput(genreExample.Id);
        var useCase = new UseCase.DeleteGenre(repositoryMock.Object, unitOfWork.Object);

        // When
        await useCase.Handle(input, CancellationToken.None);

        // Then        
        repositoryMock.Verify(x => x.Get(genreExample.Id, It.IsAny<CancellationToken>()), Times.Once);
        repositoryMock.Verify(x => x.Delete(genreExample, It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = nameof(ThrowWhenGenreNotFound))]
    [Trait("Application", "DeleteGenre - Use Cases")]
    public async void ThrowWhenGenreNotFound()
    {
        // Given
        var repositoryMock = _fixture.GetRepositoryMock();
        var unitOfWork = _fixture.GetUnitOfWorkMock();
        var exampleGuid = Guid.NewGuid();
        repositoryMock.Setup(x => x.Get(exampleGuid, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException($"Genre '{exampleGuid}' not found"));
        var input = new UseCase.DeleteGenreInput(exampleGuid);
        var useCase = new UseCase.DeleteGenre(repositoryMock.Object, unitOfWork.Object);

        // When
        var task = async () => await useCase.Handle(input, CancellationToken.None);

        // Then
        await task.Should().ThrowAsync<NotFoundException>().WithMessage($"Genre '{exampleGuid}' not found");
        repositoryMock.Verify(x => x.Get(exampleGuid, It.IsAny<CancellationToken>()), Times.Once);
        repositoryMock.Verify(x => x.Delete(It.IsAny<Entities.Genre>(), It.IsAny<CancellationToken>()), Times.Never);
        unitOfWork.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Never);
    }
}