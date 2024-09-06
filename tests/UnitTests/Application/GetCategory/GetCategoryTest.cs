using Application.Exceptions;
using FluentAssertions;
using Moq;
using UseCase = Application.UseCases.Category.GetCategory;

namespace UnitTests.Application.GetCategory;

[Collection(nameof(GetCategoryTestFixture))]
public class GetCategoryTest
{
    private readonly GetCategoryTestFixture _fixture;

    public GetCategoryTest(GetCategoryTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(GetCategoryOk))]
    [Trait("Application", "GetCategory - Use Cases")]
    public async void GetCategoryOk()
    {
        // Given
        var repositoryMock = _fixture.GetRepositoryMock();
        var exampleCategory = _fixture.GetExampleCategory();
        repositoryMock.Setup(x => x.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(exampleCategory);
        var input = new UseCase.GetCategoryInput(exampleCategory.Id);
        var useCase = new UseCase.GetCategory(repositoryMock.Object);

        // When
        var output = await useCase.Handle(input, CancellationToken.None);

        // Then
        output.Should().NotBeNull();
        output.Id.Should().Be(exampleCategory.Id);
        output.Name.Should().Be(exampleCategory.Name);
        output.Description.Should().Be(exampleCategory.Description);
        output.IsActive.Should().Be(exampleCategory.IsActive);
        output.CreatedAt.Should().Be(exampleCategory.CreatedAt);
        repositoryMock.Verify(x => x.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = nameof(NotFoundExceptionWhenCategoryNotExists))]
    [Trait("Application", "GetCategory - Use Cases")]
    public async void NotFoundExceptionWhenCategoryNotExists()
    {
        // Given
        var repositoryMock = _fixture.GetRepositoryMock();
        var exampleGuid = Guid.NewGuid();
        repositoryMock.Setup(x => x.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException($"Category '{exampleGuid}' not found."));

        var input = new UseCase.GetCategoryInput(exampleGuid);
        var useCase = new UseCase.GetCategory(repositoryMock.Object);

        // When
        var task = async () => await useCase.Handle(input, CancellationToken.None);

        // Then
        await task.Should().ThrowAsync<NotFoundException>();
        repositoryMock.Verify(x => x.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}