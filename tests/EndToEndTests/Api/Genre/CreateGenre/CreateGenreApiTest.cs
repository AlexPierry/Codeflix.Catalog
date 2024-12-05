using System.Net;
using Api.Models.Response;
using Application.UseCases.Genre.Common;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace EndToEndTests.Api.Genre;

[Collection(nameof(CreateGenreApiTestFixture))]
public class CreateGenreApiTest : IDisposable
{
    private readonly CreateGenreApiTestFixture _fixture;

    public CreateGenreApiTest(CreateGenreApiTestFixture fixture)
    {
        _fixture = fixture;
    }

    public void Dispose()
    {
        _fixture.CleanPercistence();
    }

    [Fact(DisplayName = nameof(CreateGenreOk))]
    [Trait("EndToEnd/API", "CreateGenre - Endpoints")]
    public async Task CreateGenreOk()
    {
        // Given
        var input = _fixture.GetExampleInput();

        // When
        var (response, output) = await _fixture.ApiClient.Post<ApiResponse<GenreModelOutput>>("/genres", input);

        // Then
        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.Created);

        output.Should().NotBeNull();
        output!.Data.Should().NotBeNull();
        output.Data.Name.Should().Be(input.Name);
        output.Data.IsActive.Should().Be(input.IsActive);
        output.Data.Id.Should().NotBeEmpty();
        output.Data.CreatedAt.Should().NotBeSameDateAs(default);
        output.Data.Catetories.Should().HaveCount(0);

        var dbGenre = await _fixture.Persistence.GetById(output.Data.Id);
        dbGenre.Should().NotBeNull();
        dbGenre!.Name.Should().Be(input.Name);
        dbGenre.IsActive.Should().Be(input.IsActive);
        dbGenre.Id.Should().NotBeEmpty();
        dbGenre.CreatedAt.Should().BeSameDateAs(output.Data.CreatedAt);
        dbGenre.Categories.Should().HaveCount(0);
    }

    [Fact(DisplayName = nameof(CreateWithRelations))]
    [Trait("EndToEnd/API", "CreateGenre - Endpoints")]
    public async Task CreateWithRelations()
    {
        var categoriesList = _fixture.GetExampleCategoriesList(3);
        await _fixture.CategoryPersistence.InsertList(categoriesList);

        // Given
        var input = _fixture.GetExampleInputWithCategories(categoriesList.Select(c => c.Id).ToList());

        // When
        var (response, output) = await _fixture.ApiClient.Post<ApiResponse<GenreModelOutput>>("/genres", input);

        // Then
        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.Created);

        output.Should().NotBeNull();
        output!.Data.Should().NotBeNull();
        output.Data.Name.Should().Be(input.Name);
        output.Data.IsActive.Should().Be(input.IsActive);
        output.Data.Id.Should().NotBeEmpty();
        output.Data.CreatedAt.Should().NotBeSameDateAs(default);
        output.Data.Catetories.Should().HaveCount(input.Categories!.Count);
        output.Data.Catetories.Select(c => c.Id).Except(input.Categories).Should().HaveCount(0);

        var dbGenre = await _fixture.Persistence.GetById(output.Data.Id);
        dbGenre.Should().NotBeNull();
        dbGenre!.Name.Should().Be(input.Name);
        dbGenre.IsActive.Should().Be(input.IsActive);
        dbGenre.Id.Should().NotBeEmpty();
        dbGenre.CreatedAt.Should().BeSameDateAs(output.Data.CreatedAt);

        var dbRelations = await _fixture.GenresCategoriesPersistence.GetByGenreId(output.Data.Id);
        dbRelations.Should().HaveCount(input.Categories.Count);
        dbRelations.Select(r => r.CategoryId).Except(input.Categories).Should().HaveCount(0);
    }

    [Fact(DisplayName = nameof(ErrorWhenRelationDoesNotExist))]
    [Trait("EndToEnd/API", "CreateGenre - Endpoints")]
    public async Task ErrorWhenRelationDoesNotExist()
    {
        var categoriesList = _fixture.GetExampleCategoriesList(3);
        await _fixture.CategoryPersistence.InsertList(categoriesList);

        // Given
        var input = _fixture.GetExampleInputWithCategories(categoriesList.Select(c => c.Id).ToList());
        var invalidCategoryId = Guid.NewGuid();
        input.Categories!.Add(invalidCategoryId);

        // When
        var (response, output) = await _fixture.ApiClient.Post<ProblemDetails>("/genres", input);

        // Then
        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);

        output.Should().NotBeNull();
        output!.Title.Should().Be("Invalid Related Aggregate.");
        output.Type.Should().Be("RelatedAggregate");
        output.Status.Should().Be((int)HttpStatusCode.UnprocessableEntity);
        output.Detail.Should().Be($"Related category id(s) not found: {invalidCategoryId}");
    }
}