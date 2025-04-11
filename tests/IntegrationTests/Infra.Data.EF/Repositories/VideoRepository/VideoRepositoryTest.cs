
using Application.Exceptions;
using Domain.Entity;
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
}
