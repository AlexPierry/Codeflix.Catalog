using Application.Exceptions;
using Domain.Entity;
using Domain.SeedWork.SearchableRepository;
using FluentAssertions;
using Infra.Data.EF;
using Infra.Data.EF.Repositories;

namespace IntegrationTest.Infra.Data.EF.Repositories;

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

        var categoryRepository = new CategoryRepository(_fixture.CreateDbContext(true)); // new dbContext to avoid cache

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

    [Fact(DisplayName = nameof(GetThrowsExceptionWhenNotFound))]
    [Trait("Integration/Infra.Data", "CategoryRepository - Repositories")]
    public async Task GetThrowsExceptionWhenNotFound()
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
        var newDbContext = _fixture.CreateDbContext(true); // to avoid cache
        var dbCategory = await newDbContext.Categories.FindAsync(exampleCategory.Id);

        // Then        
        dbCategory.Should().NotBeNull();
        dbCategory!.Id.Should().Be(exampleCategory.Id);
        dbCategory.Name.Should().Be(exampleCategory.Name);
        dbCategory.Description.Should().Be(exampleCategory.Description);
        dbCategory.IsActive.Should().Be(exampleCategory.IsActive);
        dbCategory.CreatedAt.Should().Be(exampleCategory.CreatedAt);
    }

    [Fact(DisplayName = nameof(DeleteOk))]
    [Trait("Integration/Infra.Data", "CategoryRepository - Repositories")]
    public async Task DeleteOk()
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
        await categoryRepository.Delete(exampleCategory, CancellationToken.None);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var newDbContext = _fixture.CreateDbContext(true); // to avoid cache
        var dbCategory = await newDbContext.Categories.FindAsync(exampleCategory.Id);

        // Then        
        dbCategory.Should().BeNull();
    }

    [Fact(DisplayName = nameof(SearchReturnsListAndTotal))]
    [Trait("Integration/Infra.Data", "CategoryRepository - Repositories")]
    public async Task SearchReturnsListAndTotal()
    {
        // Given
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleCategoriesList = _fixture.GetExampleCategoriesList(15);
        await dbContext.AddRangeAsync(exampleCategoriesList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var categoryRepository = new CategoryRepository(dbContext);
        var searchInput = new SearchInput(1, 20, "", "", SearchOrder.Asc);

        // When
        var output = await categoryRepository.Search(searchInput, CancellationToken.None);

        // Then        
        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.CurrentPage.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(exampleCategoriesList.Count);
        output.Items.Should().HaveCount(exampleCategoriesList.Count);
        foreach (Category outputItem in output.Items)
        {
            var exampleItem = exampleCategoriesList.Find(x => x.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();
            outputItem!.Name.Should().Be(exampleItem!.Name);
            outputItem.Description.Should().Be(exampleItem.Description);
            outputItem.IsActive.Should().Be(exampleItem.IsActive);
            outputItem.CreatedAt.Should().Be(exampleItem.CreatedAt);
        }
    }

    [Fact(DisplayName = nameof(SearchReturnsEmptyWhenPersistenceIsEmpty))]
    [Trait("Integration/Infra.Data", "CategoryRepository - Repositories")]
    public async Task SearchReturnsEmptyWhenPersistenceIsEmpty()
    {
        // Given
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var categoryRepository = new CategoryRepository(dbContext);
        var searchInput = new SearchInput(1, 20, "", "", SearchOrder.Asc);

        // When
        var output = await categoryRepository.Search(searchInput, CancellationToken.None);

        // Then        
        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.CurrentPage.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(0);
        output.Items.Should().HaveCount(0);
    }

    [Theory(DisplayName = nameof(SearchReturnsPaginated))]
    [Trait("Integration/Infra.Data", "CategoryRepository - Repositories")]
    [InlineData(10, 1, 5, 5)]
    [InlineData(10, 2, 5, 5)]
    [InlineData(7, 2, 5, 2)]
    [InlineData(7, 3, 5, 0)]
    public async Task SearchReturnsPaginated(int numberOfCategoriesToGenerate, int page, int perPage, int expectedNumberOfItems)
    {
        // Given
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleCategoriesList = _fixture.GetExampleCategoriesList(numberOfCategoriesToGenerate);
        await dbContext.AddRangeAsync(exampleCategoriesList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var categoryRepository = new CategoryRepository(dbContext);
        var searchInput = new SearchInput(page, perPage, "", "", SearchOrder.Asc);

        // When
        var output = await categoryRepository.Search(searchInput, CancellationToken.None);

        // Then        
        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.CurrentPage.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(numberOfCategoriesToGenerate);
        output.Items.Should().HaveCount(expectedNumberOfItems);
        foreach (Category outputItem in output.Items)
        {
            var exampleItem = exampleCategoriesList.Find(x => x.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();
            outputItem!.Name.Should().Be(exampleItem!.Name);
            outputItem.Description.Should().Be(exampleItem.Description);
            outputItem.IsActive.Should().Be(exampleItem.IsActive);
            outputItem.CreatedAt.Should().Be(exampleItem.CreatedAt);
        }
    }

    [Theory(DisplayName = nameof(SearchByText))]
    [Trait("Integration/Infra.Data", "CategoryRepository - Repositories")]
    [InlineData("Action", 1, 5, 1, 1)]
    [InlineData("Horror", 1, 5, 3, 3)]
    [InlineData("Horror", 2, 5, 0, 3)]
    [InlineData("Sci-fi", 1, 5, 4, 4)]
    [InlineData("Sci-fi", 1, 2, 2, 4)]
    [InlineData("Sci-fi", 2, 3, 1, 4)]
    [InlineData("Romance", 1, 5, 0, 0)]
    [InlineData("Robots", 1, 5, 2, 2)]
    public async Task SearchByText(string search, int page, int perPage, int expectedNumberOfItems, int expectedTotalItems)
    {
        // Given
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext(false, Guid.NewGuid().ToString());
        var exampleCategoriesList = _fixture.GetExampleCategoriesListWithNames(new List<string>(){
            "Action",
            "Horror",
            "Horror - Robots",
            "Horror - Based on Real Facts",
            "Drama",
            "Sci-fi IA",
            "Sci-fi Robots",
            "Sci-fi Space",
            "Sci-fi Future",
        });
        await dbContext.AddRangeAsync(exampleCategoriesList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var categoryRepository = new CategoryRepository(dbContext);
        var searchInput = new SearchInput(page, perPage, search, "", SearchOrder.Asc);

        // When
        var output = await categoryRepository.Search(searchInput, CancellationToken.None);

        // Then        
        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.CurrentPage.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(expectedTotalItems);
        output.Items.Should().HaveCount(expectedNumberOfItems);
        foreach (Category outputItem in output.Items)
        {
            var exampleItem = exampleCategoriesList.Find(x => x.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();
            outputItem!.Name.Should().Be(exampleItem!.Name);
            outputItem.Description.Should().Be(exampleItem.Description);
            outputItem.IsActive.Should().Be(exampleItem.IsActive);
            outputItem.CreatedAt.Should().Be(exampleItem.CreatedAt);
        }
    }

    [Theory(DisplayName = nameof(SearchOrdered))]
    [Trait("Integration/Infra.Data", "CategoryRepository - Repositories")]
    [InlineData("name", "asc")]
    [InlineData("name", "desc")]
    [InlineData("id", "asc")]
    [InlineData("id", "desc")]
    [InlineData("createdAt", "asc")]
    [InlineData("createdAt", "desc")]
    [InlineData("", "asc")]
    public async Task SearchOrdered(string orderBy, string order)
    {
        // Given
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleCategoriesList = _fixture.GetExampleCategoriesList();
        await dbContext.AddRangeAsync(exampleCategoriesList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var categoryRepository = new CategoryRepository(dbContext);
        var searchOrder = order.ToLower() == "asc" ? SearchOrder.Asc : SearchOrder.Desc;
        var searchInput = new SearchInput(1, 20, "", orderBy, searchOrder);
        var expectedOrderedList = _fixture.CloneCategoryListOrdered(exampleCategoriesList, orderBy, searchOrder);

        // When
        var output = await categoryRepository.Search(searchInput, CancellationToken.None);

        // Then
        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.CurrentPage.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(exampleCategoriesList.Count());
        output.Items.Should().HaveCount(exampleCategoriesList.Count);
        for (int index = 0; index < expectedOrderedList.Count(); index++)
        {
            var expecetedItem = expectedOrderedList[index];
            var outputItem = output.Items[index];
            expecetedItem.Should().NotBeNull();
            outputItem.Should().NotBeNull();
            outputItem!.Id.Should().Be(expecetedItem!.Id);
            outputItem.Name.Should().Be(expecetedItem.Name);
            outputItem.Description.Should().Be(expecetedItem.Description);
            outputItem.IsActive.Should().Be(expecetedItem.IsActive);
            outputItem.CreatedAt.Should().Be(expecetedItem.CreatedAt);
        }
    }
}