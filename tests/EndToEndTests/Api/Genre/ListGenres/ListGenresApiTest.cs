using System.Net;
using Api.Models.Response;
using Application.UseCases.Genre.Common;
using Application.UseCases.Genre.ListGenres;
using Domain.SeedWork.SearchableRepository;
using EndToEndTests.Models;
using FluentAssertions;
using Infra.Data.EF.Models;

namespace EndToEndTests.Api.Genre;

[Collection(nameof(ListGenresApiTestFixture))]
public class ListGenresApiTest : IDisposable
{
    private readonly ListGenresApiTestFixture _fixture;

    public ListGenresApiTest(ListGenresApiTestFixture fixture)
    {
        _fixture = fixture;
    }

    public void Dispose()
    {
        _fixture.CleanPercistence();
    }

    [Fact(DisplayName = nameof(ListGenresAndTotal))]
    [Trait("EndToEnd/API", "Genre/List - Endpoints")]
    public async Task ListGenresAndTotal()
    {
        // Given
        var defaultPageSize = 15;
        var exampleGenreList = _fixture.GetExampleGenresList(20);
        await _fixture.Persistence.InsertList(exampleGenreList);

        // When
        var (response, output) = await _fixture.ApiClient.Get<TestApiResponseList<GenreModelOutput>>("/genres");

        // Then
        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output!.Meta.Should().NotBeNull();
        output.Data.Should().NotBeNull();
        output.Meta!.Total.Should().Be(exampleGenreList.Count);
        output.Meta.CurrentPage.Should().Be(1);
        output.Meta.PerPage.Should().Be(defaultPageSize);
        output.Data.Should().HaveCount(defaultPageSize);
        foreach (var item in output.Data!)
        {
            var exampleItem = exampleGenreList.FirstOrDefault(i => i.Id == item.Id);
            exampleItem.Should().NotBeNull();
            exampleItem!.Name.Should().Be(item.Name);
            exampleItem.IsActive.Should().Be(item.IsActive);
            exampleItem.CreatedAt.Should().BeSameDateAs(item.CreatedAt);
            exampleItem.Categories.Should().HaveCount(0);
        }
    }

    [Fact(DisplayName = nameof(NoItemsWhenPersistenceIsEmpty))]
    [Trait("EndToEnd/API", "Genre/List - Endpoints")]
    public async Task NoItemsWhenPersistenceIsEmpty()
    {
        // Given

        // When
        var (response, output) = await _fixture.ApiClient.Get<TestApiResponseList<GenreModelOutput>>("/genres");

        // Then
        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output!.Meta.Should().NotBeNull();
        output.Data.Should().NotBeNull();
        output.Meta!.Total.Should().Be(0);
        output.Data.Should().HaveCount(0);
    }

    [Fact(DisplayName = nameof(ListGenresWithRelations))]
    [Trait("EndToEnd/API", "GetGenre - Endpoints")]
    public async Task ListGenresWithRelations()
    {
        // Given
        var defaultPageSize = 15;
        var exampleGenres = _fixture.GetExampleGenresList(20);
        await _fixture.Persistence.InsertList(exampleGenres);
        var exampleCategories = _fixture.GetExampleCategoriesList(15);
        await _fixture.CategoryPersistence.InsertList(exampleCategories);
        var genresCategoriesList = new List<GenresCategories>();
        exampleGenres.ForEach(genre =>
        {
            var random = new Random();
            var startPoint = random.Next(0, exampleCategories.Count);
            var itemsToTake = random.Next(0, 4);
            for (int i = startPoint; i < exampleCategories.Count; i++)
            {
                if (itemsToTake > 0)
                {
                    var categoryId = exampleCategories[i].Id;
                    genre.AddCategory(categoryId);
                    itemsToTake--;
                    genresCategoriesList.Add(new GenresCategories(categoryId, genre.Id));
                }
            }
        });

        await _fixture.GenresCategoriesPersistence.InsertList(genresCategoriesList);

        // When
        var (response, output) = await _fixture.ApiClient.Get<TestApiResponseList<GenreModelOutput>>($"/genres");

        // Then
        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output!.Meta.Should().NotBeNull();
        output.Data.Should().NotBeNull();
        output.Meta!.Total.Should().Be(exampleGenres.Count);
        output.Meta.CurrentPage.Should().Be(1);
        output.Meta.PerPage.Should().Be(defaultPageSize);
        output.Data.Should().HaveCount(defaultPageSize);
        foreach (var item in output.Data!)
        {
            var exampleItem = exampleGenres.FirstOrDefault(i => i.Id == item.Id);
            exampleItem.Should().NotBeNull();
            exampleItem!.Name.Should().Be(item.Name);
            exampleItem.IsActive.Should().Be(item.IsActive);
            exampleItem.CreatedAt.Should().BeSameDateAs(item.CreatedAt);
            exampleItem.Categories.Should().HaveCount(item.Catetories.Count);
            item.Catetories.Select(c => c.Id).ToList().Should().BeEquivalentTo(exampleItem.Categories);
            item.Catetories.ToList().ForEach(category =>
            {
                var exampleCategory = exampleCategories.FirstOrDefault(c => c.Id == category.Id);
                category.Name.Should().Be(exampleCategory!.Name);
            });
        }
    }

    [Theory(DisplayName = nameof(ListPaginated))]
    [Trait("EndToEnd/API", "Genre/List - Endpoints")]
    [InlineData(10, 1, 5, 5)]
    [InlineData(10, 2, 5, 5)]
    [InlineData(7, 2, 5, 2)]
    [InlineData(7, 3, 5, 0)]
    public async Task ListPaginated(int numberOfGenresToGenerate, int page, int perPage, int expectedNumberOfItems)
    {
        // Given
        var exampleGenreList = _fixture.GetExampleGenresList(numberOfGenresToGenerate);
        await _fixture.Persistence.InsertList(exampleGenreList);
        var input = new ListGenresInput(page, perPage);

        // When
        var (response, output) = await _fixture.ApiClient.Get<TestApiResponseList<GenreModelOutput>>("/genres", input);

        // Then        
        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output!.Meta.Should().NotBeNull();
        output.Data.Should().NotBeNull();
        output.Meta!.Total.Should().Be(numberOfGenresToGenerate);
        output.Meta.CurrentPage.Should().Be(input.Page);
        output.Meta.PerPage.Should().Be(input.PerPage);
        output.Data.Should().HaveCount(expectedNumberOfItems);
        foreach (var item in output.Data!)
        {
            var exampleItem = exampleGenreList.FirstOrDefault(i => i.Id == item.Id);
            exampleItem.Should().NotBeNull();
            exampleItem!.Name.Should().Be(item.Name);
            exampleItem.IsActive.Should().Be(item.IsActive);
            exampleItem.CreatedAt.Should().BeSameDateAs(item.CreatedAt);
        }
    }

    [Theory(DisplayName = nameof(SearchByText))]
    [Trait("EndToEnd/API", "Genre/List - Endpoints")]
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
        var exampleCategoryList = _fixture.GetExampleGenresListWithNames(new List<string>(){
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
        var input = new ListGenresInput(page, perPage, search);

        // When
        var (response, output) = await _fixture.ApiClient.Get<TestApiResponseList<GenreModelOutput>>("/genres", input);

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
            exampleItem.IsActive.Should().Be(item.IsActive);
            exampleItem.CreatedAt.Should().BeSameDateAs(item.CreatedAt);
        }
    }

    [Theory(DisplayName = nameof(SearchOrdered))]
    [Trait("Integration/Application", "Genre/List - Use Cases")]
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
        var exampleGenreList = _fixture.GetExampleGenresList();
        await _fixture.Persistence.InsertList(exampleGenreList);
        var searchOrder = order.ToLower() == "asc" ? SearchOrder.Asc : SearchOrder.Desc;
        var input = new ListGenresInput(1, 20, "", orderBy, searchOrder);

        // When
        var (response, output) = await _fixture.ApiClient.Get<TestApiResponseList<GenreModelOutput>>("/genres", input);

        // Then
        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output!.Meta.Should().NotBeNull();
        output.Data.Should().NotBeNull();
        output.Meta!.CurrentPage.Should().Be(input.Page);
        output.Meta.PerPage.Should().Be(input.PerPage);
        output.Meta.Total.Should().Be(exampleGenreList.Count);
        output.Data.Should().HaveCount(exampleGenreList.Count);

        var expectedOrderedList = _fixture.CloneGenreListOrdered(exampleGenreList, orderBy, searchOrder);
        for (int index = 0; index < expectedOrderedList.Count(); index++)
        {
            var expecetedItem = expectedOrderedList[index];
            var outputItem = output.Data![index];
            expecetedItem.Should().NotBeNull();
            outputItem.Should().NotBeNull();
            outputItem!.Id.Should().Be(expecetedItem!.Id);
            outputItem.Name.Should().Be(expecetedItem.Name);
            outputItem.IsActive.Should().Be(expecetedItem.IsActive);
            outputItem.CreatedAt.Should().BeSameDateAs(expecetedItem.CreatedAt);
        }
    }
}