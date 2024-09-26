using System.Net;
using Application.UseCases.Category.Common;
using Application.UseCases.Category.UpdateCategory;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EndToEndTests.Api.Category.UpdateCategory;

[Collection(nameof(UpdateCategoryApiTestFixture))]
public class UpdateCategoryApiTest : IDisposable
{
    private readonly UpdateCategoryApiTestFixture _fixture;

    public UpdateCategoryApiTest(UpdateCategoryApiTestFixture fixture)
    {
        _fixture = fixture;
    }

    public void Dispose()
    {
        _fixture.CleanPercistence();
    }

    [Fact(DisplayName = nameof(UpdateCategoryOk))]
    [Trait("EndToEnd/API", "UpdateCategory - Endpoints")]
    public async Task UpdateCategoryOk()
    {
        // Given
        var exampleCategoryList = _fixture.GetExampleCategoriesList(10);
        await _fixture.Persistence.InsertList(exampleCategoryList);
        var exampleCategory = exampleCategoryList[5];
        var input = _fixture.GetExampleInput(exampleCategory.Id);

        // When
        var (response, output) = await _fixture.ApiClient.Put<CategoryModelOutput>($"/categories/{exampleCategory.Id}", input);

        // Then
        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output!.Id.Should().Be(input.Id);
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(input.Description);
        output.IsActive.Should().Be((bool)input.IsActive!);

        var percistenceCategory = await _fixture.Persistence.GetById(input.Id);
        percistenceCategory.Should().NotBeNull();
        percistenceCategory!.Name.Should().Be(output.Name);
        percistenceCategory.Description.Should().Be(output.Description);
        percistenceCategory.IsActive.Should().Be(output.IsActive!);
        percistenceCategory.Id.Should().Be(output.Id);
        percistenceCategory.CreatedAt.Should().Be(output.CreatedAt);
    }

    [Fact(DisplayName = nameof(UpdateCategoryOnlyName))]
    [Trait("EndToEnd/API", "UpdateCategory - Endpoints")]
    public async Task UpdateCategoryOnlyName()
    {
        // Given
        var exampleCategoryList = _fixture.GetExampleCategoriesList(10);
        await _fixture.Persistence.InsertList(exampleCategoryList);
        var exampleCategory = exampleCategoryList[5];
        var input = new UpdateCategoryInput(exampleCategory.Id, _fixture.GetValidCategoryName());

        // When
        var (response, output) = await _fixture.ApiClient.Put<CategoryModelOutput>($"/categories/{exampleCategory.Id}", input);

        // Then
        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output!.Id.Should().Be(input.Id);
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(exampleCategory.Description);
        output.IsActive.Should().Be(exampleCategory.IsActive!);

        var percistenceCategory = await _fixture.Persistence.GetById(input.Id);
        percistenceCategory.Should().NotBeNull();
        percistenceCategory!.Name.Should().Be(output.Name);
        percistenceCategory.Description.Should().Be(exampleCategory.Description);
        percistenceCategory.IsActive.Should().Be(exampleCategory.IsActive!);
        percistenceCategory.Id.Should().Be(exampleCategory.Id);
    }

    [Fact(DisplayName = nameof(UpdateCategoryNameAndDescription))]
    [Trait("EndToEnd/API", "UpdateCategory - Endpoints")]
    public async Task UpdateCategoryNameAndDescription()
    {
        // Given
        var exampleCategoryList = _fixture.GetExampleCategoriesList(10);
        await _fixture.Persistence.InsertList(exampleCategoryList);
        var exampleCategory = exampleCategoryList[5];
        var input = new UpdateCategoryInput(exampleCategory.Id, _fixture.GetValidCategoryName(), _fixture.GetValidCategoryDescription());

        // When
        var (response, output) = await _fixture.ApiClient.Put<CategoryModelOutput>($"/categories/{exampleCategory.Id}", input);

        // Then
        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output!.Id.Should().Be(input.Id);
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(input.Description);
        output.IsActive.Should().Be(exampleCategory.IsActive!);

        var percistenceCategory = await _fixture.Persistence.GetById(input.Id);
        percistenceCategory.Should().NotBeNull();
        percistenceCategory!.Name.Should().Be(output.Name);
        percistenceCategory.Description.Should().Be(input.Description);
        percistenceCategory.IsActive.Should().Be(exampleCategory.IsActive!);
        percistenceCategory.Id.Should().Be(exampleCategory.Id);
    }

    [Fact(DisplayName = nameof(NotFoundWhenCategoryDoesNotExist))]
    [Trait("EndToEnd/API", "UpdateCategory - Endpoints")]
    public async Task NotFoundWhenCategoryDoesNotExist()
    {
        // Given
        var exampleCategoryList = _fixture.GetExampleCategoriesList(10);
        await _fixture.Persistence.InsertList(exampleCategoryList);
        var input = _fixture.GetExampleInput(Guid.NewGuid());

        // When
        var (response, output) = await _fixture.ApiClient.Put<ProblemDetails>($"/categories/{input.Id}", input);

        // Then
        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.NotFound);
        output.Should().NotBeNull();
        output!.Status.Should().Be(StatusCodes.Status404NotFound);
        output.Title.Should().Be("Not Found");
        output.Detail.Should().Be($"Category '{input.Id}' not found.");
        output.Type.Should().Be("NotFound");
    }

    [Theory(DisplayName = nameof(ErrorWhenCantInstantiateAggregate))]
    [Trait("EndToEnd/API", "CreateCategory - Endpoints")]
    [MemberData(
        nameof(UpdateCategoryApiTestDataGenerator.GetInvalidInputs),
        MemberType = typeof(UpdateCategoryApiTestDataGenerator))]
    public async Task ErrorWhenCantInstantiateAggregate(UpdateCategoryInput invalidInput, string expectedDetail)
    {
        // Given
        var exampleCategoryList = _fixture.GetExampleCategoriesList(10);
        await _fixture.Persistence.InsertList(exampleCategoryList);
        var exampleCategory = exampleCategoryList[5];
        invalidInput.Id = exampleCategory.Id;

        // When
        var (response, output) = await _fixture.ApiClient.Put<ProblemDetails>($"/categories/{invalidInput.Id}", invalidInput);

        // Then
        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);

        output.Should().NotBeNull();
        output!.Title.Should().Be("One or more validation errors occurred.");
        output.Type.Should().Be("UnprocessableEntity");
        output.Status.Should().Be((int)HttpStatusCode.UnprocessableEntity);
        output.Detail.Should().Be(expectedDetail);
    }
}