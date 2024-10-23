using System.Net;
using Api.Models.Response;
using Application.UseCases.Category.Common;
using Application.UseCases.Category.ListCategories;
using Domain.SeedWork.SearchableRepository;
using EndToEndTests.Models;
using FluentAssertions;

namespace EndToEndTests.Api.Category.ListCategories;

[Collection(nameof(ListCategoriesApiTestFixture))]
public class ListCategoriesApiTest : IDisposable
{
    private readonly ListCategoriesApiTestFixture _fixture;

    public ListCategoriesApiTest(ListCategoriesApiTestFixture fixture)
    {
        _fixture = fixture;
    }

    public void Dispose()
    {
        _fixture.CleanPercistence();
    }

    [Fact(DisplayName = nameof(ListCategoriesAndTotalDefault))]
    [Trait("EndToEnd/API", "Category/List - Endpoints")]
    public async Task ListCategoriesAndTotalDefault()
    {
        // Given
        var defaultPageSize = 15;
        var exampleCategoryList = _fixture.GetExampleCategoriesList(20);
        await _fixture.Persistence.InsertList(exampleCategoryList);

        // When
        var (response, output) = await _fixture.ApiClient.Get<TestApiResponseList<CategoryModelOutput>>("/categories");

        // Then
        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output!.Meta.Should().NotBeNull();
        output.Data.Should().NotBeNull();
        output.Meta!.Total.Should().Be(exampleCategoryList.Count);
        output.Meta.CurrentPage.Should().Be(1);
        output.Meta.PerPage.Should().Be(defaultPageSize);
        output.Data.Should().HaveCount(defaultPageSize);
        foreach (var item in output.Data!)
        {
            var exampleItem = exampleCategoryList.FirstOrDefault(i => i.Id == item.Id);
            exampleItem.Should().NotBeNull();
            exampleItem!.Name.Should().Be(item.Name);
            exampleItem.Description.Should().Be(item.Description);
            exampleItem.IsActive.Should().Be(item.IsActive);
            exampleItem.CreatedAt.Should().BeSameDateAs(item.CreatedAt);
        }
    }

    [Fact(DisplayName = nameof(NoItemsWhenPersistenceIsEmpty))]
    [Trait("EndToEnd/API", "Category/List - Endpoints")]
    public async Task NoItemsWhenPersistenceIsEmpty()
    {
        // Given

        // When
        var (response, output) = await _fixture.ApiClient.Get<TestApiResponseList<CategoryModelOutput>>("/categories");

        // Then
        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output!.Meta.Should().NotBeNull();
        output.Data.Should().NotBeNull();
        output.Meta!.Total.Should().Be(0);
        output.Data.Should().HaveCount(0);
    }

    [Fact(DisplayName = nameof(ListCategoriesAndTotal))]
    [Trait("EndToEnd/API", "Category/List - Endpoints")]
    public async Task ListCategoriesAndTotal()
    {
        // Given
        var exampleCategoryList = _fixture.GetExampleCategoriesList(20);
        await _fixture.Persistence.InsertList(exampleCategoryList);
        var input = new ListCategoriesInput(page: 1, perPage: 5);

        // When
        var (response, output) = await _fixture.ApiClient.Get<TestApiResponseList<CategoryModelOutput>>("/categories", input);

        // Then
        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output!.Meta.Should().NotBeNull();
        output.Data.Should().NotBeNull();
        output.Meta!.Total.Should().Be(exampleCategoryList.Count);
        output.Meta.CurrentPage.Should().Be(input.Page);
        output.Meta.PerPage.Should().Be(input.PerPage);
        output.Data.Should().HaveCount(input.PerPage);
        foreach (var item in output.Data!)
        {
            var exampleItem = exampleCategoryList.FirstOrDefault(i => i.Id == item.Id);
            exampleItem.Should().NotBeNull();
            exampleItem!.Name.Should().Be(item.Name);
            exampleItem.Description.Should().Be(item.Description);
            exampleItem.IsActive.Should().Be(item.IsActive);
            exampleItem.CreatedAt.Should().BeSameDateAs(item.CreatedAt);
        }
    }

    [Theory(DisplayName = nameof(ListPaginated))]
    [Trait("EndToEnd/API", "Category/List - Endpoints")]
    [InlineData(10, 1, 5, 5)]
    [InlineData(10, 2, 5, 5)]
    [InlineData(7, 2, 5, 2)]
    [InlineData(7, 3, 5, 0)]
    public async Task ListPaginated(int numberOfCategoriesToGenerate, int page, int perPage, int expectedNumberOfItems)
    {
        // Given
        var exampleCategoryList = _fixture.GetExampleCategoriesList(numberOfCategoriesToGenerate);
        await _fixture.Persistence.InsertList(exampleCategoryList);
        var input = new ListCategoriesInput(page, perPage);

        // When
        var (response, output) = await _fixture.ApiClient.Get<TestApiResponseList<CategoryModelOutput>>("/categories", input);

        // Then        
        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output!.Meta.Should().NotBeNull();
        output.Data.Should().NotBeNull();
        output.Meta!.Total.Should().Be(numberOfCategoriesToGenerate);
        output.Meta.CurrentPage.Should().Be(input.Page);
        output.Meta.PerPage.Should().Be(input.PerPage);
        output.Data.Should().HaveCount(expectedNumberOfItems);
        foreach (var item in output.Data!)
        {
            var exampleItem = exampleCategoryList.FirstOrDefault(i => i.Id == item.Id);
            exampleItem.Should().NotBeNull();
            exampleItem!.Name.Should().Be(item.Name);
            exampleItem.Description.Should().Be(item.Description);
            exampleItem.IsActive.Should().Be(item.IsActive);
            exampleItem.CreatedAt.Should().BeSameDateAs(item.CreatedAt);
        }
    }

    [Theory(DisplayName = nameof(SearchByText))]
    [Trait("EndToEnd/API", "Category/List - Endpoints")]
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
        var exampleCategoryList = _fixture.GetExampleCategoriesListWithNames(new List<string>(){
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

        await _fixture.Persistence.InsertList(exampleCategoryList);
        var input = new ListCategoriesInput(page, perPage, search);

        // When
        var (response, output) = await _fixture.ApiClient.Get<TestApiResponseList<CategoryModelOutput>>("/categories", input);

        // Then        
        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output!.Meta.Should().NotBeNull();
        output.Data.Should().NotBeNull();
        output.Meta!.Total.Should().Be(expectedTotalItems);
        output.Meta.CurrentPage.Should().Be(input.Page);
        output.Meta.PerPage.Should().Be(input.PerPage);
        output.Data.Should().HaveCount(expectedNumberOfItems);
        foreach (var item in output.Data!)
        {
            var exampleItem = exampleCategoryList.FirstOrDefault(i => i.Id == item.Id);
            exampleItem.Should().NotBeNull();
            exampleItem!.Name.Should().Be(item.Name);
            exampleItem.Description.Should().Be(item.Description);
            exampleItem.IsActive.Should().Be(item.IsActive);
            exampleItem.CreatedAt.Should().BeSameDateAs(item.CreatedAt);
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
        var exampleCategoryList = _fixture.GetExampleCategoriesList();
        await _fixture.Persistence.InsertList(exampleCategoryList);
        var searchOrder = order.ToLower() == "asc" ? SearchOrder.Asc : SearchOrder.Desc;
        var input = new ListCategoriesInput(1, 20, "", orderBy, searchOrder);

        // When
        var (response, output) = await _fixture.ApiClient.Get<TestApiResponseList<CategoryModelOutput>>("/categories", input);

        // Then
        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output!.Meta.Should().NotBeNull();
        output.Data.Should().NotBeNull();
        output.Meta!.CurrentPage.Should().Be(input.Page);
        output.Meta.PerPage.Should().Be(input.PerPage);
        output.Meta.Total.Should().Be(exampleCategoryList.Count);
        output.Data.Should().HaveCount(exampleCategoryList.Count);

        var expectedOrderedList = _fixture.CloneCategoryListOrdered(exampleCategoryList, orderBy, searchOrder);
        for (int index = 0; index < expectedOrderedList.Count(); index++)
        {
            var expecetedItem = expectedOrderedList[index];
            var outputItem = output.Data![index];
            expecetedItem.Should().NotBeNull();
            outputItem.Should().NotBeNull();
            outputItem!.Id.Should().Be(expecetedItem!.Id);
            outputItem.Name.Should().Be(expecetedItem.Name);
            outputItem.Description.Should().Be(expecetedItem.Description);
            outputItem.IsActive.Should().Be(expecetedItem.IsActive);
            outputItem.CreatedAt.Should().BeSameDateAs(expecetedItem.CreatedAt);
        }
    }

}