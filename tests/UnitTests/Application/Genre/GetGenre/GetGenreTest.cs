using Application.Exceptions;
using FluentAssertions;
using Moq;
using UseCase = Application.UseCases.Genre.GetGenre;

namespace UnitTests.Application.Genre;

[Collection(nameof(GetGenreTestFixture))]
public class GetGenreTest
{
    private readonly GetGenreTestFixture _fixture;

    public GetGenreTest(GetGenreTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(GetGenre))]
    [Trait("Application", "GetGenre - Use Cases")]
    public async void GetGenre()
    {
        // Given
        var repositoryMock = _fixture.GetRepositoryMock();
        var exampleGenre = _fixture.GetExampleGenre(null, Enumerable.Range(1, 5).Select(_ => Guid.NewGuid()).ToList());

        repositoryMock.Setup(x => x.Get(It.Is<Guid>(x => x == exampleGenre.Id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleGenre);

        var useCase = new UseCase.GetGenre(repositoryMock.Object);
        var input = _fixture.GetExampleInput(exampleGenre.Id);

        // When
        var output = await useCase.Handle(input, CancellationToken.None);

        // Then
        repositoryMock.Verify(repository => repository.Get(It.Is<Guid>(x => x == exampleGenre.Id),
            It.IsAny<CancellationToken>()), Times.Once);
        output.Should().NotBeNull();
        output.Id.Should().Be(exampleGenre.Id);
        output.Name.Should().Be(exampleGenre.Name);
        output.Catetories.Should().HaveCount(exampleGenre.Categories.Count());
        output.Catetories.Select(relation => relation.Id).Except(exampleGenre.Categories!).Should().HaveCount(0);
        output.IsActive.Should().Be(exampleGenre.IsActive);
        output.CreatedAt.Should().BeSameDateAs(exampleGenre.CreatedAt);
    }

    [Fact(DisplayName = nameof(ThrowWhenNotFound))]
    [Trait("Application", "GetGenre - Use Cases")]
    public async void ThrowWhenNotFound()
    {
        // Given
        var repositoryMock = _fixture.GetRepositoryMock();
        var exampleId = Guid.NewGuid();

        repositoryMock.Setup(x => x.Get(It.Is<Guid>(x => x == exampleId), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException($"Genre '{exampleId}' not found."));

        var useCase = new UseCase.GetGenre(repositoryMock.Object);
        var input = _fixture.GetExampleInput(exampleId);

        // When
        var action = async () => await useCase.Handle(input, CancellationToken.None);

        // Then
        await action.Should().ThrowAsync<NotFoundException>().WithMessage($"Genre '{exampleId}' not found.");
        repositoryMock.Verify(repository => repository.Get(It.Is<Guid>(x => x == exampleId),
            It.IsAny<CancellationToken>()), Times.Once);
    }

}