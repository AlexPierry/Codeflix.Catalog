using Application.Exceptions;
using FluentAssertions;
using Infra.Data.EF;
using Infra.Data.EF.Repositories;
using Microsoft.EntityFrameworkCore;

namespace IntegrationTest.Infra.Data.EF.Repositories.CategoryRepositories;

[Collection(nameof(CategoryRepositoriesTestFixture))]
public class CategoryRepositoriesTest
{
    private CategoryRepositoriesTestFixture _fixture;

    public CategoryRepositoriesTest(CategoryRepositoriesTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(InsertOk))]
    [Trait("Integration/Infra.Data", "CategoryRepository - Repositories")]
    public async Task InsertOk()
    {
        // Given
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleCategory = _fixture.GetExampleCategory();
        var categoryRepository = new CategoryRepository(dbContext);

        // When
        await categoryRepository.Insert(exampleCategory, CancellationToken.None);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        // Then
        var dbCategory = await dbContext.Categories.FindAsync(exampleCategory.Id);
        dbCategory.Should().NotBeNull();
        dbCategory!.Name.Should().Be(exampleCategory.Name);
        dbCategory.Description.Should().Be(exampleCategory.Description);
        dbCategory.IsActive.Should().Be(exampleCategory.IsActive);
        dbCategory.CreatedAt.Should().Be(exampleCategory.CreatedAt);
    }

    [Fact(DisplayName = nameof(GetOk))]
    [Trait("Integration/Infra.Data", "CategoryRepository - Repositories")]
    public async Task GetOk()
    {
        // Given
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleCategory = _fixture.GetExampleCategory();
        var exampleCategoriesList = _fixture.GetExampleCategoriesList(15);
        exampleCategoriesList.Add(exampleCategory);
        await dbContext.AddRangeAsync(exampleCategoriesList);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var categoryRepository = new CategoryRepository(dbContext);

        // When
        var dbCategory = await categoryRepository.Get(exampleCategory.Id, CancellationToken.None);

        // Then        
        dbCategory.Should().NotBeNull();
        dbCategory!.Id.Should().Be(exampleCategory.Id);
        dbCategory.Name.Should().Be(exampleCategory.Name);
        dbCategory.Description.Should().Be(exampleCategory.Description);
        dbCategory.IsActive.Should().Be(exampleCategory.IsActive);
        dbCategory.CreatedAt.Should().Be(exampleCategory.CreatedAt);
    }

    [Fact(DisplayName = nameof(GetThrowExceptionWhenNotFound))]
    [Trait("Integration/Infra.Data", "CategoryRepository - Repositories")]
    public async Task GetThrowExceptionWhenNotFound()
    {
        // Given
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleId = Guid.NewGuid();
        await dbContext.AddRangeAsync(_fixture.GetExampleCategoriesList(15));
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var categoryRepository = new CategoryRepository(dbContext);

        // When
        var task = async () => await categoryRepository.Get(exampleId, CancellationToken.None);

        // Then        
        await task.Should().ThrowAsync<NotFoundException>($"Category '{exampleId}' not found.");
    }

    [Fact(DisplayName = nameof(UpdateOk))]
    [Trait("Integration/Infra.Data", "CategoryRepository - Repositories")]
    public async Task UpdateOk()
    {
        // Given
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleCategory = _fixture.GetExampleCategory();
        var exampleCategoriesList = _fixture.GetExampleCategoriesList(15);
        exampleCategoriesList.Add(exampleCategory);
        await dbContext.AddRangeAsync(exampleCategoriesList);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var categoryRepository = new CategoryRepository(dbContext);
        var newCategoryValues = _fixture.GetExampleCategory();
        exampleCategory.Update(newCategoryValues.Name, newCategoryValues.Description);

        // When
        await categoryRepository.Update(exampleCategory, CancellationToken.None);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var dbCategory = await dbContext.Categories.FindAsync(exampleCategory.Id);

        // Then        
        dbCategory.Should().NotBeNull();
        dbCategory!.Id.Should().Be(exampleCategory.Id);
        dbCategory.Name.Should().Be(exampleCategory.Name);
        dbCategory.Description.Should().Be(exampleCategory.Description);
        dbCategory.IsActive.Should().Be(exampleCategory.IsActive);
        dbCategory.CreatedAt.Should().Be(exampleCategory.CreatedAt);
    }
}