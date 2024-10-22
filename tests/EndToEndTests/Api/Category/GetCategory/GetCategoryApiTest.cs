using System.Net;
using Application.UseCases.Category.Common;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EndToEndTests.Api.Category.GetCategory;

[Collection(nameof(GetCategoryApiTestFixture))]
public class GetCategoryApiTest : IDisposable
{
    private readonly GetCategoryApiTestFixture _fixture;

    public GetCategoryApiTest(GetCategoryApiTestFixture fixture)
    {
        _fixture = fixture;
    }

    public void Dispose()
    {
        _fixture.CleanPercistence();
    }

    [Fact(DisplayName = nameof(GetCategoryOk))]
    [Trait("EndToEnd/API", "GetCategory - Endpoints")]
    public async Task GetCategoryOk()
    {
        // Given
        var exampleCategoryList = _fixture.GetExampleCategoriesList(10);
        await _fixture.Persistence.InsertList(exampleCategoryList);
        var exampleCategory = exampleCategoryList[5];

        // When
        var (response, output) = await _fixture.ApiClient.Get<CategoryModelOutput>($"/categories/{exampleCategory.Id}");

        // Then
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output!.Id.Should().Be(exampleCategory.Id);
        output.Name.Should().Be(exampleCategory.Name);
        output.Description.Should().Be(exampleCategory.Description);
        output.IsActive.Should().Be(exampleCategory.IsActive);
        output.CreatedAt.Should().BeSameDateAs(exampleCategory.CreatedAt);
    }

    [Fact(DisplayName = nameof(NotFoundWhenCategoryDoesNotExist))]
    [Trait("EndToEnd/API", "GetCategory - Endpoints")]
    public async Task NotFoundWhenCategoryDoesNotExist()
    {
        // Given
        var exampleCategoryList = _fixture.GetExampleCategoriesList(10);
        await _fixture.Persistence.InsertList(exampleCategoryList);
        var exampleCategoryId = Guid.NewGuid();

        // When
        var (response, output) = await _fixture.ApiClient.Get<ProblemDetails>($"/categories/{exampleCategoryId}");

        // Then
        response!.StatusCode.Should().Be(HttpStatusCode.NotFound);
        output.Should().NotBeNull();
        output!.Status.Should().Be(StatusCodes.Status404NotFound);
        output.Title.Should().Be("Not Found");
        output.Detail.Should().Be($"Category '{exampleCategoryId}' not found.");
        output.Type.Should().Be("NotFound");
    }
}