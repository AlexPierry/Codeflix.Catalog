using System.Net;
using Application.UseCases.Category.Common;
using Application.UseCases.Category.CreateCategory;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace EndToEndTests.Api.Category.CreateCategory;

[Collection(nameof(CreateCategoryApiTestFixture))]
public class CreateCategoryApiTest
{
    private readonly CreateCategoryApiTestFixture _fixture;

    public CreateCategoryApiTest(CreateCategoryApiTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(CreateCategoryOk))]
    [Trait("EndToEnd/API", "CreateCategory - Endpoints")]
    public async Task CreateCategoryOk()
    {
        // Given
        var input = _fixture.GetExampleInput();

        // When
        var (response, output) = await _fixture.ApiClient.Post<CategoryModelOutput>("/categories", input);

        // Then
        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.Created);

        output.Should().NotBeNull();
        output!.Name.Should().Be(input.Name);
        output.Description.Should().Be(input.Description);
        output.IsActive.Should().Be(input.IsActive);
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBeSameDateAs(default);

        var dbCategory = await _fixture.Persistence.GetById(output.Id);
        dbCategory.Should().NotBeNull();
        dbCategory!.Name.Should().Be(input.Name);
        dbCategory.Description.Should().Be(input.Description);
        dbCategory.IsActive.Should().Be(input.IsActive);
        dbCategory.Id.Should().NotBeEmpty();
        dbCategory.CreatedAt.Should().Be(output.CreatedAt);
    }

    [Theory(DisplayName = nameof(ThrowsWhenCantInstantiateAggregate))]
    [Trait("EndToEnd/API", "CreateCategory - Endpoints")]
    [MemberData(
        nameof(CreateCategoryApiTestDataGenerator.GetInvalidInputs),
        MemberType = typeof(CreateCategoryApiTestDataGenerator))]
    public async Task ThrowsWhenCantInstantiateAggregate(CreateCategoryInput invalidInput, string expectedDetail)
    {
        // Given

        // When
        var (response, output) = await _fixture.ApiClient.Post<ProblemDetails>("/categories", invalidInput);

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