using Entities = Domain.Entity;
using Domain.Repository;
using Domain.SeedWork.SearchableRepository;
using Moq;
using Application.UseCases.Video.ListVideos;
using FluentAssertions;

namespace UnitTests.Application.Video;

[Collection(nameof(ListVideosTestFixture))]
public class ListVideosTest
{
    private readonly ListVideosTestFixture _fixture;

    public ListVideosTest(ListVideosTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(ListVideoOk))]
    [Trait("Application", "ListVideo - Uses Cases")]
    public async Task ListVideoOk()
    {
        // Arrange
        var repositoryMock = new Mock<IVideoRepository>();
        var videosExampleList = _fixture.GetValidVideoList(15);
        var input = _ = _fixture.GetExampleInput();
        var outputRepositorySearch = new SearchOutput<Entities.Video>(
            currentPage: input.Page,
            perPage: input.PerPage,
            items: videosExampleList,
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

        var usecase = new ListVideos(repositoryMock.Object);

        // Act
        var output = await usecase.Handle(input, CancellationToken.None);

        // Assert
        output.Should().NotBeNull();
        output.Page.Should().Be(outputRepositorySearch.CurrentPage);
        output.PerPage.Should().Be(outputRepositorySearch.PerPage);
        output.Total.Should().Be(outputRepositorySearch.Total);
        output.Items.Should().HaveCount(outputRepositorySearch.Items.Count);
        foreach (var outputItem in output.Items)
        {
            var repositoryVideo = outputRepositorySearch.Items.First(x => x.Id == outputItem.Id);
            outputItem.Should().NotBeNull();
            outputItem.Id.Should().Be(repositoryVideo.Id);
            outputItem.Title.Should().Be(repositoryVideo.Title);
            outputItem.Description.Should().Be(repositoryVideo.Description);
            outputItem.Duration.Should().Be(repositoryVideo.Duration);
            outputItem.Rating.Should().Be(repositoryVideo.MovieRating);
            outputItem.Published.Should().Be(repositoryVideo.Published);
            outputItem.Opened.Should().Be(repositoryVideo.Opened);
            outputItem.CreatedAt.Should().Be(repositoryVideo.CreatedAt);
            outputItem.CategoriesIds.Should().BeEquivalentTo(repositoryVideo.Categories);
            outputItem.GenresIds.Should().BeEquivalentTo(repositoryVideo.Genres);
            outputItem.CastMembersIds.Should().BeEquivalentTo(repositoryVideo.CastMembers);
            outputItem.Thumb.Should().Be(repositoryVideo.Thumb?.Path);
            outputItem.Banner.Should().Be(repositoryVideo.Banner?.Path);
            outputItem.ThumbHalf.Should().Be(repositoryVideo.ThumbHalf?.Path);
            outputItem.Media.Should().Be(repositoryVideo.Media?.FilePath);
            outputItem.Trailer.Should().Be(repositoryVideo.Trailer?.FilePath);
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
    [Trait("Application", "ListVideos - Use Cases")]
    public async void ListOkWhenEmpty()
    {
        // Arrange
        var repositoryMock = new Mock<IVideoRepository>();
        var input = _fixture.GetExampleInput();
        var outputRepositorySearch = new SearchOutput<Entities.Video>(
            currentPage: input.Page,
            perPage: input.PerPage,
            items: new List<Entities.Video>(),
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

        var usecase = new ListVideos(repositoryMock.Object);

        // Act
        var output = await usecase.Handle(input, CancellationToken.None);

        // Assert
        output.Should().NotBeNull();
        output.Page.Should().Be(outputRepositorySearch.CurrentPage);
        output.PerPage.Should().Be(outputRepositorySearch.PerPage);
        output.Total.Should().Be(outputRepositorySearch.Total);
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
