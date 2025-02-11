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
}