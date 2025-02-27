using Domain.Enum;
using Domain.Exceptions;
using Domain.Validation;
using FluentAssertions;
using Entities = Domain.Entity;

namespace UnitTests.Domain.Entity.Video;

[Collection(nameof(VideoTestFixture))]
public class VideoTest
{
    private readonly VideoTestFixture _fixture;

    public VideoTest(VideoTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(InstantiateOk))]
    [Trait("Domain", "Video - Aggregates")]
    public void InstantiateOk()
    {
        // Given
        var title = _fixture.GetValidTitle();
        var description = _fixture.GetValidDescription();
        var opened = _fixture.GetValidOpened();
        var published = _fixture.GetValidPublished();
        var year = _fixture.GetValidYear();
        var duration = _fixture.GetValidDuration();
        var rating = _fixture.GetRandomMovieRating();
        var now = DateTime.Now;

        // When
        var video = new Entities.Video(title, description, opened, published, year, duration, rating);

        // Then
        video.Should().NotBeNull();
        video.Id.Should().NotBe(Guid.Empty);
        video.Title.Should().Be(title);
        video.Description.Should().Be(description);
        video.Opened.Should().Be(opened);
        video.Published.Should().Be(published);
        video.YearLaunched.Should().Be(year);
        video.Duration.Should().Be(duration);
        video.CreatedAt.Should().BeSameDateAs(now);
        video.Thumb.Should().BeNull();
        video.ThumbHalf.Should().BeNull();
        video.Banner.Should().BeNull();
        video.Media.Should().BeNull();
        video.Trailer.Should().BeNull();
    }

    [Fact(DisplayName = nameof(ValidateWhenValidState))]
    [Trait("Domain", "Video - Aggregates")]
    public void ValidateWhenValidState()
    {
        // Given
        var validVideo = _fixture.GetValidVideo();
        var notificationValidationHandler = new NotificationValidationHandler();

        // When
        validVideo.Validate(notificationValidationHandler);

        // Then
        notificationValidationHandler.HasErrors().Should().BeFalse();
    }

    [Fact(DisplayName = nameof(ValidateWhenInvalidState))]
    [Trait("Domain", "Video - Aggregates")]
    public void ValidateWhenInvalidState()
    {
        // Given
        var invalidVideo = new Entities.Video(
            _fixture.GetTooLongTitle(),
            _fixture.GetDescriptionTooLong(),
            _fixture.GetValidOpened(),
            _fixture.GetValidPublished(),
            _fixture.GetValidYear(),
            _fixture.GetValidDuration(),
            _fixture.GetRandomMovieRating()
        );

        var notificationValidationHandler = new NotificationValidationHandler();

        // When
        invalidVideo.Validate(notificationValidationHandler);

        // Then
        notificationValidationHandler.HasErrors().Should().BeTrue();
        notificationValidationHandler.Errors().Should().HaveCount(2);
        notificationValidationHandler.Errors().Should().BeEquivalentTo(
        [
            new ValidationError("'Title' should be less or equal 255 characters"),
            new ValidationError("'Description' should be less or equal 4000 characters")
        ]);
    }

    [Fact(DisplayName = nameof(ValidateAfterUpdateWhenValidState))]
    [Trait("Domain", "Video - Aggregates")]
    public void ValidateAfterUpdateWhenValidState()
    {
        // Given
        var validVideo = _fixture.GetValidVideo();
        validVideo.Update(
            _fixture.GetValidTitle(),
            _fixture.GetValidDescription(),
            _fixture.GetValidOpened(),
            _fixture.GetValidPublished(),
            _fixture.GetValidYear(),
            _fixture.GetValidDuration(),
            _fixture.GetRandomMovieRating()
        );

        var notificationValidationHandler = new NotificationValidationHandler();

        // When
        validVideo.Validate(notificationValidationHandler);

        // Then
        notificationValidationHandler.HasErrors().Should().BeFalse();
    }

    //validate after update when invalid state
    [Fact(DisplayName = nameof(ValidateAfterUpdateWhenInvalidState))]
    [Trait("Domain", "Video - Aggregates")]
    public void ValidateAfterUpdateWhenInvalidState()
    {
        // Given
        var validVideo = _fixture.GetValidVideo();
        validVideo.Update(
            _fixture.GetTooLongTitle(),
            _fixture.GetDescriptionTooLong(),
            _fixture.GetValidOpened(),
            _fixture.GetValidPublished(),
            _fixture.GetValidYear(),
            _fixture.GetValidDuration(),
            _fixture.GetRandomMovieRating()
        );

        var notificationValidationHandler = new NotificationValidationHandler();

        // When
        validVideo.Validate(notificationValidationHandler);

        // Then
        notificationValidationHandler.HasErrors().Should().BeTrue();
        notificationValidationHandler.Errors().Should().HaveCount(2);
        notificationValidationHandler.Errors().Should().BeEquivalentTo(
        [
            new ValidationError("'Title' should be less or equal 255 characters"),
            new ValidationError("'Description' should be less or equal 4000 characters")
        ]);
    }

    [Fact(DisplayName = nameof(UpdateThumb))]
    [Trait("Domain", "Video - Aggregates")]
    public void UpdateThumb()
    {
        // Given
        var validVideo = _fixture.GetValidVideo();
        var validImagePath = _fixture.GetValidImagePath();

        // When
        validVideo.UpdateThumb(validImagePath);

        // Then
        validVideo.Thumb.Should().NotBeNull();
        validVideo.Thumb!.Path.Should().Be(validImagePath);
    }

    [Fact(DisplayName = nameof(UpdateThumbHalf))]
    [Trait("Domain", "Video - Aggregates")]
    public void UpdateThumbHalf()
    {
        // Given
        var validVideo = _fixture.GetValidVideo();
        var validImagePath = _fixture.GetValidImagePath();

        // When
        validVideo.UpdateThumbHalf(validImagePath);

        // Then
        validVideo.ThumbHalf.Should().NotBeNull();
        validVideo.ThumbHalf!.Path.Should().Be(validImagePath);
    }

    [Fact(DisplayName = nameof(UpdateBanner))]
    [Trait("Domain", "Video - Aggregates")]
    public void UpdateBanner()
    {
        // Given
        var validVideo = _fixture.GetValidVideo();
        var validImagePath = _fixture.GetValidImagePath();

        // When
        validVideo.UpdateBanner(validImagePath);

        // Then
        validVideo.Banner.Should().NotBeNull();
        validVideo.Banner!.Path.Should().Be(validImagePath);
    }


    [Fact(DisplayName = nameof(UpdateMedia))]
    [Trait("Domain", "Video - Aggregates")]
    public void UpdateMedia()
    {
        // Given
        var validVideo = _fixture.GetValidVideo();
        var validPath = _fixture.GetValidMediaPath();

        // When
        validVideo.UpdateMedia(validPath);

        // Then
        validVideo.Media.Should().NotBeNull();
        validVideo.Media!.FilePath.Should().Be(validPath);
    }

    [Fact(DisplayName = nameof(UpdateTrailer))]
    [Trait("Domain", "Video - Aggregates")]
    public void UpdateTrailer()
    {
        // Given
        var validVideo = _fixture.GetValidVideo();
        var validPath = _fixture.GetValidMediaPath();

        // When
        validVideo.UpdateTrailer(validPath);

        // Then
        validVideo.Trailer.Should().NotBeNull();
        validVideo.Trailer!.FilePath.Should().Be(validPath);
    }

    [Fact(DisplayName = nameof(UpdateAsSentToEncode))]
    [Trait("Domain", "Video - Aggregates")]
    public void UpdateAsSentToEncode()
    {
        // Given
        var validVideo = _fixture.GetValidVideo();
        var validPath = _fixture.GetValidMediaPath();
        validVideo.UpdateMedia(validPath);

        // When
        validVideo.UpdateAsSentToEncode();

        // Then
        validVideo.Media.Should().NotBeNull();
        validVideo.Media!.Status.Should().Be(MediaStatus.Processing);
    }

    [Fact(DisplayName = nameof(UpdateAsSentToEncodeThrowsWhenThereIsNoMedia))]
    [Trait("Domain", "Video - Aggregates")]
    public void UpdateAsSentToEncodeThrowsWhenThereIsNoMedia()
    {
        // Given
        var validVideo = _fixture.GetValidVideo();

        // When
        var action = () => validVideo.UpdateAsSentToEncode();

        // Then
        action.Should().Throw<EntityValidationException>()
            .WithMessage("There is no media.");
    }

    [Fact(DisplayName = nameof(UpdateAsEncoded))]
    [Trait("Domain", "Video - Aggregates")]
    public void UpdateAsEncoded()
    {
        // Given
        var validVideo = _fixture.GetValidVideo();
        var validPath = _fixture.GetValidMediaPath();
        validVideo.UpdateMedia(validPath);
        var encodedExamplePath = _fixture.GetValidMediaPath();

        // When
        validVideo.UpdateAsEncoded(encodedExamplePath);

        // Then
        validVideo.Media!.Status.Should().Be(MediaStatus.Completed);
        validVideo.Media.EncodedPath.Should().Be(encodedExamplePath);
    }

    [Fact(DisplayName = nameof(UpdateAsEncodedThrowsWhenThereIsNoMedia))]
    [Trait("Domain", "Video - Aggregates")]
    public void UpdateAsEncodedThrowsWhenThereIsNoMedia()
    {
        // Given
        var validVideo = _fixture.GetValidVideo();

        // When
        var action = () => validVideo.UpdateAsEncoded(_fixture.GetValidMediaPath());

        // Then
        action.Should().Throw<EntityValidationException>()
            .WithMessage("There is no media.");
    }

    [Fact(DisplayName = nameof(AddCategory))]
    [Trait("Domain", "Video - Aggregates")]
    public void AddCategory()
    {
        // Given
        var validVideo = _fixture.GetValidVideo();
        var categoryExample = Guid.NewGuid();

        // When
        validVideo.AddCategory(categoryExample);

        // Then
        validVideo.Categories.Should().HaveCount(1);
        validVideo.Categories[0].Should().Be(categoryExample);
    }

    [Fact(DisplayName = nameof(RemoveCategory))]
    [Trait("Domain", "Video - Aggregates")]
    public void RemoveCategory()
    {
        // Given
        var validVideo = _fixture.GetValidVideo();
        var categoryExample = Guid.NewGuid();
        validVideo.AddCategory(categoryExample);
        var categoryExample2 = Guid.NewGuid();
        validVideo.AddCategory(categoryExample2);

        // When
        validVideo.RemoveCategory(categoryExample);

        // Then
        validVideo.Categories.Should().HaveCount(1);
        validVideo.Categories[0].Should().Be(categoryExample2);
    }

    [Fact(DisplayName = nameof(RemoveAllCategories))]
    [Trait("Domain", "Video - Aggregates")]
    public void RemoveAllCategories()
    {
        // Given
        var validVideo = _fixture.GetValidVideo();        
        validVideo.AddCategory(Guid.NewGuid());
        validVideo.AddCategory(Guid.NewGuid());
        validVideo.AddCategory(Guid.NewGuid());

        // When
        validVideo.RemoveAllCategories();

        // Then
        validVideo.Categories.Should().HaveCount(0);
    }

    [Fact(DisplayName = nameof(AddGenre))]
    [Trait("Domain", "Video - Aggregates")]
    public void AddGenre()
    {
        // Given
        var validVideo = _fixture.GetValidVideo();
        var genreExample = Guid.NewGuid();

        // When
        validVideo.AddGenre(genreExample);

        // Then
        validVideo.Genres.Should().HaveCount(1);
        validVideo.Genres[0].Should().Be(genreExample);
    }

    [Fact(DisplayName = nameof(RemoveGenre))]
    [Trait("Domain", "Video - Aggregates")]
    public void RemoveGenre()
    {
        // Given
        var validVideo = _fixture.GetValidVideo();
        var genreExample = Guid.NewGuid();
        validVideo.AddGenre(genreExample);
        var genreExample2 = Guid.NewGuid();
        validVideo.AddGenre(genreExample2);

        // When
        validVideo.RemoveGenre(genreExample);

        // Then
        validVideo.Genres.Should().HaveCount(1);
        validVideo.Genres[0].Should().Be(genreExample2);
    }

    [Fact(DisplayName = nameof(RemoveAllGenres))]
    [Trait("Domain", "Video - Aggregates")]
    public void RemoveAllGenres()
    {
        // Given
        var validVideo = _fixture.GetValidVideo();        
        validVideo.AddGenre(Guid.NewGuid());
        validVideo.AddGenre(Guid.NewGuid());
        validVideo.AddGenre(Guid.NewGuid());

        // When
        validVideo.RemoveAllGenres();

        // Then
        validVideo.Genres.Should().HaveCount(0);
    }

    [Fact(DisplayName = nameof(AddCastMember))]
    [Trait("Domain", "Video - Aggregates")]
    public void AddCastMember()
    {
        // Given
        var validVideo = _fixture.GetValidVideo();
        var castMemberExample = Guid.NewGuid();

        // When
        validVideo.AddCastMember(castMemberExample);

        // Then
        validVideo.CastMembers.Should().HaveCount(1);
        validVideo.CastMembers[0].Should().Be(castMemberExample);
    }

    [Fact(DisplayName = nameof(RemoveCastMember))]
    [Trait("Domain", "Video - Aggregates")]
    public void RemoveCastMember()
    {
        // Given
        var validVideo = _fixture.GetValidVideo();
        var castMemberExample = Guid.NewGuid();
        validVideo.AddCastMember(castMemberExample);
        var castMemberExample2 = Guid.NewGuid();
        validVideo.AddCastMember(castMemberExample2);

        // When
        validVideo.RemoveCastMember(castMemberExample);

        // Then
        validVideo.CastMembers.Should().HaveCount(1);
        validVideo.CastMembers[0].Should().Be(castMemberExample2);
    }

    [Fact(DisplayName = nameof(RemoveAllCastMembers))]
    [Trait("Domain", "Video - Aggregates")]
    public void RemoveAllCastMembers()
    {
        // Given
        var validVideo = _fixture.GetValidVideo();        
        validVideo.AddCastMember(Guid.NewGuid());
        validVideo.AddCastMember(Guid.NewGuid());
        validVideo.AddCastMember(Guid.NewGuid());

        // When
        validVideo.RemoveAllCastMembers();

        // Then
        validVideo.CastMembers.Should().HaveCount(0);
    }
}