using System.Net;
using Api.Models.Response;
using Application.UseCases.Genre.Common;
using FluentAssertions;
using Infra.Data.EF.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EndToEndTests.Api.Genre;

[Collection(nameof(GetGenreApiTestFixture))]
public class GetGenreApiTest : IDisposable
{
    private readonly GetGenreApiTestFixture _fixture;

    public GetGenreApiTest(GetGenreApiTestFixture fixture)
    {
        _fixture = fixture;
    }

    public void Dispose()
    {
        _fixture.CleanPercistence();
    }

    [Fact(DisplayName = nameof(GetGenreOk))]
    [Trait("EndToEnd/API", "GetGenre - Endpoints")]
    public async Task GetGenreOk()
    {
        // Given
        var exampleGenreList = _fixture.GetExampleGenresList(10);
        await _fixture.Persistence.InsertList(exampleGenreList);
        var exampleGenre = exampleGenreList[5];

        // When
        var (response, output) = await _fixture.ApiClient.Get<ApiResponse<GenreModelOutput>>($"/genres/{exampleGenre.Id}");

        // Then
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output!.Data.Should().NotBeNull();
        output.Data.Id.Should().Be(exampleGenre.Id);
        output.Data.Name.Should().Be(exampleGenre.Name);
        output.Data.IsActive.Should().Be(exampleGenre.IsActive);
        output.Data.CreatedAt.Should().BeSameDateAs(exampleGenre.CreatedAt);
    }

    [Fact(DisplayName = nameof(NotFound))]
    [Trait("EndToEnd/API", "GetGenre - Endpoints")]
    public async Task NotFound()
    {
        // Given
        var exampleGenreList = _fixture.GetExampleGenresList(10);
        await _fixture.Persistence.InsertList(exampleGenreList);
        var exampleGenre = Guid.NewGuid();

        // When
        var (response, output) = await _fixture.ApiClient.Get<ProblemDetails>($"/genres/{exampleGenre}");

        // Then
        response!.StatusCode.Should().Be(HttpStatusCode.NotFound);
        output.Should().NotBeNull();
        output!.Status.Should().Be(StatusCodes.Status404NotFound);
        output.Title.Should().Be("Not Found");
        output.Detail.Should().Be($"Genre '{exampleGenre}' not found.");
        output.Type.Should().Be("NotFound");
    }

    [Fact(DisplayName = nameof(GetGenreWithRelations))]
    [Trait("EndToEnd/API", "GetGenre - Endpoints")]
    public async Task GetGenreWithRelations()
    {
        // Given
        var exampleGenres = _fixture.GetExampleGenresList(10);
        await _fixture.Persistence.InsertList(exampleGenres);
        var exampleCategories = _fixture.GetExampleCategoriesList(10);
        await _fixture.CategoryPersistence.InsertList(exampleCategories);
        exampleGenres.ForEach(async genre =>
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
                    await _fixture.Persistence.InsertGenresCategories(new GenresCategories(categoryId, genre.Id));
                }
            }
        });

        var exampleGenre = exampleGenres[5];

        // When
        var (response, output) = await _fixture.ApiClient.Get<ApiResponse<GenreModelOutput>>($"/genres/{exampleGenre.Id}");

        // Then
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output!.Data.Should().NotBeNull();
        output.Data.Id.Should().Be(exampleGenre.Id);
        output.Data.Name.Should().Be(exampleGenre.Name);
        output.Data.IsActive.Should().Be(exampleGenre.IsActive);
        output.Data.CreatedAt.Should().BeSameDateAs(exampleGenre.CreatedAt);
        output.Data.Catetories.Should().HaveCount(exampleGenre.Categories.Count);
        output.Data.Catetories.Select(c => c.Id).Except(exampleGenre.Categories).Should().HaveCount(0);
    }
}