using System.Net;
using FluentAssertions;
using Infra.Data.EF.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EndToEndTests.Api.Genre;

[Collection(nameof(DeleteGenreApiTestFixture))]
public class DeleteGenreApiTest : IDisposable
{
    private readonly DeleteGenreApiTestFixture _fixture;

    public DeleteGenreApiTest(DeleteGenreApiTestFixture fixture)
    {
        _fixture = fixture;
    }

    public void Dispose()
    {
        _fixture.CleanPercistence();
    }

    [Fact(DisplayName = nameof(DeleteGenreOk))]
    [Trait("EndToEnd/API", "DeleteGenre - Endpoints")]
    public async Task DeleteGenreOk()
    {
        // Given
        var exampleGenreList = _fixture.GetExampleGenresList(10);
        await _fixture.Persistence.InsertList(exampleGenreList);
        var exampleGenre = exampleGenreList[5];

        // When
        var (response, output) = await _fixture.ApiClient.Delete<object>($"/genres/{exampleGenre.Id}");

        // Then
        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.NoContent);
        output.Should().BeNull();
        var percistenceGenre = await _fixture.Persistence.GetById(exampleGenre.Id);
        percistenceGenre.Should().BeNull();
    }

    [Fact(DisplayName = nameof(NotFoundWhenGenreDoesNotExist))]
    [Trait("EndToEnd/API", "DeleteGenre - Endpoints")]
    public async Task NotFoundWhenGenreDoesNotExist()
    {
        // Given
        var exampleGenreList = _fixture.GetExampleGenresList(10);
        await _fixture.Persistence.InsertList(exampleGenreList);
        var exampleGenreId = Guid.NewGuid();

        // When
        var (response, output) = await _fixture.ApiClient.Delete<ProblemDetails>($"/genres/{exampleGenreId}");

        // Then
        response!.StatusCode.Should().Be(HttpStatusCode.NotFound);
        output.Should().NotBeNull();
        output!.Status.Should().Be(StatusCodes.Status404NotFound);
        output.Title.Should().Be("Not Found");
        output.Detail.Should().Be($"Genre '{exampleGenreId}' not found.");
        output.Type.Should().Be("NotFound");
    }

    [Fact(DisplayName = nameof(DeleteGenreWithRelations))]
    [Trait("EndToEnd/API", "DeleteGenre - Endpoints")]
    public async Task DeleteGenreWithRelations()
    {
        // Given
        var exampleGenres = _fixture.GetExampleGenresList(10);
        await _fixture.Persistence.InsertList(exampleGenres);
        var exampleCategories = _fixture.GetExampleCategoriesList(10);
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
        var exampleGenre = exampleGenres[5];

        // When
        var (response, output) = await _fixture.ApiClient.Delete<object>($"/genres/{exampleGenre.Id}");

        // Then
        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.NoContent);
        output.Should().BeNull();
        var percistenceGenre = await _fixture.Persistence.GetById(exampleGenre.Id);
        percistenceGenre.Should().BeNull();
        var persistenceRelations = await _fixture.GenresCategoriesPersistence.GetByGenreId(exampleGenre.Id);
        persistenceRelations.Should().HaveCount(0);
    }
}