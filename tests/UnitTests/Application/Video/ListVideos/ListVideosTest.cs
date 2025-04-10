using Entities = Domain.Entity;
using Domain.Repository;
using Domain.SeedWork.SearchableRepository;
using Moq;
using Application.UseCases.Video.ListVideos;
using FluentAssertions;
using Domain.Extensions;

namespace UnitTests.Application.Video;

[Collection(nameof(ListVideosTestFixture))]
public class ListVideosTest
{
    private readonly ListVideosTestFixture _fixture;
    private readonly Mock<IVideoRepository> _videoRepositoryMock;
    private readonly Mock<ICategoryRepository> _categoryRepository;
    private readonly Mock<IGenreRepository> _genreRepository;
    private readonly Mock<ICastMemberRepository> _castMemberRepository;
    private readonly ListVideos _listVideosUseCase;

    public ListVideosTest(ListVideosTestFixture fixture)
    {
        _fixture = fixture;
        _videoRepositoryMock = new Mock<IVideoRepository>();
        _categoryRepository = new Mock<ICategoryRepository>();
        _genreRepository = new Mock<IGenreRepository>();
        _castMemberRepository = new Mock<ICastMemberRepository>();
        _listVideosUseCase = new ListVideos(
            _videoRepositoryMock.Object,
            _categoryRepository.Object,
            _genreRepository.Object,
            _castMemberRepository.Object);
    }

    [Fact(DisplayName = nameof(ListVideoOk))]
    [Trait("Application", "ListVideo - Uses Cases")]
    public async Task ListVideoOk()
    {
        // Arrange
        var examples = _fixture.GetValidVideoListAndAggregates(15);
        var input = _ = _fixture.GetExampleInput();
        var outputRepositorySearch = new SearchOutput<Entities.Video>(
            currentPage: input.Page,
            perPage: input.PerPage,
            items: examples.videos,
            total: new Random().Next(50, 200)
        );
        _videoRepositoryMock.Setup(x => x.Search(
            It.Is<SearchInput>(searchInput =>
                searchInput.Page == input.Page
                && searchInput.PerPage == input.PerPage
                && searchInput.Search == input.Search
                && searchInput.OrderBy == input.Sort
                && searchInput.Order == input.Dir),
            It.IsAny<CancellationToken>())).ReturnsAsync(outputRepositorySearch);

        // Act
        var output = await _listVideosUseCase.Handle(input, CancellationToken.None);

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
            outputItem.Rating.Should().Be(repositoryVideo.MovieRating.ToFriendlyString());
            outputItem.Published.Should().Be(repositoryVideo.Published);
            outputItem.Opened.Should().Be(repositoryVideo.Opened);
            outputItem.CreatedAt.Should().Be(repositoryVideo.CreatedAt);
            var outputCategoryItemIds = outputItem.Categories.Select(dto => dto.Id).ToList();
            outputCategoryItemIds.Should().BeEquivalentTo(repositoryVideo.Categories);
            var outputGenreItemIds = outputItem.Genres.Select(dto => dto.Id).ToList();
            outputGenreItemIds.Should().BeEquivalentTo(repositoryVideo.Genres);
            var outputCastMemberItemIds = outputItem.CastMembers.Select(dto => dto.Id).ToList();
            outputCastMemberItemIds.Should().BeEquivalentTo(repositoryVideo.CastMembers);
            outputItem.ThumbFileUrl.Should().Be(repositoryVideo.Thumb?.Path);
            outputItem.BannerFileUrl.Should().Be(repositoryVideo.Banner?.Path);
            outputItem.ThumbHalfFileUrl.Should().Be(repositoryVideo.ThumbHalf?.Path);
            outputItem.VideoFileUrl.Should().Be(repositoryVideo.Media?.FilePath);
            outputItem.TrailerFileUrl.Should().Be(repositoryVideo.Trailer?.FilePath);
        }

        _videoRepositoryMock.Verify(x => x.Search(
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
        var input = _fixture.GetExampleInput();
        var outputRepositorySearch = new SearchOutput<Entities.Video>(
            currentPage: input.Page,
            perPage: input.PerPage,
            items: new List<Entities.Video>(),
            total: 0
        );
        _videoRepositoryMock.Setup(x => x.Search(
            It.Is<SearchInput>(searchInput =>
                searchInput.Page == input.Page
                && searchInput.PerPage == input.PerPage
                && searchInput.Search == input.Search
                && searchInput.OrderBy == input.Sort
                && searchInput.Order == input.Dir),
            It.IsAny<CancellationToken>())).ReturnsAsync(outputRepositorySearch);

        // Act
        var output = await _listVideosUseCase.Handle(input, CancellationToken.None);

        // Assert
        output.Should().NotBeNull();
        output.Page.Should().Be(outputRepositorySearch.CurrentPage);
        output.PerPage.Should().Be(outputRepositorySearch.PerPage);
        output.Total.Should().Be(outputRepositorySearch.Total);
        output.Items.Should().HaveCount(0);

        _videoRepositoryMock.Verify(x => x.Search(
                It.Is<SearchInput>(searchInput =>
                searchInput.Page == input.Page
                && searchInput.PerPage == input.PerPage
                && searchInput.Search == input.Search
                && searchInput.OrderBy == input.Sort
                && searchInput.Order == input.Dir),
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [Fact(DisplayName = nameof(ListVideoWithRelations))]
    [Trait("Application", "ListVideo - Uses Cases")]
    public async Task ListVideoWithRelations()
    {
        // Arrange
        var examples = _fixture.GetValidVideoListAndAggregates(15);
        var exampleCategoryIds = examples.categories.Select(x => x.Id).ToList();
        var exampleGenreIds = examples.genres.Select(x => x.Id).ToList();
        var exampleCastMemberIds = examples.castMembers.Select(x => x.Id).ToList();

        _categoryRepository.Setup(x => x.GetListByIds(
            It.Is<List<Guid>>(list => list.All(exampleCategoryIds.Contains) && list.Count == exampleCategoryIds.Count),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(examples.categories);

        _genreRepository.Setup(x => x.GetListByIds(
            It.Is<List<Guid>>(list => list.All(exampleGenreIds.Contains) && list.Count == exampleGenreIds.Count),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(examples.genres);

        _castMemberRepository.Setup(x => x.GetListByIds(
            It.Is<List<Guid>>(list => list.All(exampleCastMemberIds.Contains) && list.Count == exampleCastMemberIds.Count),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(examples.castMembers);

        var input = _ = _fixture.GetExampleInput();
        var outputRepositorySearch = new SearchOutput<Entities.Video>(
            currentPage: input.Page,
            perPage: input.PerPage,
            items: examples.videos,
            total: new Random().Next(50, 200)
        );
        _videoRepositoryMock.Setup(x => x.Search(
            It.Is<SearchInput>(searchInput =>
                searchInput.Page == input.Page
                && searchInput.PerPage == input.PerPage
                && searchInput.Search == input.Search
                && searchInput.OrderBy == input.Sort
                && searchInput.Order == input.Dir),
            It.IsAny<CancellationToken>())).ReturnsAsync(outputRepositorySearch);

        // Act
        var output = await _listVideosUseCase.Handle(input, CancellationToken.None);

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
            outputItem.Rating.Should().Be(repositoryVideo.MovieRating.ToFriendlyString());
            outputItem.Published.Should().Be(repositoryVideo.Published);
            outputItem.Opened.Should().Be(repositoryVideo.Opened);
            outputItem.CreatedAt.Should().Be(repositoryVideo.CreatedAt);
            outputItem.ThumbFileUrl.Should().Be(repositoryVideo.Thumb?.Path);
            outputItem.BannerFileUrl.Should().Be(repositoryVideo.Banner?.Path);
            outputItem.ThumbHalfFileUrl.Should().Be(repositoryVideo.ThumbHalf?.Path);
            outputItem.VideoFileUrl.Should().Be(repositoryVideo.Media?.FilePath);
            outputItem.TrailerFileUrl.Should().Be(repositoryVideo.Trailer?.FilePath);
            foreach (var category in outputItem.Categories)
            {
                var categoryFromRepo = examples.categories.FirstOrDefault(x => x.Id == category.Id);
                category.Should().NotBeNull();
                category.Name.Should().Be(categoryFromRepo?.Name);
            }
            foreach (var genre in outputItem.Genres)
            {
                var genreFromRepo = examples.genres.FirstOrDefault(x => x.Id == genre.Id);
                genre.Should().NotBeNull();
                genre.Name.Should().Be(genreFromRepo?.Name);
            }
            foreach (var castMember in outputItem.CastMembers)
            {
                var castMemberFromRepo = examples.castMembers.FirstOrDefault(x => x.Id == castMember.Id);
                castMember.Should().NotBeNull();
                castMember.Name.Should().Be(castMemberFromRepo?.Name);
            }
        }

        _videoRepositoryMock.Verify(x => x.Search(
                It.Is<SearchInput>(searchInput =>
                searchInput.Page == input.Page
                && searchInput.PerPage == input.PerPage
                && searchInput.Search == input.Search
                && searchInput.OrderBy == input.Sort
                && searchInput.Order == input.Dir),
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [Fact(DisplayName = nameof(ListVideoWithoutRelationsDoesNotCallOtherRepositories))]
    [Trait("Application", "ListVideo - Uses Cases")]
    public async Task ListVideoWithoutRelationsDoesNotCallOtherRepositories()
    {
        // Arrange
        var examplesVideos = _fixture.GetValidVideoListWithoutRelations(15);

        var input = _ = _fixture.GetExampleInput();
        var outputRepositorySearch = new SearchOutput<Entities.Video>(
            currentPage: input.Page,
            perPage: input.PerPage,
            items: examplesVideos,
            total: new Random().Next(50, 200)
        );
        _videoRepositoryMock.Setup(x => x.Search(
            It.Is<SearchInput>(searchInput =>
                searchInput.Page == input.Page
                && searchInput.PerPage == input.PerPage
                && searchInput.Search == input.Search
                && searchInput.OrderBy == input.Sort
                && searchInput.Order == input.Dir),
            It.IsAny<CancellationToken>())).ReturnsAsync(outputRepositorySearch);

        // Act
        var output = await _listVideosUseCase.Handle(input, CancellationToken.None);

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
            outputItem.Rating.Should().Be(repositoryVideo.MovieRating.ToFriendlyString());
            outputItem.Published.Should().Be(repositoryVideo.Published);
            outputItem.Opened.Should().Be(repositoryVideo.Opened);
            outputItem.CreatedAt.Should().Be(repositoryVideo.CreatedAt);
            outputItem.ThumbFileUrl.Should().Be(repositoryVideo.Thumb?.Path);
            outputItem.BannerFileUrl.Should().Be(repositoryVideo.Banner?.Path);
            outputItem.ThumbHalfFileUrl.Should().Be(repositoryVideo.ThumbHalf?.Path);
            outputItem.VideoFileUrl.Should().Be(repositoryVideo.Media?.FilePath);
            outputItem.TrailerFileUrl.Should().Be(repositoryVideo.Trailer?.FilePath);
            outputItem.Categories.Should().BeEmpty();
            outputItem.Genres.Should().BeEmpty();
            outputItem.CastMembers.Should().BeEmpty();
        }

        _videoRepositoryMock.Verify(x => x.Search(
                It.Is<SearchInput>(searchInput =>
                    searchInput.Page == input.Page
                    && searchInput.PerPage == input.PerPage
                    && searchInput.Search == input.Search
                    && searchInput.OrderBy == input.Sort
                    && searchInput.Order == input.Dir),
                It.IsAny<CancellationToken>()
            ), Times.Once);

        _categoryRepository.Verify(x => x.GetListByIds(
                It.IsAny<List<Guid>>(),
                It.IsAny<CancellationToken>()
            ), Times.Never);
        _genreRepository.Verify(x => x.GetListByIds(
                It.IsAny<List<Guid>>(),
                It.IsAny<CancellationToken>()
            ), Times.Never);
        _castMemberRepository.Verify(x => x.GetListByIds(
                It.IsAny<List<Guid>>(),
                It.IsAny<CancellationToken>()
            ), Times.Never);
    }
}
