using Moq;
using UseCase = Application.UseCases.Genre.UpdateGenre;
using Entities = Domain.Entity;
using FluentAssertions;
using Application.Exceptions;
using Domain.Exceptions;

namespace UnitTests.Application.Genre;

[Collection(nameof(UpdateGenreTestFixture))]
public class UpdateGenreTest
{
    private readonly UpdateGenreTestFixture _fixture;

    public UpdateGenreTest(UpdateGenreTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(UpdateGenre))]
    [Trait("Application", "UpdateGenre - Use Cases")]
    public async void UpdateGenre()
    {
        // Given
        var categoryRepoMock = _fixture.GetCategoryRepositoryMock();
        var repositoryMock = _fixture.GetRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var exampleGenre = _fixture.GetExampleGenre();

        repositoryMock.Setup(x => x.Get(It.Is<Guid>(x => x == exampleGenre.Id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleGenre);

        var useCase = new UseCase.UpdateGenre(repositoryMock.Object, unitOfWorkMock.Object, categoryRepoMock.Object);
        var input = _fixture.GetExampleInput(exampleGenre.Id);

        // When
        var output = await useCase.Handle(input, CancellationToken.None);

        // Then
        repositoryMock.Verify(repository => repository.Update(It.Is<Entities.Genre>(x => x.Id == exampleGenre.Id),
            It.IsAny<CancellationToken>()), Times.Once);
        unitOfWorkMock.Verify(unitOfWork => unitOfWork.Commit(It.IsAny<CancellationToken>()), Times.Once);

        output.Should().NotBeNull();
        output.Id.Should().Be(input.Id);
        output.Name.Should().Be(input.Name);
        output.Catetories.Should().HaveCount(0);
        output.IsActive.Should().Be((bool)input.IsActive!);
        output.CreatedAt.Should().BeSameDateAs(exampleGenre.CreatedAt);
    }

    [Fact(DisplayName = nameof(ThrowsWhenNotFound))]
    [Trait("Application", "UpdateGenre - Use Cases")]
    public async void ThrowsWhenNotFound()
    {
        // Given
        var categoryRepoMock = _fixture.GetCategoryRepositoryMock();
        var repositoryMock = _fixture.GetRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var exampleId = Guid.NewGuid();
        var exampleInput = _fixture.GetExampleInput(exampleId);

        repositoryMock.Setup(x => x.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException($"Genre '{exampleId}' not found."));

        var useCase = new UseCase.UpdateGenre(repositoryMock.Object, unitOfWorkMock.Object, categoryRepoMock.Object);

        // When
        var action = async () => await useCase.Handle(exampleInput, CancellationToken.None);

        // Then
        await action.Should().ThrowAsync<NotFoundException>().WithMessage($"Genre '{exampleId}' not found.");
    }

    [Theory(DisplayName = nameof(ThrowsWhenNameIsInvalid))]
    [Trait("Application", "UpdateGenre - Use Cases")]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("  ")]
    public async void ThrowsWhenNameIsInvalid(string? invalidName)
    {
        // Given
        var categoryRepoMock = _fixture.GetCategoryRepositoryMock();
        var repositoryMock = _fixture.GetRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var exampleGenre = _fixture.GetExampleGenre();

        repositoryMock.Setup(x => x.Get(It.Is<Guid>(x => x == exampleGenre.Id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleGenre);

        var useCase = new UseCase.UpdateGenre(repositoryMock.Object, unitOfWorkMock.Object, categoryRepoMock.Object);
        var input = _fixture.GetExampleInput(exampleGenre.Id);
        input.Name = invalidName!;

        // When
        var action = async () => await useCase.Handle(input, CancellationToken.None);

        // Then
        await action.Should().ThrowAsync<EntityValidationException>().WithMessage($"Name should not be null or empty.");
    }

    [Theory(DisplayName = nameof(UpdateGenreOnlyName))]
    [Trait("Application", "UpdateGenre - Use Cases")]
    [InlineData(true)]
    [InlineData(false)]
    [InlineData(null)]
    public async void UpdateGenreOnlyName(bool? isActive)
    {
        // Given
        var categoryRepoMock = _fixture.GetCategoryRepositoryMock();
        var repositoryMock = _fixture.GetRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var exampleGenre = _fixture.GetExampleGenre(isActive);

        repositoryMock.Setup(x => x.Get(It.Is<Guid>(x => x == exampleGenre.Id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleGenre);

        var useCase = new UseCase.UpdateGenre(repositoryMock.Object, unitOfWorkMock.Object, categoryRepoMock.Object);
        var input = _fixture.GetExampleInput(exampleGenre.Id);
        input.IsActive = null;

        // When
        var output = await useCase.Handle(input, CancellationToken.None);

        // Then
        repositoryMock.Verify(repository => repository.Update(It.Is<Entities.Genre>(x => x.Id == exampleGenre.Id),
            It.IsAny<CancellationToken>()), Times.Once);
        unitOfWorkMock.Verify(unitOfWork => unitOfWork.Commit(It.IsAny<CancellationToken>()), Times.Once);

        output.Should().NotBeNull();
        output.Id.Should().Be(input.Id);
        output.Name.Should().Be(input.Name);
        output.Catetories.Should().HaveCount(0);
        output.IsActive.Should().Be(exampleGenre.IsActive!);
        output.CreatedAt.Should().BeSameDateAs(exampleGenre.CreatedAt);
    }

    [Fact(DisplayName = nameof(UpdateGenreAddingCategoriesIds))]
    [Trait("Application", "UpdateGenre - Use Cases")]
    public async void UpdateGenreAddingCategoriesIds()
    {
        // Given
        var categoryRepoMock = _fixture.GetCategoryRepositoryMock();
        var repositoryMock = _fixture.GetRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var exampleGenre = _fixture.GetExampleGenre();

        repositoryMock.Setup(x => x.Get(It.Is<Guid>(x => x == exampleGenre.Id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleGenre);

        var useCase = new UseCase.UpdateGenre(repositoryMock.Object, unitOfWorkMock.Object, categoryRepoMock.Object);
        var input = _fixture.GetExampleInputWithCategories(exampleGenre.Id);

        categoryRepoMock.Setup(x => x.GetIdsListByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(input.Categories!);

        // When
        var output = await useCase.Handle(input, CancellationToken.None);

        // Then
        repositoryMock.Verify(repository => repository.Update(It.Is<Entities.Genre>(x => x.Id == exampleGenre.Id),
            It.IsAny<CancellationToken>()), Times.Once);
        unitOfWorkMock.Verify(unitOfWork => unitOfWork.Commit(It.IsAny<CancellationToken>()), Times.Once);

        output.Should().NotBeNull();
        output.Id.Should().Be(exampleGenre.Id);
        output.Name.Should().Be(exampleGenre.Name);
        output.Catetories.Should().HaveCount(input.Categories!.Count());
        output.IsActive.Should().Be(exampleGenre.IsActive);
        output.CreatedAt.Should().BeSameDateAs(exampleGenre.CreatedAt);
    }

    [Fact(DisplayName = nameof(UpdateGenreReplacingCategoriesIds))]
    [Trait("Application", "UpdateGenre - Use Cases")]
    public async void UpdateGenreReplacingCategoriesIds()
    {
        // Given
        var categoryRepoMock = _fixture.GetCategoryRepositoryMock();
        var repositoryMock = _fixture.GetRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var exampleGenre = _fixture.GetExampleGenre(null, Enumerable.Range(1, 5).Select(_ => Guid.NewGuid()).ToList());

        repositoryMock.Setup(x => x.Get(It.Is<Guid>(x => x == exampleGenre.Id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleGenre);

        var useCase = new UseCase.UpdateGenre(repositoryMock.Object, unitOfWorkMock.Object, categoryRepoMock.Object);
        var input = _fixture.GetExampleInputWithCategories(exampleGenre.Id);

        categoryRepoMock.Setup(x => x.GetIdsListByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(input.Categories!);

        // When
        var output = await useCase.Handle(input, CancellationToken.None);

        // Then
        repositoryMock.Verify(repository => repository.Update(It.Is<Entities.Genre>(x => x.Id == exampleGenre.Id),
            It.IsAny<CancellationToken>()), Times.Once);
        unitOfWorkMock.Verify(unitOfWork => unitOfWork.Commit(It.IsAny<CancellationToken>()), Times.Once);

        output.Should().NotBeNull();
        output.Id.Should().Be(exampleGenre.Id);
        output.Name.Should().Be(exampleGenre.Name);
        output.Catetories.Should().HaveCount(input.Categories!.Count());
        output.Catetories.Select(relation => relation.Id).Except(input.Categories!).Should().HaveCount(0);
        output.IsActive.Should().Be(exampleGenre.IsActive);
        output.CreatedAt.Should().BeSameDateAs(exampleGenre.CreatedAt);
    }

    [Fact(DisplayName = nameof(ThrowsWhenCategoryDoesNotExist))]
    [Trait("Application", "UpdateGenre - Use Cases")]
    public async void ThrowsWhenCategoryDoesNotExist()
    {
        // Given
        var categoryRepoMock = _fixture.GetCategoryRepositoryMock();
        var repositoryMock = _fixture.GetRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var exampleGenre = _fixture.GetExampleGenre(null, Enumerable.Range(1, 5).Select(_ => Guid.NewGuid()).ToList());

        repositoryMock.Setup(x => x.Get(It.Is<Guid>(x => x == exampleGenre.Id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleGenre);

        categoryRepoMock.Setup(x => x.GetIdsListByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleGenre.Categories!);

        var useCase = new UseCase.UpdateGenre(repositoryMock.Object, unitOfWorkMock.Object, categoryRepoMock.Object);
        var input = _fixture.GetExampleInputWithCategories(exampleGenre.Id);

        // When
        var action = async () => await useCase.Handle(input, CancellationToken.None);

        // Then
        var notFoundCategories = string.Join(", ", input.Categories!);
        await action.Should().ThrowAsync<RelatedAggregateException>().WithMessage($"Related category id(s) not found: {notFoundCategories}");
        categoryRepoMock.Verify(x => x.GetIdsListByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = nameof(UpdateGenreWithoutCategoriesIds))]
    [Trait("Application", "UpdateGenre - Use Cases")]
    public async void UpdateGenreWithoutCategoriesIds()
    {
        // Given
        var categoryRepoMock = _fixture.GetCategoryRepositoryMock();
        var repositoryMock = _fixture.GetRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var exampleGenre = _fixture.GetExampleGenre(null, Enumerable.Range(1, 5).Select(_ => Guid.NewGuid()).ToList());

        repositoryMock.Setup(x => x.Get(It.Is<Guid>(x => x == exampleGenre.Id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleGenre);

        var useCase = new UseCase.UpdateGenre(repositoryMock.Object, unitOfWorkMock.Object, categoryRepoMock.Object);
        var input = _fixture.GetExampleInput(exampleGenre.Id);

        categoryRepoMock.Setup(x => x.GetIdsListByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleGenre.Categories);

        // When
        var output = await useCase.Handle(input, CancellationToken.None);

        // Then
        repositoryMock.Verify(repository => repository.Update(It.Is<Entities.Genre>(x => x.Id == exampleGenre.Id),
            It.IsAny<CancellationToken>()), Times.Once);
        unitOfWorkMock.Verify(unitOfWork => unitOfWork.Commit(It.IsAny<CancellationToken>()), Times.Once);

        output.Should().NotBeNull();
        output.Id.Should().Be(exampleGenre.Id);
        output.Name.Should().Be(input.Name);
        output.Catetories.Should().HaveCount(exampleGenre.Categories.Count());
        output.Catetories.Select(relation => relation.Id).Except(exampleGenre.Categories!).Should().HaveCount(0);
        output.IsActive.Should().Be((bool)input.IsActive!);
        output.CreatedAt.Should().BeSameDateAs(exampleGenre.CreatedAt);
    }

    [Fact(DisplayName = nameof(UpdateGenreWithoutCategoriesIds))]
    [Trait("Application", "UpdateGenre - Use Cases")]
    public async void UpdateGenreWithEmptyCategoriesIds()
    {
        // Given
        var categoryRepoMock = _fixture.GetCategoryRepositoryMock();
        var repositoryMock = _fixture.GetRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var exampleGenre = _fixture.GetExampleGenre(null, Enumerable.Range(1, 5).Select(_ => Guid.NewGuid()).ToList());

        repositoryMock.Setup(x => x.Get(It.Is<Guid>(x => x == exampleGenre.Id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleGenre);

        var useCase = new UseCase.UpdateGenre(repositoryMock.Object, unitOfWorkMock.Object, categoryRepoMock.Object);
        var input = _fixture.GetExampleInputWithEmptyCategories(exampleGenre.Id);

        categoryRepoMock.Setup(x => x.GetIdsListByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleGenre.Categories);

        // When
        var output = await useCase.Handle(input, CancellationToken.None);

        // Then
        repositoryMock.Verify(repository => repository.Update(It.Is<Entities.Genre>(x => x.Id == exampleGenre.Id),
            It.IsAny<CancellationToken>()), Times.Once);
        unitOfWorkMock.Verify(unitOfWork => unitOfWork.Commit(It.IsAny<CancellationToken>()), Times.Once);

        output.Should().NotBeNull();
        output.Id.Should().Be(exampleGenre.Id);
        output.Name.Should().Be(input.Name);
        output.Catetories.Should().HaveCount(0);
        output.IsActive.Should().Be((bool)input.IsActive!);
        output.CreatedAt.Should().BeSameDateAs(exampleGenre.CreatedAt);
    }
}