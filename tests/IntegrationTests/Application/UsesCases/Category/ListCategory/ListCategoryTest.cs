using Application.UseCases.Category.Common;
using Application.UseCases.Category.ListCategories;
using Domain.SeedWork.SearchableRepository;
using FluentAssertions;
using Infra.Data.EF.Repositories;

namespace IntegrationTest.Application.UseCases.Category;

[Collection(nameof(ListCategoriesTestFixture))]
public class ListCategoryTest
{
    private readonly ListCategoriesTestFixture _fixture;

    public ListCategoryTest(ListCategoriesTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(SearchReturnsListAndTotal))]
    [Trait("Integration/Application", "ListCategories - Use Cases")]
    public async void SearchReturnsListAndTotal()
    {
        // Given
        var dbContext = _fixture.CreateDbContext();
        var repository = new CategoryRepository(dbContext);
        var categoriesExampleList = _fixture.GetExampleCategoriesList(10);
        await dbContext.AddRangeAsync(categoriesExampleList);
        await dbContext.SaveChangesAsync();

        var input = new ListCategoriesInput(1, 20);

        var useCase = new ListCategories(repository);

        // When
        var output = await useCase.Handle(input, CancellationToken.None);

        // Then
        output.Should().NotBeNull();
        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Total.Should().Be(categoriesExampleList.Count);
        output.Items.Should().HaveCount(categoriesExampleList.Count);
        ((List<CategoryModelOutput>)output.Items).ForEach(outputItem =>
        {
            var repositoryCategory = categoriesExampleList.First(x => x.Id == outputItem.Id);
            outputItem.Should().NotBeNull();
            outputItem.Id.Should().Be(repositoryCategory.Id);
            outputItem.Name.Should().Be(repositoryCategory.Name);
            outputItem.Description.Should().Be(repositoryCategory.Description);
            outputItem.IsActive.Should().Be(repositoryCategory.IsActive);
            outputItem.CreatedAt.Should().Be(repositoryCategory.CreatedAt);
        });
    }

    [Fact(DisplayName = nameof(SearchReturnsEmptyWhenEmpty))]
    [Trait("Integration/Application", "ListCategories - Use Cases")]
    public async void SearchReturnsEmptyWhenEmpty()
    {
        // Given
        var dbContext = _fixture.CreateDbContext();
        var repository = new CategoryRepository(dbContext);
        var categoriesExampleList = _fixture.GetExampleCategoriesList(0);
        await dbContext.AddRangeAsync(categoriesExampleList);
        await dbContext.SaveChangesAsync();

        var input = new ListCategoriesInput(1, 20);

        var useCase = new ListCategories(repository);

        // When
        var output = await useCase.Handle(input, CancellationToken.None);

        // Then
        output.Should().NotBeNull();
        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Total.Should().Be(0);
        output.Items.Should().HaveCount(0);
    }


    [Theory(DisplayName = nameof(SearchReturnsPaginated))]
    [Trait("Integration/Application", "ListCategories - Use Cases")]
    [InlineData(10, 1, 5, 5)]
    [InlineData(10, 2, 5, 5)]
    [InlineData(7, 2, 5, 2)]
    [InlineData(7, 3, 5, 0)]
    public async Task SearchReturnsPaginated(int numberOfCategoriesToGenerate, int page, int perPage, int expectedNumberOfItems)
    {
        // Given
        var dbContext = _fixture.CreateDbContext();
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
        output.Total.Should().Be(exampleCategoriesList.Count);
        output.Items.Should().HaveCount(expectedNumberOfItems);
        foreach (var outputItem in output.Items)
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
    [Trait("Integration/Application", "ListCategories - Use Cases")]
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
        var dbContext = _fixture.CreateDbContext(false, Guid.NewGuid().ToString());
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
        var input = new ListCategoriesInput(page, perPage, search, "", SearchOrder.Asc);
        var useCase = new ListCategories(categoryRepository);

        // When
        var output = await useCase.Handle(input, CancellationToken.None);

        // Then        
        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Total.Should().Be(expectedTotalItems);
        output.Items.Should().HaveCount(expectedNumberOfItems);
        foreach (var outputItem in output.Items)
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
    [Trait("Integration/Application", "ListCategories - Use Cases")]
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
        var dbContext = _fixture.CreateDbContext();
        var exampleCategoriesList = _fixture.GetExampleCategoriesList();
        await dbContext.AddRangeAsync(exampleCategoriesList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var categoryRepository = new CategoryRepository(dbContext);
        var searchOrder = order.ToLower() == "asc" ? SearchOrder.Asc : SearchOrder.Desc;
        var input = new ListCategoriesInput(1, 20, "", orderBy, searchOrder);
        var expectedOrderedList = _fixture.CloneCategoryListOrdered(exampleCategoriesList, orderBy, searchOrder);
        var useCase = new ListCategories(categoryRepository);

        // When
        var output = await useCase.Handle(input, CancellationToken.None);

        // Then
        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
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