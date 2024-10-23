using Entities = Domain.Entity;
using Moq;
using UseCase = Application.UseCases.Genre.CreateGenre;
using FluentAssertions;

namespace UnitTests.Application.Genre;

[Collection(nameof(CreateGenreTestFixture))]
public class CreateGenreTest
{
    private readonly CreateGenreTestFixture _fixture;

    public CreateGenreTest(CreateGenreTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(CreateGenre))]
    [Trait("Application", "CreateGenre - Use Cases")]
    public async void CreateGenre()
    {
        // Given
        var repositoryMock = _fixture.GetRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();

        var useCase = new UseCase.CreateGenre(repositoryMock.Object, unitOfWorkMock.Object);
        var input = _fixture.GetExampleInput();

        // When
        var output = await useCase.Handle(input, CancellationToken.None);

        // Then
        repositoryMock.Verify(repository => repository.Insert(It.IsAny<Entities.Genre>(), It.IsAny<CancellationToken>()), Times.Once);
        unitOfWorkMock.Verify(unitOfWork => unitOfWork.Commit(It.IsAny<CancellationToken>()), Times.Once);

        output.Should().NotBeNull();
        output.Id.Should().NotBe(Guid.Empty);
        output.Name.Should().Be(input.Name);
        output.Catetories.Should().HaveCount(0);
        output.IsActive.Should().Be(input.IsActive);
        output.CreatedAt.Should().NotBe(default);
    }

    [Fact(DisplayName = nameof(CreateGenreWithCategories))]
    [Trait("Application", "CreateGenre - Use Cases")]
    public async void CreateGenreWithCategories()
    {
        // Given
        var repositoryMock = _fixture.GetRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();

        var useCase = new UseCase.CreateGenre(repositoryMock.Object, unitOfWorkMock.Object);
        var input = _fixture.GetExampleInputWithCategories();

        // When
        var output = await useCase.Handle(input, CancellationToken.None);

        // Then
        repositoryMock.Verify(repository => repository.Insert(It.IsAny<Entities.Genre>(), It.IsAny<CancellationToken>()), Times.Once);
        unitOfWorkMock.Verify(unitOfWork => unitOfWork.Commit(It.IsAny<CancellationToken>()), Times.Once);

        output.Should().NotBeNull();
        output.Id.Should().NotBe(Guid.Empty);
        output.Name.Should().Be(input.Name);
        output.Catetories.Should().HaveCount(input.Categories?.Count ?? 0);
        input.Categories?.ForEach(id => output.Catetories.Should().Contain(id));
        output.IsActive.Should().Be(input.IsActive);
        output.CreatedAt.Should().NotBe(default);
    }
}