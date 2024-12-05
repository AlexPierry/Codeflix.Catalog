using System.Net;
using Api.Models.Response;
using Application.UseCases.Genre.Common;
using FluentAssertions;
using Infra.Data.EF.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Entities = Domain.Entity;

namespace EndToEndTests.Api.Genre;

[Collection(nameof(UpdateGenreApiTestFixture))]
public class UpdateGenreApiTest : IDisposable
{
    private readonly UpdateGenreApiTestFixture _fixture;

    public UpdateGenreApiTest(UpdateGenreApiTestFixture fixture)
    {
        _fixture = fixture;
    }

    public void Dispose()
    {
        _fixture.CleanPercistence();
    }

    [Fact(DisplayName = nameof(UpdateGenreOk))]
    [Trait("EndToEnd/API", "UpdateGenre - Endpoints")]
    public async Task UpdateGenreOk()
    {
        // Given
        var exampleGenreList = _fixture.GetExampleGenresList(10);
        await _fixture.Persistence.InsertList(exampleGenreList);
        var exampleGenre = exampleGenreList[5];
        var input = _fixture.GetExampleInput(exampleGenre.Id, exampleGenre.IsActive);

        // When
        var (response, output) = await _fixture.ApiClient.Put<ApiResponse<GenreModelOutput>>($"/genres/{exampleGenre.Id}", input);

        // Then
        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output!.Data.Should().NotBeNull();
        output.Data.Id.Should().Be(exampleGenre.Id);
        output.Data.Name.Should().Be(input.Name);
        output.Data.IsActive.Should().Be((bool)input.IsActive!);
        output.Data.Catetories.Should().HaveCount(0);

        var persistenceGenre = await _fixture.Persistence.GetById(exampleGenre.Id);
        persistenceGenre.Should().NotBeNull();
        persistenceGenre!.Name.Should().Be(output.Data.Name);
        persistenceGenre.IsActive.Should().Be(output.Data.IsActive!);
        persistenceGenre.Id.Should().Be(output.Data.Id);
        persistenceGenre.CreatedAt.Should().Be(output.Data.CreatedAt);
    }

    [Fact(DisplayName = nameof(NotFoundWhenGenreDoesNotExist))]
    [Trait("EndToEnd/API", "UpdateGenre - Endpoints")]
    public async Task NotFoundWhenGenreDoesNotExist()
    {
        // Given
        var exampleGenreList = _fixture.GetExampleGenresList(10);
        await _fixture.Persistence.InsertList(exampleGenreList);
        var exampleGenre = exampleGenreList[5];
        var randomId = Guid.NewGuid();
        var input = _fixture.GetExampleInput(exampleGenre.Id, exampleGenre.IsActive);

        // When
        var (response, output) = await _fixture.ApiClient.Put<ProblemDetails>($"/genres/{randomId}", input);

        // Then
        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.NotFound);
        output.Should().NotBeNull();
        output!.Status.Should().Be(StatusCodes.Status404NotFound);
        output.Title.Should().Be("Not Found");
        output.Detail.Should().Be($"Genre '{randomId}' not found.");
        output.Type.Should().Be("NotFound");
    }

    [Fact(DisplayName = nameof(UpdateAddingRelations))]
    [Trait("EndToEnd/API", "UpdateGenre - Endpoints")]
    public async Task UpdateAddingRelations()
    {
        // Given
        var categoriesList = _fixture.GetExampleCategoriesList(3);
        await _fixture.CategoryPersistence.InsertList(categoriesList);
        var exampleGenreList = _fixture.GetExampleGenresList(10);
        await _fixture.Persistence.InsertList(exampleGenreList);
        var exampleGenre = exampleGenreList[5];
        var input = _fixture.GetExampleInput(exampleGenre.Id, exampleGenre.IsActive, categoriesList.Select(c => c.Id).ToList());

        // When
        var (response, output) = await _fixture.ApiClient.Put<ApiResponse<GenreModelOutput>>($"/genres/{exampleGenre.Id}", input);

        // Then
        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);

        output.Should().NotBeNull();
        output!.Data.Should().NotBeNull();
        output.Data.Name.Should().Be(input.Name);
        output.Data.IsActive.Should().Be((bool)input.IsActive!);
        output.Data.Id.Should().NotBeEmpty();
        output.Data.CreatedAt.Should().NotBeSameDateAs(default);
        output.Data.Catetories.Should().HaveCount(input.Categories!.Count);
        output.Data.Catetories.Select(c => c.Id).Except(input.Categories).Should().HaveCount(0);

        var dbGenre = await _fixture.Persistence.GetById(output.Data.Id);
        dbGenre.Should().NotBeNull();
        dbGenre!.Name.Should().Be(input.Name);
        dbGenre.IsActive.Should().Be((bool)input.IsActive!);
        dbGenre.Id.Should().NotBeEmpty();
        dbGenre.CreatedAt.Should().BeSameDateAs(output.Data.CreatedAt);

        var dbRelations = await _fixture.GenresCategoriesPersistence.GetByGenreId(output.Data.Id);
        dbRelations.Should().HaveCount(input.Categories.Count);
        dbRelations.Select(r => r.CategoryId).Except(input.Categories).Should().HaveCount(0);
    }

    [Fact(DisplayName = nameof(ErrorWhenRelationDoesNotExist))]
    [Trait("EndToEnd/API", "UpdateGenre - Endpoints")]
    public async Task ErrorWhenRelationDoesNotExist()
    {
        // Given
        var categoriesList = _fixture.GetExampleCategoriesList(3);
        await _fixture.CategoryPersistence.InsertList(categoriesList);
        var exampleGenre = _fixture.GetExampleGenre();
        categoriesList.ForEach(category => exampleGenre.AddCategory(category.Id));
        await _fixture.Persistence.InsertGenre(exampleGenre);
        await _fixture.GenresCategoriesPersistence.InsertList(
            categoriesList.Select(category => new GenresCategories(category.Id, exampleGenre.Id)).ToList());

        var input = _fixture.GetExampleInput(exampleGenre.Id, exampleGenre.IsActive, exampleGenre.Categories.ToList());
        var invalidCategoryId = Guid.NewGuid();
        input.Categories!.Add(invalidCategoryId);

        // When
        var (response, output) = await _fixture.ApiClient.Put<ProblemDetails>($"/genres/{exampleGenre.Id}", input);

        // Then
        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);

        output.Should().NotBeNull();
        output!.Title.Should().Be("Invalid Related Aggregate.");
        output.Type.Should().Be("RelatedAggregate");
        output.Status.Should().Be((int)HttpStatusCode.UnprocessableEntity);
        output.Detail.Should().Be($"Related category id(s) not found: {invalidCategoryId}");
    }

    [Fact(DisplayName = nameof(UpdateOmitingRelations))]
    [Trait("EndToEnd/API", "UpdateGenre - Endpoints")]
    public async Task UpdateOmitingRelations()
    {
        // Given
        var categoriesList = _fixture.GetExampleCategoriesList(3);
        await _fixture.CategoryPersistence.InsertList(categoriesList);
        var exampleGenreList = _fixture.GetExampleGenresList(10);
        var exampleGenre = exampleGenreList[5];
        categoriesList.ForEach(category => exampleGenre.AddCategory(category.Id));
        await _fixture.Persistence.InsertList(exampleGenreList);
        await _fixture.GenresCategoriesPersistence.InsertList(
            categoriesList.Select(c => new GenresCategories(c.Id, exampleGenre.Id)).ToList());
        var input = _fixture.GetExampleInput(exampleGenre.Id, exampleGenre.IsActive);

        // When
        var (response, output) = await _fixture.ApiClient.Put<ApiResponse<GenreModelOutput>>($"/genres/{exampleGenre.Id}", input);

        // Then
        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);

        output.Should().NotBeNull();
        output!.Data.Should().NotBeNull();
        output.Data.Name.Should().Be(input.Name);
        output.Data.IsActive.Should().Be((bool)input.IsActive!);
        output.Data.Id.Should().NotBeEmpty();
        output.Data.CreatedAt.Should().NotBeSameDateAs(default);
        output.Data.Catetories.Should().HaveCount(exampleGenre.Categories.Count);
        output.Data.Catetories.Select(c => c.Id).Except(exampleGenre.Categories).Should().HaveCount(0);

        var dbGenre = await _fixture.Persistence.GetById(output.Data.Id);
        dbGenre.Should().NotBeNull();
        dbGenre!.Name.Should().Be(input.Name);
        dbGenre.IsActive.Should().Be((bool)input.IsActive!);
        dbGenre.Id.Should().NotBeEmpty();
        dbGenre.CreatedAt.Should().BeSameDateAs(output.Data.CreatedAt);

        var dbRelations = await _fixture.GenresCategoriesPersistence.GetByGenreId(output.Data.Id);
        dbRelations.Should().HaveCount(exampleGenre.Categories.Count);
        dbRelations.Select(r => r.CategoryId).Except(exampleGenre.Categories).Should().HaveCount(0);
    }
}