using Application.Exceptions;
using FluentAssertions;
using Infra.Data.EF;
using Infra.Data.EF.Repositories;
using Microsoft.EntityFrameworkCore;
using UseCase = Application.UseCases.Category.DeleteCategory;

namespace IntegrationTest.Application.UseCases.Category.DeleteCategory;

[Collection(nameof(DeleteCategoryTestFixture))]
public class DeleteCategoryTest
{
    private readonly DeleteCategoryTestFixture _fixture;

    public DeleteCategoryTest(DeleteCategoryTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(DeleteCategoryOk))]
    [Trait("Integration/Application", "DeleteCategory - Use Cases")]
    public async void DeleteCategoryOk()
    {
        // Given
        var dbContext = _fixture.CreateDbContext();
        var repository = new CategoryRepository(dbContext);
        var unitOfWork = new UnitOfWork(dbContext);
        var exampleCategory = _fixture.GetExampleCategory();
        var tracking = await dbContext.AddAsync(exampleCategory);
        var exampleList = _fixture.GetExampleCategoriesList(10);
        await dbContext.AddRangeAsync(exampleList);
        dbContext.SaveChanges();

        tracking.State = EntityState.Detached;

        var input = new UseCase.DeleteCategoryInput(exampleCategory.Id);
        var useCase = new UseCase.DeleteCategory(repository, unitOfWork);

        // When
        await useCase.Handle(input, CancellationToken.None);

        // Then
        var dbContextToTest = _fixture.CreateDbContext(true);
        var deletedCategory = await dbContextToTest.Categories.FindAsync(exampleCategory.Id);
        deletedCategory.Should().BeNull();
        var allCategories = await dbContext.Categories.ToListAsync();
        allCategories.Should().HaveCount(exampleList.Count);
    }

    [Fact(DisplayName = nameof(DeleteThrowsWhenCategoryDoesNotExist))]
    [Trait("Integration/Application", "DeleteCategory - Use Cases")]
    public async void DeleteThrowsWhenCategoryDoesNotExist()
    {
        // Given
        var dbContext = _fixture.CreateDbContext();
        var repository = new CategoryRepository(dbContext);
        var unitOfWork = new UnitOfWork(dbContext);
        var exampleList = _fixture.GetExampleCategoriesList(10);
        await dbContext.AddRangeAsync(exampleList);
        dbContext.SaveChanges();

        var input = new UseCase.DeleteCategoryInput(Guid.NewGuid());
        var useCase = new UseCase.DeleteCategory(repository, unitOfWork);

        // When
        var task = async () => await useCase.Handle(input, CancellationToken.None);

        // Then
        await task.Should().ThrowAsync<NotFoundException>().WithMessage($"Category '{input.Id}' not found.");
    }
}