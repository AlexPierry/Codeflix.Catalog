using Domain.Entity;
using Domain.Enum;
using FluentAssertions;

namespace UnitTests.Domain.Entity.Video;
[Collection(nameof(VideoTestFixture))]
public class MediaTest
{
    private readonly VideoTestFixture _fixture;

    public MediaTest(VideoTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(Instantiate))]
    [Trait("Domain", "Media - Entities")]
    public void Instantiate()
    {
        // Given
        var expectedFilePath = _fixture.GetValidMediaPath();

        // When
        var media = new Media(expectedFilePath);

        // Then
        media.Should().NotBeNull();
        media.FilePath.Should().Be(expectedFilePath);
        media.Status.Should().Be(MediaStatus.Pending);
    }

    [Fact(DisplayName = nameof(UpdateAsSentToEncode))]
    [Trait("Domain", "Media - Entities")]
    public void UpdateAsSentToEncode()
    {
        // Given
        var media = _fixture.GetValidMedia();

        // When
        media.UpdateAsSentToEncode();

        // Then
        media.Status.Should().Be(MediaStatus.Processing);
    }

    [Fact(DisplayName = nameof(UpdateAsEncoded))]
    [Trait("Domain", "Media - Entities")]
    public void UpdateAsEncoded()
    {
        // Given
        var media = _fixture.GetValidMedia();
        media.UpdateAsSentToEncode();
        var encodedExamplePath = _fixture.GetValidMediaPath();

        // When
        media.UpdateAsEncoded(encodedExamplePath);

        // Then
        media.Status.Should().Be(MediaStatus.Completed);
        media.EncodedPath.Should().Be(encodedExamplePath);
    }
}