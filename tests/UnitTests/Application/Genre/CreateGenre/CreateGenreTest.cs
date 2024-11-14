using Entities = Domain.Entity;
using Moq;
using UseCase = Application.UseCases.Genre.CreateGenre;
using FluentAssertions;
using Application.Exceptions;
using Domain.Exceptions;

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
        var categoryRepoMock = _fixture.GetRepositoryGenreMock();
        var repositoryMock = _fixture.GetRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();

        var useCase = new UseCase.CreateGenre(repositoryMock.Object, unitOfWorkMock.Object, categoryRepoMock.Object);
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
        var input = _fixture.GetExampleInputWithCategories();
        var categoryRepoMock = _fixture.GetRepositoryGenreMock();
        categoryRepoMock.Setup(x => x.GetIdsListByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(input.Categories!);
        var repositoryMock = _fixture.GetRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();

        var useCase = new UseCase.CreateGenre(repositoryMock.Object, unitOfWorkMock.Object, categoryRepoMock.Object);

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

    [Fact(DisplayName = nameof(CreateThrowsWhenCategoryDoesNotExist))]
    [Trait("Application", "CreateGenre - Use Cases")]
    public async void CreateThrowsWhenCategoryDoesNotExist()
    {
        // Given
        var input = _fixture.GetExampleInputWithCategories();
        var exampleGuid = input.Categories![^1]; // pega o último elemento
        var categoryRepoMock = _fixture.GetRepositoryGenreMock();
        categoryRepoMock.Setup(x => x.GetIdsListByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(input.Categories.Except([exampleGuid]).ToList());
        var repositoryMock = _fixture.GetRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();

        var useCase = new UseCase.CreateGenre(repositoryMock.Object, unitOfWorkMock.Object, categoryRepoMock.Object);

        // When
        var action = async () => await useCase.Handle(input, CancellationToken.None);

        // Then
        await action.Should().ThrowAsync<RelatedAggregateException>().WithMessage($"Related category id(s) not found: {exampleGuid}");
        categoryRepoMock.Verify(x => x.GetIdsListByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory(DisplayName = nameof(ThrowsWhenNameIsInvalid))]
    [Trait("Application", "CreateGenre - Use Cases")]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("  ")]
    public async void ThrowsWhenNameIsInvalid(string? invalidName)
    {
        // Given
        var input = _fixture.GetExampleInput();
        input.Name = invalidName!;
        var categoryRepoMock = _fixture.GetRepositoryGenreMock();
        var repositoryMock = _fixture.GetRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();

        var useCase = new UseCase.CreateGenre(repositoryMock.Object, unitOfWorkMock.Object, categoryRepoMock.Object);

        // When
        var action = async () => await useCase.Handle(input, CancellationToken.None);

        // Then
        await action.Should().ThrowAsync<EntityValidationException>().WithMessage($"Name should not be null or empty.");
    }
}