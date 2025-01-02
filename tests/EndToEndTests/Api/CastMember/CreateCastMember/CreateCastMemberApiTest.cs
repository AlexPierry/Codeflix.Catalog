using System.Net;
using Api.Models.Response;
using Application.UseCases.CastMember;
using Application.UseCases.CastMember.Common;
using EndToEndTests.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace EndToEndTests.Api.CastMember;

[Collection(nameof(CastMemberBaseFixture))]
public class CreateCastMemberApiTest : IDisposable
{
    private readonly CastMemberBaseFixture _fixture;

    public CreateCastMemberApiTest(CastMemberBaseFixture fixture)
    {
        _fixture = fixture;
    }

    public void Dispose()
    {
        _fixture.CleanPercistence();
    }

    [Fact(DisplayName = nameof(CreateCastMemberOk))]
    [Trait("EndToEnd/API", "CreateCastMember - Endpoints")]
    public async Task CreateCastMemberOk()
    {
        // Given
        var input = new CreateCastMemberInput(_fixture.GetValidName(), _fixture.GetRandomCastMemberType());

        // When
        var (response, output) = await _fixture.ApiClient.Post<TestApiResponse<CastMemberModelOutput>>("/castmembers", input);

        // Then
        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.Created);

        output.Should().NotBeNull();
        output!.Data.Should().NotBeNull();
        output.Data!.Name.Should().Be(input.Name);
        output.Data.Type.Should().Be(input.Type);
        output.Data.Id.Should().NotBeEmpty();
        output.Data.CreatedAt.Should().NotBeSameDateAs(default);

        var dbCastMember = await _fixture.Persistence.GetById(output.Data.Id);
        dbCastMember.Should().NotBeNull();
        dbCastMember!.Name.Should().Be(input.Name);
        dbCastMember.Type.Should().Be(input.Type);
        dbCastMember.Id.Should().NotBeEmpty();
        dbCastMember.CreatedAt.Should().BeSameDateAs(output.Data.CreatedAt);
    }

    [Fact(DisplayName = nameof(ThrowsErrorWhenInvalidEntity))]
    [Trait("EndToEnd/API", "CreateCastMember - Endpoints")]
    public async Task ThrowsErrorWhenInvalidEntity()
    {
        // Given
        var input = new CreateCastMemberInput("", _fixture.GetRandomCastMemberType());

        // When
        var (response, output) = await _fixture.ApiClient.Post<ProblemDetails>("/castmembers", input);

        // Then
        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);


        output.Should().NotBeNull();
        output!.Title.Should().Be("One or more validation errors occurred.");
        output.Type.Should().Be("UnprocessableEntity");
        output.Status.Should().Be((int)HttpStatusCode.UnprocessableEntity);
        output.Detail.Should().Be("Name should not be null or empty.");
    }
}