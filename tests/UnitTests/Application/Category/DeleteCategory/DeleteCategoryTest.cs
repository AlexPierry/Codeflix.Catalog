using Application.Exceptions;
using Domain.Entity;
using FluentAssertions;
using Moq;
using UseCase = Application.UseCases.Category.DeleteCategory;

namespace UnitTests.Application.DeleteCategory;

[Collection(nameof(DeleteCategoryTestFixture))]
public class GetCategoryTest
{
    private DeleteCategoryTestFixture _fixture;

    public GetCategoryTest(DeleteCategoryTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(DeleteCategoryOk))]
    [Trait("Application", "DeleteCategory - Use Cases")]
    public async void DeleteCategoryOk()
    {
        // Given
        var repositoryMock = _fixture.GetRepositoryMock();
        var unitOfWork = _fixture.GetUnitOfWorkMock();
        var categoryExample = _fixture.GetExampleCategory();
        repositoryMock.Setup(x => x.Get(categoryExample.Id, It.IsAny<CancellationToken>())).ReturnsAsync(categoryExample);
        var input = new UseCase.DeleteCategoryInput(categoryExample.Id);
        var useCase = new UseCase.DeleteCategory(repositoryMock.Object, unitOfWork.Object);

        // When
        await useCase.Handle(input, CancellationToken.None);

        // Then        
        repositoryMock.Verify(x => x.Get(categoryExample.Id, It.IsAny<CancellationToken>()), Times.Once);
        repositoryMock.Verify(x => x.Delete(categoryExample, It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = nameof(ThrowWhenCategoryNotFound))]
    [Trait("Application", "DeleteCategory - Use Cases")]
    public async void ThrowWhenCategoryNotFound()
    {
        // Given
        var repositoryMock = _fixture.GetRepositoryMock();
        var unitOfWork = _fixture.GetUnitOfWorkMock();
        var exampleGuid = Guid.NewGuid();
        repositoryMock.Setup(x => x.Get(exampleGuid, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException($"Category '{exampleGuid}' not found"));
        var input = new UseCase.DeleteCategoryInput(exampleGuid);
        var useCase = new UseCase.DeleteCategory(repositoryMock.Object, unitOfWork.Object);

        // When
        var task = async () => await useCase.Handle(input, CancellationToken.None);

        // Then
        await task.Should().ThrowAsync<NotFoundException>();
        repositoryMock.Verify(x => x.Get(exampleGuid, It.IsAny<CancellationToken>()), Times.Once);
        repositoryMock.Verify(x => x.Delete(It.IsAny<Category>(), It.IsAny<CancellationToken>()), Times.Never);
        unitOfWork.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Never);
    }
}