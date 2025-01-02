using System.Net;
using Api.Models.Response;
using Application.UseCases.CastMember.Common;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace EndToEndTests.Api.CastMember;

[Collection(nameof(CastMemberBaseFixture))]
public class GetCastMemberApiTest : IDisposable
{
    private readonly CastMemberBaseFixture _fixture;

    public GetCastMemberApiTest(CastMemberBaseFixture fixture)
    {
        _fixture = fixture;
    }

    public void Dispose()
    {
        _fixture.CleanPercistence();
    }

    [Fact(DisplayName = nameof(GetCastMemberOk))]
    [Trait("EndToEnd/API", "GetCastMember - Endpoints")]
    public async Task GetCastMemberOk()
    {
        // Given
        var castMembersList = _fixture.GetExampleCastMembersList(10);
        var castMember = castMembersList[5];
        await _fixture.Persistence.InsertList(castMembersList);

        // When
        var (response, output) = await _fixture.ApiClient.Get<ApiResponse<CastMemberModelOutput>>($"/castmembers/{castMember.Id}");

        // Then
        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);

        output.Should().NotBeNull();
        output!.Data.Should().NotBeNull();
        output.Data.Name.Should().Be(castMember.Name);
        output.Data.Type.Should().Be(castMember.Type);
        output.Data.Id.Should().Be(castMember.Id);
        output.Data.CreatedAt.Should().BeSameDateAs(castMember.CreatedAt);
    }

    [Fact(DisplayName = nameof(GetCastMemberNotFound))]
    [Trait("EndToEnd/API", "GetCastMember - Endpoints")]
    public async Task GetCastMemberNotFound()
    {
        // Given
        var id = Guid.NewGuid();

        // When
        var (response, output) = await _fixture.ApiClient.Get<ProblemDetails>($"/castmembers/{id}");

        // Then
        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.NotFound);

        output.Should().NotBeNull();
        output!.Detail.Should().Be($"CastMember '{id}' not found.");
    }
}