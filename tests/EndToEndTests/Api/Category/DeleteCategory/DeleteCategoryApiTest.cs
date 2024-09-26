using System.Net;
using Application.UseCases.Category.Common;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EndToEndTests.Api.Category.DeleteCategory;

[Collection(nameof(DeleteCategoryApiTestFixture))]
public class DeleteCategoryApiTest : IDisposable
{
    private readonly DeleteCategoryApiTestFixture _fixture;

    public DeleteCategoryApiTest(DeleteCategoryApiTestFixture fixture)
    {
        _fixture = fixture;
    }

    public void Dispose()
    {
        _fixture.CleanPercistence();
    }

    [Fact(DisplayName = nameof(DeleteCategoryOk))]
    [Trait("EndToEnd/API", "DeleteCategory - Endpoints")]
    public async Task DeleteCategoryOk()
    {
        // Given
        var exampleCategoryList = _fixture.GetExampleCategoriesList(10);
        await _fixture.Persistence.InsertList(exampleCategoryList);
        var exampleCategory = exampleCategoryList[5];

        // When
        var (response, output) = await _fixture.ApiClient.Delete<object>($"/categories/{exampleCategory.Id}");

        // Then
        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.NoContent);
        output.Should().BeNull();
        var percistenceCategory = await _fixture.Persistence.GetById(exampleCategory.Id);
        percistenceCategory.Should().BeNull();
    }

    [Fact(DisplayName = nameof(NotFoundWhenCategoryDoesNotExist))]
    [Trait("EndToEnd/API", "DeleteCategory - Endpoints")]
    public async Task NotFoundWhenCategoryDoesNotExist()
    {
        // Given
        var exampleCategoryList = _fixture.GetExampleCategoriesList(10);
        await _fixture.Persistence.InsertList(exampleCategoryList);
        var exampleCategoryId = Guid.NewGuid();

        // When
        var (response, output) = await _fixture.ApiClient.Delete<ProblemDetails>($"/categories/{exampleCategoryId}");

        // Then
        response!.StatusCode.Should().Be(HttpStatusCode.NotFound);
        output.Should().NotBeNull();
        output!.Status.Should().Be(StatusCodes.Status404NotFound);
        output.Title.Should().Be("Not Found");
        output.Detail.Should().Be($"Category '{exampleCategoryId}' not found.");
        output.Type.Should().Be("NotFound");
    }
}