using Application.Exceptions;
using Application.UseCases.Category.Common;
using Application.UseCases.Category.UpdateCategory;
using Domain.Entity;
using Domain.Exceptions;
using FluentAssertions;
using Moq;
using UseCase = Application.UseCases.Category.UpdateCategory;

namespace UnitTests.Application.UpdateCategory;

[Collection(nameof(UpdateCategoryTestFixture))]
public class UpdateCategoryTest
{
    private readonly UpdateCategoryTestFixture _fixture;

    public UpdateCategoryTest(UpdateCategoryTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Theory(DisplayName = nameof(UpdateOk))]
    [Trait("Application", "UpdateCategory - Use Cases")]
    [MemberData(
        nameof(UpdateCategoryTestDataGenerator.GetCategoriesToUpdate),
        parameters: 10,
        MemberType = typeof(UpdateCategoryTestDataGenerator)
    )]
    public async void UpdateOk(Category exampleCategory, UpdateCategoryInput input)
    {
        // Given
        var repositoryMock = _fixture.GetRepositoryMock();
        var unitOfWork = _fixture.GetUnitOfWorkMock();
        repositoryMock.Setup(x => x.Get(exampleCategory.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleCategory);
        var useCase = new UseCase.UpdateCategory(repositoryMock.Object, unitOfWork.Object);

        // When
        CategoryModelOutput output = await useCase.Handle(input, CancellationToken.None);

        // Then
        output.Should().NotBeNull();
        output.Id.Should().NotBe(Guid.Empty);
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(input.Description);
        output.IsActive.Should().Be((bool)input.IsActive!);
        output.CreatedAt.Should().Be(exampleCategory.CreatedAt);
        repositoryMock.Verify(x => x.Get(exampleCategory.Id, It.IsAny<CancellationToken>()), Times.Once);
        repositoryMock.Verify(x => x.Update(exampleCategory, It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = nameof(ThrowExceptionWhenCategoryNotFound))]
    [Trait("Application", "UpdateCategory - Use Cases")]
    public async void ThrowExceptionWhenCategoryNotFound()
    {
        // Given
        var repositoryMock = _fixture.GetRepositoryMock();
        var unitOfWork = _fixture.GetUnitOfWorkMock();
        var exampleGuid = Guid.NewGuid();
        repositoryMock.Setup(x => x.Get(exampleGuid, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException($"Category '{exampleGuid}' not found"));

        var useCase = new UseCase.UpdateCategory(repositoryMock.Object, unitOfWork.Object);

        // When
        var task = async () => await useCase.Handle(_fixture.GetValidInput(exampleGuid), CancellationToken.None);

        // Then
        await task.Should().ThrowAsync<NotFoundException>();
        repositoryMock.Verify(x => x.Get(exampleGuid, It.IsAny<CancellationToken>()), Times.Once);
        repositoryMock.Verify(x => x.Update(It.IsAny<Category>(), It.IsAny<CancellationToken>()), Times.Never);
        unitOfWork.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Theory(DisplayName = nameof(UpdateCategoryNotProvidingIsActive))]
    [Trait("Application", "UpdateCategory - Use Cases")]
    [MemberData(
        nameof(UpdateCategoryTestDataGenerator.GetCategoriesToUpdate),
        parameters: 10,
        MemberType = typeof(UpdateCategoryTestDataGenerator)
    )]
    public async void UpdateCategoryNotProvidingIsActive(Category exampleCategory, UpdateCategoryInput exampleInput)
    {
        // Given
        var input = new UpdateCategoryInput(exampleInput.Id, exampleInput.Name, exampleInput.Description);
        var repositoryMock = _fixture.GetRepositoryMock();
        var unitOfWork = _fixture.GetUnitOfWorkMock();
        repositoryMock.Setup(x => x.Get(exampleCategory.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleCategory);
        var useCase = new UseCase.UpdateCategory(repositoryMock.Object, unitOfWork.Object);

        // When
        CategoryModelOutput output = await useCase.Handle(input, CancellationToken.None);

        // Then
        output.Should().NotBeNull();
        output.Id.Should().NotBe(Guid.Empty);
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(input.Description);
        output.IsActive.Should().Be(exampleCategory.IsActive);
        output.CreatedAt.Should().Be(exampleCategory.CreatedAt);
        repositoryMock.Verify(x => x.Get(exampleCategory.Id, It.IsAny<CancellationToken>()), Times.Once);
        repositoryMock.Verify(x => x.Update(exampleCategory, It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory(DisplayName = nameof(UpdateCategoryProvidingOnlyName))]
    [Trait("Application", "UpdateCategory - Use Cases")]
    [MemberData(
        nameof(UpdateCategoryTestDataGenerator.GetCategoriesToUpdate),
        parameters: 10,
        MemberType = typeof(UpdateCategoryTestDataGenerator)
    )]
    public async void UpdateCategoryProvidingOnlyName(Category exampleCategory, UpdateCategoryInput exampleInput)
    {
        // Given
        var input = new UpdateCategoryInput(exampleInput.Id, exampleInput.Name);
        var repositoryMock = _fixture.GetRepositoryMock();
        var unitOfWork = _fixture.GetUnitOfWorkMock();
        repositoryMock.Setup(x => x.Get(exampleCategory.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleCategory);
        var useCase = new UseCase.UpdateCategory(repositoryMock.Object, unitOfWork.Object);

        // When
        CategoryModelOutput output = await useCase.Handle(input, CancellationToken.None);

        // Then
        output.Should().NotBeNull();
        output.Id.Should().NotBe(Guid.Empty);
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(exampleCategory.Description);
        output.IsActive.Should().Be(exampleCategory.IsActive);
        output.CreatedAt.Should().Be(exampleCategory.CreatedAt);
        repositoryMock.Verify(x => x.Get(exampleCategory.Id, It.IsAny<CancellationToken>()), Times.Once);
        repositoryMock.Verify(x => x.Update(exampleCategory, It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory(DisplayName = nameof(ThrowWhenCantUpdateCategory))]
    [Trait("Application", "UpdateCategory - Use Cases")]
    [MemberData(
        nameof(UpdateCategoryTestDataGenerator.GetInvalidInputs),
        parameters: 12,
        MemberType = typeof(UpdateCategoryTestDataGenerator))]
    public async void ThrowWhenCantUpdateCategory(UseCase.UpdateCategoryInput input, string exceptionMessage)
    {
        // Given
        var repositoryMock = _fixture.GetRepositoryMock();
        var exampleCategory = _fixture.GetExampleCategory();
        input.Id = exampleCategory.Id;
        repositoryMock.Setup(x => x.Get(exampleCategory.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleCategory);

        var useCase = new UseCase.UpdateCategory(repositoryMock.Object, _fixture.GetUnitOfWorkMock().Object);

        // When
        Func<Task> task = async () => await useCase.Handle(input, CancellationToken.None);

        // Then
        await task.Should().ThrowAsync<EntityValidationException>().WithMessage(exceptionMessage);
        repositoryMock.Verify(x => x.Get(exampleCategory.Id, It.IsAny<CancellationToken>()), Times.Once);
    }
}
