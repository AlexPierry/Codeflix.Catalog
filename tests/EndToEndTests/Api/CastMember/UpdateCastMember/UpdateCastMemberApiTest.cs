using System.Net;
using Application.UseCases.CastMember;
using Application.UseCases.CastMember.Common;
using EndToEndTests.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace EndToEndTests.Api.CastMember;

[Collection(nameof(CastMemberBaseFixture))]
public class UpdateCastMemberApiTest : IDisposable
{
    private readonly CastMemberBaseFixture _fixture;

    public UpdateCastMemberApiTest(CastMemberBaseFixture fixture)
    {
        _fixture = fixture;
    }

    public void Dispose()
    {
        _fixture.CleanPercistence();
    }

    [Fact(DisplayName = nameof(UpdateCastMemberOk))]
    [Trait("EndToEnd/API", "UpdateCastMember - Endpoints")]
    public async Task UpdateCastMemberOk()
    {
        // Given
        var castMembersList = _fixture.GetExampleCastMembersList(10);
        var castMember = castMembersList[5];
        await _fixture.Persistence.InsertList(castMembersList);

        var input = new UpdateCastMemberInput(castMember.Id, _fixture.GetValidName(), _fixture.GetRandomCastMemberType());

        // When
        var (response, output) = await _fixture.ApiClient.Put<TestApiResponse<CastMemberModelOutput>>($"/castmembers/{castMember.Id}", input);

        // Then
        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);

        output.Should().NotBeNull();
        output!.Data.Should().NotBeNull();
        output.Data!.Name.Should().Be(input.Name);
        output.Data.Type.Should().Be(input.Type);
        output.Data.Id.Should().Be(castMember.Id);
        output.Data.CreatedAt.Should().BeSameDateAs(castMember.CreatedAt);

        var dbCastMember = await _fixture.Persistence.GetById(castMember.Id);
        dbCastMember.Should().NotBeNull();
        dbCastMember!.Name.Should().Be(input.Name);
        dbCastMember.Type.Should().Be(input.Type);
        dbCastMember.Id.Should().Be(castMember.Id);
        dbCastMember.CreatedAt.Should().BeSameDateAs(castMember.CreatedAt);
    }

    [Fact(DisplayName = nameof(UpdateCastMemberNameOnly))]
    [Trait("EndToEnd/API", "UpdateCastMember - Endpoints")]
    public async Task UpdateCastMemberNameOnly()
    {
        // Given
        var castMembersList = _fixture.GetExampleCastMembersList(10);
        var castMember = castMembersList[5];
        await _fixture.Persistence.InsertList(castMembersList);

        var input = new UpdateCastMemberInput(castMember.Id, _fixture.GetValidName());

        // When
        var (response, output) = await _fixture.ApiClient.Put<TestApiResponse<CastMemberModelOutput>>($"/castmembers/{castMember.Id}", input);

        // Then
        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);

        output.Should().NotBeNull();
        output!.Data.Should().NotBeNull();
        output.Data!.Name.Should().Be(input.Name);
        output.Data.Type.Should().Be(castMember.Type);
        output.Data.Id.Should().Be(castMember.Id);
        output.Data.CreatedAt.Should().BeSameDateAs(castMember.CreatedAt);

        var dbCastMember = await _fixture.Persistence.GetById(castMember.Id);
        dbCastMember.Should().NotBeNull();
        dbCastMember!.Name.Should().Be(input.Name);
        dbCastMember.Type.Should().Be(castMember.Type);
        dbCastMember.Id.Should().Be(castMember.Id);
        dbCastMember.CreatedAt.Should().BeSameDateAs(castMember.CreatedAt);
    }

    [Fact(DisplayName = nameof(ThrowsErrorWhenInvalidEntity))]
    [Trait("EndToEnd/API", "UpdateCastMember - Endpoints")]
    public async Task ThrowsErrorWhenInvalidEntity()
    {
        // Given
        var castMembersList = _fixture.GetExampleCastMembersList(10);
        var castMember = castMembersList[5];
        await _fixture.Persistence.InsertList(castMembersList);

        var input = new UpdateCastMemberInput(castMember.Id, "");

        // When
        var (response, output) = await _fixture.ApiClient.Put<ProblemDetails>($"/castmembers/{castMember.Id}", input);

        // Then
        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);


        output.Should().NotBeNull();
        output!.Title.Should().Be("One or more validation errors occurred.");
        output.Type.Should().Be("UnprocessableEntity");
        output.Status.Should().Be((int)HttpStatusCode.UnprocessableEntity);
        output.Detail.Should().Be("Name should not be null or empty.");
    }

    [Fact(DisplayName = nameof(UpdateCastMemberNotFound))]
    [Trait("EndToEnd/API", "UpdateCastMember - Endpoints")]
    public async Task UpdateCastMemberNotFound()
    {
        // Given
        var id = Guid.NewGuid();
        var input = new UpdateCastMemberInput(id, _fixture.GetValidName(), _fixture.GetRandomCastMemberType());

        // When
        var (response, output) = await _fixture.ApiClient.Put<ProblemDetails>($"/castmembers/{id}", input);

        // Then
        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.NotFound);

        output.Should().NotBeNull();
        output!.Title.Should().Be("Not Found");
        output!.Detail.Should().Be($"CastMember '{id}' not found.");
    }
}