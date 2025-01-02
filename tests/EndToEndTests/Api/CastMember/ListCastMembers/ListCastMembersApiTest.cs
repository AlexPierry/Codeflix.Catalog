using System.Net;
using Api.Models.Response;
using Application.UseCases.CastMember;
using Application.UseCases.CastMember.Common;
using Domain.SeedWork.SearchableRepository;
using EndToEndTests.Models;
using FluentAssertions;

namespace EndToEndTests.Api.CastMember;

[Collection(nameof(CastMemberBaseFixture))]
public class ListCastMembersApiTest : IDisposable
{
    private readonly CastMemberBaseFixture _fixture;

    public ListCastMembersApiTest(CastMemberBaseFixture fixture)
    {
        _fixture = fixture;
    }

    public void Dispose()
    {
        _fixture.CleanPercistence();
    }

    [Fact(DisplayName = nameof(ListCastMembersOk))]
    [Trait("EndToEnd/API", "ListCastMembers - Endpoints")]
    public async Task ListCastMembersOk()
    {
        // Given
        var castMembersList = _fixture.GetExampleCastMembersList(10);
        await _fixture.Persistence.InsertList(castMembersList);

        // When
        var (response, output) = await _fixture.ApiClient.Get<ApiResponse<List<CastMemberModelOutput>>>("/castmembers");

        // Then
        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);

        output.Should().NotBeNull();
        output!.Data.Should().NotBeNull();
        output.Data.Should().HaveCount(castMembersList.Count);
        output.Data.Should().BeEquivalentTo(castMembersList, options => options.Excluding(x => x.CreatedAt));
    }

    [Fact(DisplayName = nameof(ListCastMembersEmpty))]
    [Trait("EndToEnd/API", "ListCastMembers - Endpoints")]
    public async Task ListCastMembersEmpty()
    {
        // When
        var (response, output) = await _fixture.ApiClient.Get<ApiResponse<List<CastMemberModelOutput>>>("/castmembers");

        // Then
        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);

        output.Should().NotBeNull();
        output!.Data.Should().NotBeNull();
        output.Data.Should().BeEmpty();
    }

    [Theory(DisplayName = nameof(ListCastMembersPaginated))]
    [Trait("EndToEnd/API", "ListCastMembers - Endpoints")]
    [InlineData(10, 1, 5, 5)]
    [InlineData(10, 2, 5, 5)]
    [InlineData(7, 2, 5, 2)]
    [InlineData(7, 3, 5, 0)]
    public async Task ListCastMembersPaginated(int numberOfCastMembersToGenerate, int page, int perPage, int expectedNumberOfItems)
    {
        // Given
        var exampleCastMemberList = _fixture.GetExampleCastMembersList(numberOfCastMembersToGenerate);
        await _fixture.Persistence.InsertList(exampleCastMemberList);
        var input = new ListCastMembersInput(page, perPage);

        // When
        var (response, output) = await _fixture.ApiClient.Get<TestApiResponseList<CastMemberModelOutput>>("/castmembers", input);

        // Then        
        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output!.Meta.Should().NotBeNull();
        output.Data.Should().NotBeNull();
        output.Meta!.Total.Should().Be(numberOfCastMembersToGenerate);
        output.Meta.CurrentPage.Should().Be(input.Page);
        output.Meta.PerPage.Should().Be(input.PerPage);
        output.Data.Should().HaveCount(expectedNumberOfItems);
        foreach (var item in output.Data!)
        {
            var exampleItem = exampleCastMemberList.FirstOrDefault(i => i.Id == item.Id);
            exampleItem.Should().NotBeNull();
            exampleItem!.Name.Should().Be(item.Name);
            exampleItem.Type.Should().Be(item.Type);
            exampleItem.CreatedAt.Should().BeSameDateAs(item.CreatedAt);
        }
    }

    [Theory(DisplayName = nameof(SearchByText))]
    [Trait("EndToEnd/API", "Category/List - Endpoints")]
    [InlineData("Action", 1, 5, 1, 1)]
    [InlineData("Horror", 1, 5, 3, 3)]
    [InlineData("Horror", 2, 5, 0, 3)]
    [InlineData("Sci-fi", 1, 5, 4, 4)]
    [InlineData("Sci-fi", 1, 2, 2, 4)]
    [InlineData("Sci-fi", 2, 3, 1, 4)]
    [InlineData("Romance", 1, 5, 0, 0)]
    [InlineData("Robots", 1, 5, 2, 2)]
    public async Task SearchByText(string search, int page, int perPage, int expectedNumberOfItems, int expectedTotalItems)
    {
        // Given
        var exampleCastMemberList = _fixture.GetExampleCastMembersListWithNames(new List<string>(){
            "Action",
            "Horror",
            "Horror - Robots",
            "Horror - Based on Real Facts",
            "Drama",
            "Sci-fi IA",
            "Sci-fi Robots",
            "Sci-fi Space",
            "Sci-fi Future",
        });

        await _fixture.Persistence.InsertList(exampleCastMemberList);
        var input = new ListCastMembersInput(page, perPage, search);

        // When
        var (response, output) = await _fixture.ApiClient.Get<TestApiResponseList<CastMemberModelOutput>>("/castmembers", input);

        // Then        
        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output!.Meta.Should().NotBeNull();
        output.Data.Should().NotBeNull();
        output.Meta!.Total.Should().Be(expectedTotalItems);
        output.Meta.CurrentPage.Should().Be(input.Page);
        output.Meta.PerPage.Should().Be(input.PerPage);
        output.Data.Should().HaveCount(expectedNumberOfItems);
        foreach (var item in output.Data!)
        {
            var exampleItem = exampleCastMemberList.FirstOrDefault(i => i.Id == item.Id);
            exampleItem.Should().NotBeNull();
            exampleItem!.Name.Should().Be(item.Name);
            exampleItem.Type.Should().Be(item.Type);
            exampleItem.CreatedAt.Should().BeSameDateAs(item.CreatedAt);
        }
    }

    [Theory(DisplayName = nameof(SearchOrdered))]
    [Trait("Integration/Application", "ListCategories - Use Cases")]
    [InlineData("name", "asc")]
    [InlineData("name", "desc")]
    [InlineData("id", "asc")]
    [InlineData("id", "desc")]
    [InlineData("createdAt", "asc")]
    [InlineData("createdAt", "desc")]
    [InlineData("", "asc")]
    public async Task SearchOrdered(string orderBy, string order)
    {
        // Given
        var exampleCastMemberList = _fixture.GetExampleCastMembersList();
        await _fixture.Persistence.InsertList(exampleCastMemberList);
        var searchOrder = order.ToLower() == "asc" ? SearchOrder.Asc : SearchOrder.Desc;
        var input = new ListCastMembersInput(1, 20, "", orderBy, searchOrder);

        // When
        var (response, output) = await _fixture.ApiClient.Get<TestApiResponseList<CastMemberModelOutput>>("/castmembers", input);

        // Then
        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output!.Meta.Should().NotBeNull();
        output.Data.Should().NotBeNull();
        output.Meta!.CurrentPage.Should().Be(input.Page);
        output.Meta.PerPage.Should().Be(input.PerPage);
        output.Meta.Total.Should().Be(exampleCastMemberList.Count);
        output.Data.Should().HaveCount(exampleCastMemberList.Count);

        var expectedOrderedList = _fixture.CloneCastMembersListOrdered(exampleCastMemberList, orderBy, searchOrder);
        for (int index = 0; index < expectedOrderedList.Count(); index++)
        {
            var expecetedItem = expectedOrderedList[index];
            var outputItem = output.Data![index];
            expecetedItem.Should().NotBeNull();
            outputItem.Should().NotBeNull();
            outputItem!.Id.Should().Be(expecetedItem!.Id);
            outputItem.Name.Should().Be(expecetedItem.Name);
            outputItem.Type.Should().Be(expecetedItem.Type);
            outputItem.CreatedAt.Should().BeSameDateAs(expecetedItem.CreatedAt);
        }
    }
}