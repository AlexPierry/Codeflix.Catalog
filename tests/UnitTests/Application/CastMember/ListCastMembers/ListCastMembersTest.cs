using Application.UseCases.CastMember;
using Application.UseCases.CastMember.Common;
using Domain.SeedWork.SearchableRepository;
using FluentAssertions;
using Moq;
using Entities = Domain.Entity;

namespace UnitTests.Application.CastMember;

[Collection(nameof(ListCastMembersTestFixture))]
public class ListCastMemberTest
{
    private readonly ListCastMembersTestFixture _fixture;

    public ListCastMemberTest(ListCastMembersTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(ListOk))]
    [Trait("Application", "ListCastMembers - Use Cases")]
    public async void ListOk()
    {
        // Given
        var castMembersExampleList = _fixture.GetExampleCastMembersList();
        var repositoryMock = _fixture.GetRepositoryMock();
        var input = _fixture.GetExampleInput();
        var outputRepositorySearch = new SearchOutput<Entities.CastMember>(
            currentPage: input.Page,
            perPage: input.PerPage,
            items: castMembersExampleList,
            total: new Random().Next(50, 200)
        );

        repositoryMock.Setup(x => x.Search(
            It.Is<SearchInput>(searchInput =>
                searchInput.Page == input.Page
                && searchInput.PerPage == input.PerPage
                && searchInput.Search == input.Search
                && searchInput.OrderBy == input.Sort
                && searchInput.Order == input.Dir),
            It.IsAny<CancellationToken>())).ReturnsAsync(outputRepositorySearch);

        var useCase = new ListCastMembers(repositoryMock.Object);

        // When
        var output = await useCase.Handle(input, CancellationToken.None);

        // Then
        output.Should().NotBeNull();
        output.Page.Should().Be(outputRepositorySearch.CurrentPage);
        output.PerPage.Should().Be(outputRepositorySearch.PerPage);
        output.Total.Should().Be(outputRepositorySearch.Total);
        output.Items.Should().HaveCount(outputRepositorySearch.Items.Count);
        foreach (var outputItem in output.Items)
        {
            var repositoryCastMember = outputRepositorySearch.Items.First(x => x.Id == outputItem.Id);
            outputItem.Should().NotBeNull();
            outputItem.Id.Should().Be(repositoryCastMember.Id);
            outputItem.Name.Should().Be(repositoryCastMember.Name);
            outputItem.Type.Should().Be(repositoryCastMember.Type);
            outputItem.CreatedAt.Should().Be(repositoryCastMember.CreatedAt);
        }

        repositoryMock.Verify(x => x.Search(
                It.Is<SearchInput>(searchInput =>
                searchInput.Page == input.Page
                && searchInput.PerPage == input.PerPage
                && searchInput.Search == input.Search
                && searchInput.OrderBy == input.Sort
                && searchInput.Order == input.Dir),
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [Fact(DisplayName = nameof(ListOkWhenEmpty))]
    [Trait("Application", "ListCastMembers - Use Cases")]
    public async void ListOkWhenEmpty()
    {
        // Given        
        var repositoryMock = _fixture.GetRepositoryMock();
        var input = _fixture.GetExampleInput();
        var outputRepositorySearch = new SearchOutput<Entities.CastMember>(
            currentPage: input.Page,
            perPage: input.PerPage,
            items: new List<Entities.CastMember>().AsReadOnly(),
            total: 0
        );

        repositoryMock.Setup(x => x.Search(
            It.Is<SearchInput>(searchInput =>
                searchInput.Page == input.Page
                && searchInput.PerPage == input.PerPage
                && searchInput.Search == input.Search
                && searchInput.OrderBy == input.Sort
                && searchInput.Order == input.Dir),
            It.IsAny<CancellationToken>())).ReturnsAsync(outputRepositorySearch);

        var useCase = new ListCastMembers(repositoryMock.Object);

        // When
        var output = await useCase.Handle(input, CancellationToken.None);

        // Then
        output.Should().NotBeNull();
        output.Page.Should().Be(outputRepositorySearch.CurrentPage);
        output.PerPage.Should().Be(outputRepositorySearch.PerPage);
        output.Total.Should().Be(0);
        output.Items.Should().HaveCount(0);

        repositoryMock.Verify(x => x.Search(
                It.Is<SearchInput>(searchInput =>
                searchInput.Page == input.Page
                && searchInput.PerPage == input.PerPage
                && searchInput.Search == input.Search
                && searchInput.OrderBy == input.Sort
                && searchInput.Order == input.Dir),
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }
}