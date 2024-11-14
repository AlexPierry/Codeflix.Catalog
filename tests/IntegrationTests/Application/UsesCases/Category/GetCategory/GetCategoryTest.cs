using Application.Exceptions;
using FluentAssertions;
using Infra.Data.EF.Repositories;
using UseCase = Application.UseCases.Category.GetCategory;

namespace IntegrationTest.Application.UseCases.Category;

[Collection(nameof(GetCategoryTestFixture))]
public class GetCategoryTest
{
    private readonly GetCategoryTestFixture _fixture;

    public GetCategoryTest(GetCategoryTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(GetCategoryOk))]
    [Trait("Integration/Application", "GetCategory - Use Cases")]
    public async Task GetCategoryOk()
    {
        // Given
        var dbContext = _fixture.CreateDbContext();
        var repository = new CategoryRepository(dbContext);
        var exampleCategory = _fixture.GetExampleCategory();
        dbContext.Add(exampleCategory);
        dbContext.SaveChanges();

        var input = new UseCase.GetCategoryInput(exampleCategory.Id);
        var useCase = new UseCase.GetCategory(repository);

        // When
        var output = await useCase.Handle(input, CancellationToken.None);

        // Then
        output.Should().NotBeNull();
        output.Id.Should().Be(exampleCategory.Id);
        output.Name.Should().Be(exampleCategory.Name);
        output.Description.Should().Be(exampleCategory.Description);
        output.IsActive.Should().Be(exampleCategory.IsActive);
        output.CreatedAt.Should().Be(exampleCategory.CreatedAt);
    }

    [Fact(DisplayName = nameof(NotFoundExceptionWhenCategoryDoesNotExist))]
    [Trait("Integration/Application", "GetCategory - Use Cases")]
    public async Task NotFoundExceptionWhenCategoryDoesNotExist()
    {
        // Given
        var dbContext = _fixture.CreateDbContext();
        var repository = new CategoryRepository(dbContext);
        var exampleCategory = _fixture.GetExampleCategory();
        dbContext.Add(exampleCategory);
        dbContext.SaveChanges();

        var input = new UseCase.GetCategoryInput(Guid.NewGuid());
        var useCase = new UseCase.GetCategory(repository);

        // When
        var task = async () => await useCase.Handle(input, CancellationToken.None);

        // Then
        await task.Should().ThrowAsync<NotFoundException>().WithMessage($"Category '{input.Id}' not found.");
    }
}