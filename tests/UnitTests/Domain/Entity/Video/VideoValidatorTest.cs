using Domain.Validation;
using Domain.Validators;
using FluentAssertions;
using Entities = Domain.Entity;

namespace UnitTests.Domain.Entity.Video;

[Collection(nameof(VideoTestFixture))]
public class VideoValidatorTest
{
    private readonly VideoTestFixture _fixture;

    public VideoValidatorTest(VideoTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(ReturnsValidWhenVideoIsValid))]
    [Trait("Domain", "Video Validator - Validators")]
    public void ReturnsValidWhenVideoIsValid()
    {
        // Given
        var validVideo = _fixture.GetValidVideo();
        var notificationValidationHandler = new NotificationValidationHandler();
        var videoValidator = new VideoValidator(validVideo, notificationValidationHandler);

        // When        
        videoValidator.Validate();

        // Then        
        notificationValidationHandler.HasErrors().Should().BeFalse();
        notificationValidationHandler.Errors().Should().HaveCount(0);
    }

    [Fact(DisplayName = nameof(ReturnsErrorWhenTitleIsTooLong))]
    [Trait("Domain", "Video Validator - Validators")]
    public void ReturnsErrorWhenTitleIsTooLong()
    {
        // Given
        var invalidVideo = new Entities.Video(
            _fixture.GetTooLongTitle(),
            _fixture.GetValidDescription(),
            _fixture.GetValidOpened(),
            _fixture.GetValidPublished(),
            _fixture.GetValidYear(),
            _fixture.GetValidDuration(),
            _fixture.GetRandomMovieRating()
        );

        var notificationValidationHandler = new NotificationValidationHandler();
        var videoValidator = new VideoValidator(invalidVideo, notificationValidationHandler);

        // When        
        videoValidator.Validate();

        // Then        
        notificationValidationHandler.HasErrors().Should().BeTrue();
        notificationValidationHandler.Errors().Should().HaveCount(1);
        notificationValidationHandler.Errors().First().Message.Should().Be("'Title' should be less or equal 255 characters");
    }

    [Fact(DisplayName = nameof(ReturnsErrorWhenTitleIsEmpty))]
    [Trait("Domain", "Video Validator - Validators")]
    public void ReturnsErrorWhenTitleIsEmpty()
    {
        // Given
        var invalidVideo = new Entities.Video(
            string.Empty,
            _fixture.GetValidDescription(),
            _fixture.GetValidOpened(),
            _fixture.GetValidPublished(),
            _fixture.GetValidYear(),
            _fixture.GetValidDuration(),
            _fixture.GetRandomMovieRating()
        );

        var notificationValidationHandler = new NotificationValidationHandler();
        var videoValidator = new VideoValidator(invalidVideo, notificationValidationHandler);

        // When        
        videoValidator.Validate();

        // Then        
        notificationValidationHandler.HasErrors().Should().BeTrue();
        notificationValidationHandler.Errors().Should().HaveCount(1);
        notificationValidationHandler.Errors().First().Message.Should().Be("'Title' is required");
    }

    [Fact(DisplayName = nameof(ReturnsErrorWhenDescriptionIsTooLong))]
    [Trait("Domain", "Video Validator - Validators")]
    public void ReturnsErrorWhenDescriptionIsTooLong()
    {
        // Given
        var invalidVideo = new Entities.Video(
            _fixture.GetValidTitle(),
            _fixture.GetDescriptionTooLong(),
            _fixture.GetValidOpened(),
            _fixture.GetValidPublished(),
            _fixture.GetValidYear(),
            _fixture.GetValidDuration(),
            _fixture.GetRandomMovieRating()
        );

        var notificationValidationHandler = new NotificationValidationHandler();
        var videoValidator = new VideoValidator(invalidVideo, notificationValidationHandler);

        // When        
        videoValidator.Validate();

        // Then        
        notificationValidationHandler.HasErrors().Should().BeTrue();
        notificationValidationHandler.Errors().Should().HaveCount(1);
        notificationValidationHandler.Errors().First().Message.Should().Be("'Description' should be less or equal 4000 characters");
    }

    [Fact(DisplayName = nameof(ReturnsErrorWhenDescriptionIsEmpty))]
    [Trait("Domain", "Video Validator - Validators")]
    public void ReturnsErrorWhenDescriptionIsEmpty()
    {
        // Given
        var invalidVideo = new Entities.Video(
            _fixture.GetValidTitle(),
            string.Empty,
            _fixture.GetValidOpened(),
            _fixture.GetValidPublished(),
            _fixture.GetValidYear(),
            _fixture.GetValidDuration(),
            _fixture.GetRandomMovieRating()
        );

        var notificationValidationHandler = new NotificationValidationHandler();
        var videoValidator = new VideoValidator(invalidVideo, notificationValidationHandler);

        // When        
        videoValidator.Validate();

        // Then        
        notificationValidationHandler.HasErrors().Should().BeTrue();
        notificationValidationHandler.Errors().Should().HaveCount(1);
        notificationValidationHandler.Errors().First().Message.Should().Be("'Description' is required");
    }
}