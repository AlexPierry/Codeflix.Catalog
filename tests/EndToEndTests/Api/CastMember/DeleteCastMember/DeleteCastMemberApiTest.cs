using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace EndToEndTests.Api.CastMember;

[Collection(nameof(CastMemberBaseFixture))]
public class DeleteCastMemberApiTest : IDisposable
{
    private readonly CastMemberBaseFixture _fixture;

    public DeleteCastMemberApiTest(CastMemberBaseFixture fixture)
    {
        _fixture = fixture;
    }

    public void Dispose()
    {
        _fixture.CleanPercistence();
    }

    [Fact(DisplayName = nameof(DeleteCastMemberOk))]
    [Trait("EndToEnd/API", "DeleteCastMember - Endpoints")]
    public async Task DeleteCastMemberOk()
    {
        // Given
        var castMembersList = _fixture.GetExampleCastMembersList(10);
        var castMember = castMembersList[5];
        await _fixture.Persistence.InsertList(castMembersList);

        // When
        var (response, output) = await _fixture.ApiClient.Delete<object>($"/castmembers/{castMember.Id}");

        // Then
        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.NoContent);
        output.Should().BeNull();
        var dbCastMember = await _fixture.Persistence.GetById(castMember.Id);
        dbCastMember.Should().BeNull();
    }

    [Fact(DisplayName = nameof(DeleteCastMemberNotFound))]
    [Trait("EndToEnd/API", "DeleteCastMember - Endpoints")]
    public async Task DeleteCastMemberNotFound()
    {
        // Given
        var id = Guid.NewGuid();

        // When
        var (response, output) = await _fixture.ApiClient.Delete<ProblemDetails>($"/castmembers/{id}");

        // Then
        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.NotFound);

        output.Should().NotBeNull();
        output!.Detail.Should().Be($"CastMember '{id}' not found.");
    }
}