
using Application.Exceptions;
using Domain.Entity;
using Domain.SeedWork.SearchableRepository;
using FluentAssertions;
using Infra.Data.EF;
using Infra.Data.EF.Models;
using Infra.Data.EF.Repositories;
using Microsoft.EntityFrameworkCore;

namespace IntegrationTests.Infra.Data.EF.Repositories;

[Collection(nameof(VideoRepositoryTestFixture))]
public class VideoRepositoryTest
{
    private readonly VideoRepositoryTestFixture _fixture;

    public VideoRepositoryTest(VideoRepositoryTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(InsertOk))]
    [Trait("Integration/Infra.Data", "VideoRepository - Repositories")]
    public async Task InsertOk()
    {
        // Arrange
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleVideo = _fixture.GetValidVideo();
        var repository = new VideoRepository(dbContext);

        // Act
        await repository.Insert(exampleVideo, CancellationToken.None);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        // Assert
        var assertsDbContext = _fixture.CreateDbContext(true);
        var dbVideo = await assertsDbContext.Videos.FindAsync(exampleVideo.Id);
        dbVideo.Should().NotBeNull();
        dbVideo!.Id.Should().Be(exampleVideo.Id);
        dbVideo.Title.Should().Be(exampleVideo.Title);
        dbVideo.Description.Should().Be(exampleVideo.Description);
        dbVideo.Opened.Should().Be(exampleVideo.Opened);
        dbVideo.Published.Should().Be(exampleVideo.Published);
        dbVideo.YearLaunched.Should().Be(exampleVideo.YearLaunched);
        dbVideo.Duration.Should().Be(exampleVideo.Duration);
        dbVideo.MovieRating.Should().Be(exampleVideo.MovieRating);
        dbVideo.CreatedAt.Should().BeSameDateAs(exampleVideo.CreatedAt);

        dbVideo.Thumb.Should().BeNull();
        dbVideo.Trailer.Should().BeNull();
        dbVideo.Banner.Should().BeNull();
        dbVideo.Media.Should().BeNull();
        dbVideo.Trailer.Should().BeNull();

        dbVideo.Categories.Should().BeEmpty();
        dbVideo.Genres.Should().BeEmpty();
        dbVideo.CastMembers.Should().BeEmpty();
    }

    [Fact(DisplayName = nameof(InsertWithRelations))]
    [Trait("Integration/Infra.Data", "VideoRepository - Repositories")]
    public async Task InsertWithRelations()
    {
        // Arrange
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleVideo = _fixture.GetValidVideo();

        var castMembers = _fixture.GetRandomCastMemberList();
        await dbContext.CastMembers.AddRangeAsync(castMembers, CancellationToken.None);
        castMembers.ToList().ForEach(x => exampleVideo.AddCastMember(x.Id));

        var categories = _fixture.GetRandomCategoryList();
        await dbContext.Categories.AddRangeAsync(categories, CancellationToken.None);
        categories.ToList().ForEach(x => exampleVideo.AddCategory(x.Id));

        var genres = _fixture.GetRandomGenreList();
        await dbContext.Genres.AddRangeAsync(genres, CancellationToken.None);
        genres.ToList().ForEach(x => exampleVideo.AddGenre(x.Id));

        await dbContext.SaveChangesAsync();
        var repository = new VideoRepository(dbContext);

        // Act
        await repository.Insert(exampleVideo, CancellationToken.None);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        // Assert
        var assertsDbContext = _fixture.CreateDbContext(true);
        var dbVideo = await assertsDbContext.Videos.FindAsync(exampleVideo.Id);
        dbVideo.Should().NotBeNull();

        var dbVideoCategories = assertsDbContext.VideosCategories
            .Where(x => x.VideoId == dbVideo!.Id)
            .ToList();
        dbVideoCategories.Should().HaveCount(categories.Count);
        dbVideoCategories.Select(relation => relation.CategoryId)
            .Should()
            .BeEquivalentTo(categories.Select(category => category.Id));

        var dbVideoGenres = assertsDbContext.VideosGenres
            .Where(x => x.VideoId == dbVideo!.Id)
            .ToList();
        dbVideoGenres.Should().HaveCount(genres.Count);
        dbVideoGenres.Select(relation => relation.GenreId)
            .Should()
            .BeEquivalentTo(genres.Select(genre => genre.Id));

        var dbVideoCastMembers = assertsDbContext.VideosCastMembers
            .Where(x => x.VideoId == dbVideo!.Id)
            .ToList();
        dbVideoCastMembers.Should().HaveCount(castMembers.Count);
        dbVideoCastMembers.Select(relation => relation.CastMemberId)
            .Should()
            .BeEquivalentTo(castMembers.Select(castMember => castMember.Id));
    }

    [Fact(DisplayName = nameof(InsertWithMediasAndImages))]
    [Trait("Integration/Infra.Data", "VideoRepository - Repositories")]
    public async Task InsertWithMediasAndImages()
    {
        // Arrange
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleVideo = _fixture.GetValidVideoWithAllProperties();
        var repository = new VideoRepository(dbContext);

        // Act
        await repository.Insert(exampleVideo, CancellationToken.None);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        // Assert
        var assertsDbContext = _fixture.CreateDbContext(true);
        var dbVideo = await assertsDbContext.Videos
            .Include(x => x.Media)
            .Include(x => x.Trailer)
            .FirstOrDefaultAsync(video => video.Id == exampleVideo.Id);

        dbVideo.Should().NotBeNull();
        dbVideo!.Thumb.Should().NotBeNull();
        dbVideo.Thumb!.Path.Should().Be(exampleVideo.Thumb!.Path);
        dbVideo.ThumbHalf.Should().NotBeNull();
        dbVideo.ThumbHalf!.Path.Should().Be(exampleVideo.ThumbHalf!.Path);
        dbVideo.Banner.Should().NotBeNull();
        dbVideo.Banner!.Path.Should().Be(exampleVideo.Banner!.Path);
        dbVideo.Media.Should().NotBeNull();
        dbVideo.Media!.FilePath.Should().Be(exampleVideo.Media!.FilePath);
        dbVideo.Media.EncodedPath.Should().Be(exampleVideo.Media.EncodedPath);
        dbVideo.Media.Status.Should().Be(exampleVideo.Media.Status);
        dbVideo.Trailer.Should().NotBeNull();
        dbVideo.Trailer!.FilePath.Should().Be(exampleVideo.Trailer!.FilePath);
        dbVideo.Trailer.EncodedPath.Should().Be(exampleVideo.Trailer.EncodedPath);
        dbVideo.Trailer.Status.Should().Be(exampleVideo.Trailer.Status);
    }

    [Fact(DisplayName = nameof(UpdateOk))]
    [Trait("Integration/Infra.Data", "VideoRepository - Repositories")]
    public async Task UpdateOk()
    {
        // Arrange
        CodeflixCatalogDbContext dbContextArrange = _fixture.CreateDbContext();
        var exampleVideo = _fixture.GetValidVideo();
        await dbContextArrange.AddAsync(exampleVideo, CancellationToken.None);
        await dbContextArrange.SaveChangesAsync(CancellationToken.None);

        var dbContextAct = _fixture.CreateDbContext(true);
        var repository = new VideoRepository(dbContextAct);
        var newValidVideo = _fixture.GetValidVideo();
        exampleVideo.Update(
            newValidVideo.Title,
            newValidVideo.Description,
            newValidVideo.Opened,
            newValidVideo.Published,
            newValidVideo.YearLaunched,
            newValidVideo.Duration,
            newValidVideo.MovieRating
        );

        // Act
        await repository.Update(exampleVideo, CancellationToken.None);
        await dbContextAct.SaveChangesAsync(CancellationToken.None);

        // Assert
        var assertsDbContext = _fixture.CreateDbContext(true);
        var dbVideo = await assertsDbContext.Videos.FindAsync(exampleVideo.Id);
        dbVideo.Should().NotBeNull();
        dbVideo!.Id.Should().Be(exampleVideo.Id);
        dbVideo.Title.Should().Be(newValidVideo.Title);
        dbVideo.Description.Should().Be(newValidVideo.Description);
        dbVideo.Opened.Should().Be(newValidVideo.Opened);
        dbVideo.Published.Should().Be(newValidVideo.Published);
        dbVideo.YearLaunched.Should().Be(newValidVideo.YearLaunched);
        dbVideo.Duration.Should().Be(newValidVideo.Duration);
        dbVideo.MovieRating.Should().Be(newValidVideo.MovieRating);
        dbVideo.CreatedAt.Should().BeSameDateAs(newValidVideo.CreatedAt);

        dbVideo.Thumb.Should().BeNull();
        dbVideo.Trailer.Should().BeNull();
        dbVideo.Banner.Should().BeNull();
        dbVideo.Media.Should().BeNull();
        dbVideo.Trailer.Should().BeNull();

        dbVideo.Categories.Should().BeEmpty();
        dbVideo.Genres.Should().BeEmpty();
        dbVideo.CastMembers.Should().BeEmpty();
    }

    [Fact(DisplayName = nameof(UpdateEntitiesAndValueObjects))]
    [Trait("Integration/Infra.Data", "VideoRepository - Repositories")]
    public async Task UpdateEntitiesAndValueObjects()
    {
        // Arrange
        var dbContextArrange = _fixture.CreateDbContext();
        var exampleVideo = _fixture.GetValidVideo();
        await dbContextArrange.AddAsync(exampleVideo, CancellationToken.None);
        await dbContextArrange.SaveChangesAsync(CancellationToken.None);

        var updatedThumb = _fixture.GetValidImagePath();
        var updatedThumbHalf = _fixture.GetValidImagePath();
        var updatedBanner = _fixture.GetValidImagePath();
        var updatedMedia = _fixture.GetValidMediaPath();
        var updatedMediaEncoded = _fixture.GetValidMediaPath();
        var updatedTrailer = _fixture.GetValidMediaPath();

        // Act
        var dbContextAct = _fixture.CreateDbContext(true);
        var repository = new VideoRepository(dbContextAct);

        var savedVideo = await dbContextAct.Videos.FirstOrDefaultAsync(video => video.Id == exampleVideo.Id);
        savedVideo!.UpdateThumb(updatedThumb);
        savedVideo.UpdateThumbHalf(updatedThumbHalf);
        savedVideo.UpdateBanner(updatedBanner);
        savedVideo.UpdateMedia(updatedMedia);
        savedVideo.UpdateTrailer(updatedTrailer);
        savedVideo.UpdateAsEncoded(updatedMediaEncoded);

        await repository.Update(savedVideo, CancellationToken.None);
        await dbContextAct.SaveChangesAsync(CancellationToken.None);

        // Assert
        var assertsDbContext = _fixture.CreateDbContext(true);
        var dbVideo = await assertsDbContext.Videos.FindAsync(savedVideo.Id);
        dbVideo.Should().NotBeNull();
        dbVideo!.Thumb.Should().NotBeNull();
        dbVideo.Thumb!.Path.Should().Be(updatedThumb);
        dbVideo.ThumbHalf.Should().NotBeNull();
        dbVideo.ThumbHalf!.Path.Should().Be(updatedThumbHalf);
        dbVideo.Banner.Should().NotBeNull();
        dbVideo.Banner!.Path.Should().Be(updatedBanner);
        dbVideo.Media.Should().NotBeNull();
        dbVideo.Media!.FilePath.Should().Be(updatedMedia);
        dbVideo.Media.EncodedPath.Should().Be(updatedMediaEncoded);
        dbVideo.Media.Status.Should().Be(Domain.Enum.MediaStatus.Completed);
        dbVideo.Trailer.Should().NotBeNull();
        dbVideo.Trailer!.FilePath.Should().Be(updatedTrailer);
    }

    [Fact(DisplayName = nameof(UpdateWithRelations))]
    [Trait("Integration/Infra.Data", "VideoRepository - Repositories")]
    public async Task UpdateWithRelations()
    {
        // Arrange
        var dbContext = _fixture.CreateDbContext();
        var exampleVideo = _fixture.GetValidVideo();
        await dbContext.AddAsync(exampleVideo, CancellationToken.None);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var castMembers = _fixture.GetRandomCastMemberList();
        await dbContext.CastMembers.AddRangeAsync(castMembers, CancellationToken.None);

        var categories = _fixture.GetRandomCategoryList();
        await dbContext.Categories.AddRangeAsync(categories, CancellationToken.None);

        var genres = _fixture.GetRandomGenreList();
        await dbContext.Genres.AddRangeAsync(genres, CancellationToken.None);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        // Act
        var dbContextAct = _fixture.CreateDbContext(true);
        var repository = new VideoRepository(dbContextAct);
        var savedVideo = await dbContextAct.Videos.FirstOrDefaultAsync(video => video.Id == exampleVideo.Id);
        castMembers.ToList().ForEach(x => savedVideo!.AddCastMember(x.Id));
        categories.ToList().ForEach(x => savedVideo!.AddCategory(x.Id));
        genres.ToList().ForEach(x => savedVideo!.AddGenre(x.Id));

        await repository.Update(savedVideo!, CancellationToken.None);
        await dbContextAct.SaveChangesAsync(CancellationToken.None);

        // Assert
        var assertsDbContext = _fixture.CreateDbContext(true);
        var dbVideo = await assertsDbContext.Videos.FindAsync(exampleVideo.Id);
        dbVideo.Should().NotBeNull();

        var dbVideoCategories = assertsDbContext.VideosCategories
            .Where(x => x.VideoId == dbVideo!.Id)
            .ToList();
        dbVideoCategories.Should().HaveCount(categories.Count);
        dbVideoCategories.Select(relation => relation.CategoryId)
            .Should()
            .BeEquivalentTo(categories.Select(category => category.Id));

        var dbVideoGenres = assertsDbContext.VideosGenres
            .Where(x => x.VideoId == dbVideo!.Id)
            .ToList();
        dbVideoGenres.Should().HaveCount(genres.Count);
        dbVideoGenres.Select(relation => relation.GenreId)
            .Should()
            .BeEquivalentTo(genres.Select(genre => genre.Id));

        var dbVideoCastMembers = assertsDbContext.VideosCastMembers
            .Where(x => x.VideoId == dbVideo!.Id)
            .ToList();
        dbVideoCastMembers.Should().HaveCount(castMembers.Count);
        dbVideoCastMembers.Select(relation => relation.CastMemberId)
            .Should()
            .BeEquivalentTo(castMembers.Select(castMember => castMember.Id));
    }

    [Fact(DisplayName = nameof(DeleteOk))]
    [Trait("Integration/Infra.Data", "VideoRepository - Repositories")]
    public async Task DeleteOk()
    {
        // Arrange
        var dbContext = _fixture.CreateDbContext();
        var exampleVideo = _fixture.GetValidVideo();
        await dbContext.AddAsync(exampleVideo, CancellationToken.None);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        // Act
        var dbContextAct = _fixture.CreateDbContext(true);
        var repository = new VideoRepository(dbContextAct);
        var savedVideo = await dbContextAct.Videos.FirstOrDefaultAsync(video => video.Id == exampleVideo.Id);

        await repository.Delete(savedVideo!, CancellationToken.None);
        await dbContextAct.SaveChangesAsync(CancellationToken.None);

        // Assert
        var assertsDbContext = _fixture.CreateDbContext(true);
        var dbVideo = await assertsDbContext.Videos.FindAsync(exampleVideo.Id);
        dbVideo.Should().BeNull();
    }

    [Fact(DisplayName = nameof(DeleteWithAllPropertiesAndRelations))]
    [Trait("Integration/Infra.Data", "VideoRepository - Repositories")]
    public async Task DeleteWithAllPropertiesAndRelations()
    {
        // Arrange
        var dbContext = _fixture.CreateDbContext();
        var exampleVideo = _fixture.GetValidVideoWithAllProperties();

        var castMembers = _fixture.GetRandomCastMemberList();
        await dbContext.CastMembers.AddRangeAsync(castMembers, CancellationToken.None);
        castMembers.ToList().ForEach(x => dbContext.VideosCastMembers.Add(new VideosCastMembers(x.Id, exampleVideo.Id)));

        var categories = _fixture.GetRandomCategoryList();
        await dbContext.Categories.AddRangeAsync(categories, CancellationToken.None);
        categories.ToList().ForEach(x => dbContext.VideosCategories.Add(new VideosCategories(x.Id, exampleVideo.Id)));

        var genres = _fixture.GetRandomGenreList();
        await dbContext.Genres.AddRangeAsync(genres, CancellationToken.None);
        genres.ToList().ForEach(x => dbContext.VideosGenres.Add(new VideosGenres(x.Id, exampleVideo.Id)));

        await dbContext.AddAsync(exampleVideo, CancellationToken.None);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        // Act
        var dbContextAct = _fixture.CreateDbContext(true);
        var repository = new VideoRepository(dbContextAct);
        var savedVideo = await dbContextAct.Videos.FirstOrDefaultAsync(video => video.Id == exampleVideo.Id);

        await repository.Delete(savedVideo!, CancellationToken.None);
        await dbContextAct.SaveChangesAsync(CancellationToken.None);

        // Assert
        var assertsDbContext = _fixture.CreateDbContext(true);
        var dbVideo = await assertsDbContext.Videos.FindAsync(exampleVideo.Id);
        dbVideo.Should().BeNull();

        assertsDbContext.VideosCategories
            .Where(x => x.VideoId == exampleVideo!.Id)
            .Count().Should().Be(0);

        assertsDbContext.VideosGenres
            .Where(x => x.VideoId == exampleVideo!.Id)
            .Count().Should().Be(0);

        assertsDbContext.VideosCastMembers
            .Where(x => x.VideoId == exampleVideo!.Id)
            .Count().Should().Be(0);

        assertsDbContext.Set<Media>().Count().Should().Be(0);
    }

    [Fact(DisplayName = nameof(GetOk))]
    [Trait("Integration/Infra.Data", "VideoRepository - Repositories")]
    public async Task GetOk()
    {
        // Arrange
        var dbContext = _fixture.CreateDbContext();
        var exampleVideo = _fixture.GetValidVideo();
        await dbContext.AddAsync(exampleVideo, CancellationToken.None);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        // Act
        var actDbContext = _fixture.CreateDbContext(true);
        var repository = new VideoRepository(actDbContext);
        var videoFromDb = await repository.Get(exampleVideo.Id, CancellationToken.None);

        // Assert
        videoFromDb.Should().NotBeNull();
        videoFromDb!.Id.Should().Be(exampleVideo.Id);
        videoFromDb.Title.Should().Be(exampleVideo.Title);
        videoFromDb.Description.Should().Be(exampleVideo.Description);
        videoFromDb.Opened.Should().Be(exampleVideo.Opened);
        videoFromDb.Published.Should().Be(exampleVideo.Published);
        videoFromDb.YearLaunched.Should().Be(exampleVideo.YearLaunched);
        videoFromDb.Duration.Should().Be(exampleVideo.Duration);
        videoFromDb.MovieRating.Should().Be(exampleVideo.MovieRating);
        videoFromDb.CreatedAt.Should().BeSameDateAs(exampleVideo.CreatedAt);
    }

    [Fact(DisplayName = nameof(GetThrowsWhenVideoNotFound))]
    [Trait("Integration/Infra.Data", "VideoRepository - Repositories")]
    public async Task GetThrowsWhenVideoNotFound()
    {
        // Arrange
        var randomId = Guid.NewGuid();

        // Act
        var repository = new VideoRepository(_fixture.CreateDbContext());
        var action = async () => await repository.Get(randomId, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Video '{randomId}' not found");
    }

    [Fact(DisplayName = nameof(GetWithAllProperties))]
    [Trait("Integration/Infra.Data", "VideoRepository - Repositories")]
    public async Task GetWithAllProperties()
    {
        // Arrange
        var dbContext = _fixture.CreateDbContext();
        var exampleVideo = _fixture.GetValidVideoWithAllProperties();

        var castMembers = _fixture.GetRandomCastMemberList();
        await dbContext.CastMembers.AddRangeAsync(castMembers, CancellationToken.None);
        castMembers.ToList().ForEach(x => dbContext.VideosCastMembers.Add(new VideosCastMembers(x.Id, exampleVideo.Id)));

        var categories = _fixture.GetRandomCategoryList();
        await dbContext.Categories.AddRangeAsync(categories, CancellationToken.None);
        categories.ToList().ForEach(x => dbContext.VideosCategories.Add(new VideosCategories(x.Id, exampleVideo.Id)));

        var genres = _fixture.GetRandomGenreList();
        await dbContext.Genres.AddRangeAsync(genres, CancellationToken.None);
        genres.ToList().ForEach(x => dbContext.VideosGenres.Add(new VideosGenres(x.Id, exampleVideo.Id)));

        await dbContext.AddAsync(exampleVideo, CancellationToken.None);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        // Act
        var actDbContext = _fixture.CreateDbContext(true);
        var repository = new VideoRepository(actDbContext);
        var videoFromDb = await repository.Get(exampleVideo.Id, CancellationToken.None);

        // Assert
        videoFromDb.Should().NotBeNull();
        videoFromDb!.Id.Should().Be(exampleVideo.Id);
        videoFromDb.Title.Should().Be(exampleVideo.Title);
        videoFromDb.Description.Should().Be(exampleVideo.Description);
        videoFromDb.Opened.Should().Be(exampleVideo.Opened);
        videoFromDb.Published.Should().Be(exampleVideo.Published);
        videoFromDb.YearLaunched.Should().Be(exampleVideo.YearLaunched);
        videoFromDb.Duration.Should().Be(exampleVideo.Duration);
        videoFromDb.MovieRating.Should().Be(exampleVideo.MovieRating);
        videoFromDb.CreatedAt.Should().BeSameDateAs(exampleVideo.CreatedAt);
        videoFromDb.Thumb.Should().NotBeNull();
        videoFromDb.Thumb!.Path.Should().Be(exampleVideo.Thumb!.Path);
        videoFromDb.ThumbHalf.Should().NotBeNull();
        videoFromDb.ThumbHalf!.Path.Should().Be(exampleVideo.ThumbHalf!.Path);
        videoFromDb.Banner.Should().NotBeNull();
        videoFromDb.Banner!.Path.Should().Be(exampleVideo.Banner!.Path);
        videoFromDb.Media.Should().NotBeNull();
        videoFromDb.Media!.FilePath.Should().Be(exampleVideo.Media!.FilePath);
        videoFromDb.Media.EncodedPath.Should().Be(exampleVideo.Media.EncodedPath);
        videoFromDb.Media.Status.Should().Be(exampleVideo.Media.Status);
        videoFromDb.Trailer.Should().NotBeNull();
        videoFromDb.Trailer!.FilePath.Should().Be(exampleVideo.Trailer!.FilePath);
        videoFromDb.Trailer.EncodedPath.Should().Be(exampleVideo.Trailer.EncodedPath);
        videoFromDb.Trailer.Status.Should().Be(exampleVideo.Trailer.Status);

        videoFromDb.Categories.Should().BeEquivalentTo(categories.Select(c => c.Id));
        videoFromDb.Genres.Should().BeEquivalentTo(genres.Select(g => g.Id));
        videoFromDb.CastMembers.Should().BeEquivalentTo(castMembers.Select(c => c.Id));
    }

    [Fact(DisplayName = nameof(SearchReturnsListAndTotal))]
    [Trait("Integration/Infra.Data", "VideoRepository - Repositories")]
    public async Task SearchReturnsListAndTotal()
    {
        // Arrange
        var dbContext = _fixture.CreateDbContext();
        var exampleVideos = _fixture.GetVideoList(10);
        await dbContext.AddRangeAsync(exampleVideos, CancellationToken.None);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var searchInput = new SearchInput(1, 20, "", "", SearchOrder.Asc);

        // Act
        var actDbContext = _fixture.CreateDbContext(true);
        var repository = new VideoRepository(actDbContext);
        var searchResult = await repository.Search(searchInput, CancellationToken.None);

        // Assert
        searchResult.Should().NotBeNull();
        searchResult.Total.Should().Be(10);
        searchResult.Items.Should().NotBeNullOrEmpty();
        searchResult.Items.Should().HaveCount(10);
        searchResult.Items.Should().BeEquivalentTo(exampleVideos);
    }

    [Fact(DisplayName = nameof(SearchReturnsEmptyWhenPersistenceIsEmpty))]
    [Trait("Integration/Infra.Data", "VideoRepository - Repositories")]
    public async Task SearchReturnsEmptyWhenPersistenceIsEmpty()
    {
        // Arrange
        var dbContext = _fixture.CreateDbContext();
        var searchInput = new SearchInput(1, 20, "", "", SearchOrder.Asc);

        // Act
        var actDbContext = _fixture.CreateDbContext(true);
        var repository = new VideoRepository(actDbContext);
        var searchResult = await repository.Search(searchInput, CancellationToken.None);

        // Assert
        searchResult.Should().NotBeNull();
        searchResult.Total.Should().Be(0);
        searchResult.Items.Should().BeNullOrEmpty();
        searchResult.Items.Should().HaveCount(0);
    }

    [Theory(DisplayName = nameof(SearchReturnsPaginated))]
    [Trait("Integration/Infra.Data", "VideoRepository - Repositories")]
    [InlineData(10, 1, 5, 5)]
    [InlineData(10, 2, 5, 5)]
    [InlineData(7, 2, 5, 2)]
    [InlineData(7, 3, 5, 0)]
    public async Task SearchReturnsPaginated(int numberOfItemsToGenerate, int page, int perPage, int expectedNumberOfItems)
    {
        // Arrange
        var dbContext = _fixture.CreateDbContext();
        var exampleVideos = _fixture.GetVideoList(numberOfItemsToGenerate);
        await dbContext.AddRangeAsync(exampleVideos, CancellationToken.None);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var searchInput = new SearchInput(page, perPage, "", "", SearchOrder.Asc);

        // Act
        var actDbContext = _fixture.CreateDbContext(true);
        var repository = new VideoRepository(actDbContext);
        var searchResult = await repository.Search(searchInput, CancellationToken.None);

        // Assert
        searchResult.Should().NotBeNull();
        searchResult.Total.Should().Be(numberOfItemsToGenerate);
        searchResult.Items.Should().HaveCount(expectedNumberOfItems);
        searchResult.PerPage.Should().Be(perPage);
        searchResult.CurrentPage.Should().Be(page);
    }

    [Theory(DisplayName = nameof(SearchByTitle))]
    [Trait("Integration/Infra.Data", "VideoRepository - Repositories")]
    [InlineData("Action", 1, 5, 1, 1)]
    [InlineData("Horror", 1, 5, 3, 3)]
    [InlineData("Horror", 2, 5, 0, 3)]
    [InlineData("Sci-fi", 1, 5, 4, 4)]
    [InlineData("Sci-fi", 1, 2, 2, 4)]
    [InlineData("Sci-fi", 2, 3, 1, 4)]
    [InlineData("Romance", 1, 5, 0, 0)]
    [InlineData("Robots", 1, 5, 2, 2)]
    [InlineData("", 1, 5, 5, 9)]
    [InlineData("test-no-items", 1, 5, 0, 0)]
    public async Task SearchByTitle(string search, int page, int perPage, int expectedNumberOfItems, int expectedTotalItems)
    {
        // Arrange
        var dbContext = _fixture.CreateDbContext();
        var exampleVideos = _fixture.GetExampleVideoListWithNames(new List<string>(){
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
        await dbContext.AddRangeAsync(exampleVideos, CancellationToken.None);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var searchInput = new SearchInput(page, perPage, search, "", SearchOrder.Asc);

        // Act
        var actDbContext = _fixture.CreateDbContext(true);
        var repository = new VideoRepository(actDbContext);
        var searchResult = await repository.Search(searchInput, CancellationToken.None);

        // Assert
        searchResult.Should().NotBeNull();
        searchResult.Total.Should().Be(expectedTotalItems);
        searchResult.Items.Should().HaveCount(expectedNumberOfItems);
    }

    [Theory(DisplayName = nameof(SearchOrdered))]
    [Trait("Integration/Infra.Data", "VideoRepository - Repositories")]
    [InlineData("title", "asc")]
    [InlineData("title", "desc")]
    [InlineData("id", "asc")]
    [InlineData("id", "desc")]
    [InlineData("createdAt", "asc")]
    [InlineData("createdAt", "desc")]
    [InlineData("", "asc")]
    public async Task SearchOrdered(string orderBy, string order)
    {
        // Given
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleVideosList = _fixture.GetVideoList(15);
        await dbContext.AddRangeAsync(exampleVideosList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var videoRepository = new VideoRepository(dbContext);
        var searchOrder = order.ToLower() == "asc" ? SearchOrder.Asc : SearchOrder.Desc;
        var searchInput = new SearchInput(1, 20, "", orderBy, searchOrder);
        var expectedOrderedList = _fixture.CloneVideoListOrdered(exampleVideosList, orderBy, searchOrder);

        // When
        var output = await videoRepository.Search(searchInput, CancellationToken.None);

        // Then
        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.CurrentPage.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(exampleVideosList.Count());
        output.Items.Should().HaveCount(exampleVideosList.Count);
        for (int index = 0; index < expectedOrderedList.Count(); index++)
        {
            var expecetedItem = expectedOrderedList[index];
            var outputItem = output.Items[index];
            expecetedItem.Should().NotBeNull();
            outputItem.Should().NotBeNull();
            outputItem!.Id.Should().Be(expecetedItem!.Id);
            outputItem.Title.Should().Be(expecetedItem.Title);
            outputItem.Description.Should().Be(expecetedItem.Description);
            outputItem.CreatedAt.Should().Be(expecetedItem.CreatedAt);
        }
    }

    [Fact(DisplayName = nameof(SearchReturnsAllRelations))]
    [Trait("Integration/Infra.Data", "VideoRepository - Repositories")]
    public async Task SearchReturnsAllRelations()
    {
        // Given
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleVideosList = _fixture.GetVideoList(15);
        foreach (var video in exampleVideosList)
        {
            var castMembers = _fixture.GetRandomCastMemberList();
            await dbContext.CastMembers.AddRangeAsync(castMembers, CancellationToken.None);
            castMembers.ToList().ForEach(x =>
            {
                video.AddCastMember(x.Id);
                dbContext.VideosCastMembers.Add(new VideosCastMembers(x.Id, video.Id));
            });

            var categories = _fixture.GetRandomCategoryList();
            await dbContext.Categories.AddRangeAsync(categories, CancellationToken.None);
            categories.ToList().ForEach(x =>
            {
                video.AddCategory(x.Id);
                dbContext.VideosCategories.Add(new VideosCategories(x.Id, video.Id));
            });

            var genres = _fixture.GetRandomGenreList();
            await dbContext.Genres.AddRangeAsync(genres, CancellationToken.None);
            genres.ToList().ForEach(x =>
            {
                video.AddGenre(x.Id);
                dbContext.VideosGenres.Add(new VideosGenres(x.Id, video.Id));
            });
        }

        await dbContext.AddRangeAsync(exampleVideosList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var videoRepository = new VideoRepository(dbContext);
        var searchInput = new SearchInput(1, 20, "", "title", SearchOrder.Asc);

        // When
        var output = await videoRepository.Search(searchInput, CancellationToken.None);

        // Then
        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.CurrentPage.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(exampleVideosList.Count());
        output.Items.Should().HaveCount(exampleVideosList.Count);
        foreach (var outputVideo in output.Items)
        {
            var expectedVideo = exampleVideosList.Find(v => v.Id == outputVideo.Id);
            expectedVideo.Should().NotBeNull();
            outputVideo.Should().NotBeNull();
            outputVideo!.Id.Should().Be(expectedVideo!.Id);
            outputVideo.Title.Should().Be(expectedVideo.Title);
            outputVideo.Description.Should().Be(expectedVideo.Description);
            outputVideo.CreatedAt.Should().Be(expectedVideo.CreatedAt);
            outputVideo.Categories.Should().BeEquivalentTo(expectedVideo.Categories);
            outputVideo.Genres.Should().BeEquivalentTo(expectedVideo.Genres);
            outputVideo.CastMembers.Should().BeEquivalentTo(expectedVideo.CastMembers);
        }
    }

}
