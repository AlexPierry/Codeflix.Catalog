using Domain.Entity;
using Domain.Repository;
using FluentAssertions;
using Moq;
using Application.Interfaces;
using UseCases = Application.UseCases.Category.CreateCategory;

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

        var useCase = new UseCases.CreateCategory(repositoryMock.Object, unitOfWorkMock.Object);
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

}