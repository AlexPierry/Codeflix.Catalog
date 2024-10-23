using UseCase = Application.UseCases.Category.CreateCategory;
using Domain.Entity;
using Domain.Exceptions;
using FluentAssertions;
using Moq;

namespace UnitTests.Application.CreateCategory;

[Collection(nameof(CreateCategoryTestFixture))]
public class CreateCategoryTest
{
    private readonly CreateCategoryTestFixture _createCategoryTestFixture;

    public CreateCategoryTest(CreateCategoryTestFixture createCategoryTestFixture)
    {
        _createCategoryTestFixture = createCategoryTestFixture;
    }

    [Fact(DisplayName = nameof(CreateCategory))]
    [Trait("Application", "CreateCategory - Use Cases")]
    public async void CreateCategory()
    {
        // Given
        var repositoryMock = _createCategoryTestFixture.GetRepositoryMock();
        var unitOfWorkMock = _createCategoryTestFixture.GetUnitOfWorkMock();

        var useCase = new UseCase.CreateCategory(repositoryMock.Object, unitOfWorkMock.Object);
        var input = _createCategoryTestFixture.GetInput();

        // When
        var output = await useCase.Handle(input, CancellationToken.None);

        // Then
        repositoryMock.Verify(repository => repository.Insert(It.IsAny<Category>(), It.IsAny<CancellationToken>()), Times.Once);
        unitOfWorkMock.Verify(unitOfWork => unitOfWork.Commit(It.IsAny<CancellationToken>()), Times.Once);

        output.Should().NotBeNull();
        output.Id.Should().NotBe(Guid.Empty);
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(input.Description);
        output.IsActive.Should().Be(input.IsActive);
        output.CreatedAt.Should().NotBe(default(DateTime));
    }

    [Theory(DisplayName = nameof(ThrowWhenCantInstantiateCategory))]
    [Trait("Application", "CreateCategory - Use Cases")]
    [MemberData(
        nameof(CreateCategoryTestDataGenerator.GetInvalidInputs),
        parameters: 10,
        MemberType = typeof(CreateCategoryTestDataGenerator))]
    public async void ThrowWhenCantInstantiateCategory(UseCase.CreateCategoryInput input, string exceptionMessage)
    {
        // Given
        var useCase = new UseCase.CreateCategory(
            _createCategoryTestFixture.GetRepositoryMock().Object,
            _createCategoryTestFixture.GetUnitOfWorkMock().Object);

        // When
        Func<Task> task = async () => await useCase.Handle(input, CancellationToken.None);

        // Then
        await task.Should().ThrowAsync<EntityValidationException>().WithMessage(exceptionMessage);
    }

    [Fact(DisplayName = nameof(CreateCategoryWithNameOnly))]
    [Trait("Application", "CreateCategory - Use Cases")]
    public async void CreateCategoryWithNameOnly()
    {
        // Given
        var repositoryMock = _createCategoryTestFixture.GetRepositoryMock();
        var unitOfWorkMock = _createCategoryTestFixture.GetUnitOfWorkMock();

        var useCase = new UseCase.CreateCategory(repositoryMock.Object, unitOfWorkMock.Object);
        var input = new UseCase.CreateCategoryInput(_createCategoryTestFixture.GetValidCategoryName());

        // When
        var output = await useCase.Handle(input, CancellationToken.None);

        // Then
        repositoryMock.Verify(repository => repository.Insert(It.IsAny<Category>(), It.IsAny<CancellationToken>()), Times.Once);
        unitOfWorkMock.Verify(unitOfWork => unitOfWork.Commit(It.IsAny<CancellationToken>()), Times.Once);

        output.Should().NotBeNull();
        output.Id.Should().NotBe(Guid.Empty);
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be("");
        output.IsActive.Should().Be(input.IsActive);
        output.CreatedAt.Should().NotBe(default(DateTime));
    }

    [Fact(DisplayName = nameof(CreateCategoryWithNameAndDescriptionOnly))]
    [Trait("Application", "CreateCategory - Use Cases")]
    public async void CreateCategoryWithNameAndDescriptionOnly()
    {
        // Given
        var repositoryMock = _createCategoryTestFixture.GetRepositoryMock();
        var unitOfWorkMock = _createCategoryTestFixture.GetUnitOfWorkMock();

        var useCase = new UseCase.CreateCategory(repositoryMock.Object, unitOfWorkMock.Object);
        var input = new UseCase.CreateCategoryInput(
            _createCategoryTestFixture.GetValidCategoryName(),
            _createCategoryTestFixture.GetValidCategoryDescription());

        // When
        var output = await useCase.Handle(input, CancellationToken.None);

        // Then
        repositoryMock.Verify(repository => repository.Insert(It.IsAny<Category>(), It.IsAny<CancellationToken>()), Times.Once);
        unitOfWorkMock.Verify(unitOfWork => unitOfWork.Commit(It.IsAny<CancellationToken>()), Times.Once);

        output.Should().NotBeNull();
        output.Id.Should().NotBe(Guid.Empty);
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(input.Description);
        output.IsActive.Should().Be(input.IsActive);
        output.CreatedAt.Should().NotBe(default(DateTime));
    }
}