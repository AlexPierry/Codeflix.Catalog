
using Application.Exceptions;
using Application.UseCases.Video.GetVideo;
using Domain.Repository;
using FluentAssertions;
using Moq;

namespace UnitTests.Application.Video;

[Collection(nameof(GetVideoTestFixture))]
public class GetVideoTest
{
    private readonly GetVideoTestFixture _fixture;

    public GetVideoTest(GetVideoTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(GetVideoOk))]
    [Trait("Application", "GetVideo - Uses Cases")]
    public async Task GetVideoOk()
    {
        // Arrange
        var repositoryMock = new Mock<IVideoRepository>();
        var exampleVideo = _fixture.GetValidVideo();
        repositoryMock.Setup(x => x.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);
        var input = new GetVideoInput(Guid.NewGuid());
        var useCase = new GetVideo(repositoryMock.Object);

        // Act
        var output = await useCase.Handle(input, CancellationToken.None);

        // Assert
        output.Should().NotBeNull();
        output.Id.Should().Be(exampleVideo.Id);
        output.Title.Should().Be(exampleVideo.Title);
        output.Description.Should().Be(exampleVideo.Description);
        output.Published.Should().Be(exampleVideo.Published);
        output.Duration.Should().Be(exampleVideo.Duration);
        output.Rating.Should().Be(exampleVideo.MovieRating);
        output.YearLaunched.Should().Be(exampleVideo.YearLaunched);
        output.Opened.Should().Be(exampleVideo.Opened);
        output.CreatedAt.Should().Be(exampleVideo.CreatedAt);
        output.CategoriesIds.Should().BeEquivalentTo(exampleVideo.Categories);
        output.GenresIds.Should().BeEquivalentTo(exampleVideo.Genres);
        output.CastMembersIds.Should().BeEquivalentTo(exampleVideo.CastMembers);
        output.Thumb.Should().Be(exampleVideo.Thumb?.Path);
        output.Banner.Should().Be(exampleVideo.Banner?.Path);
        output.ThumbHalf.Should().Be(exampleVideo.ThumbHalf?.Path);
        output.Media.Should().Be(exampleVideo.Media?.FilePath);
        output.Trailer.Should().Be(exampleVideo.Trailer?.FilePath);

        repositoryMock.Verify(x => x.Get(It.Is<Guid>(x => x == input.Id), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = nameof(ThrowsExceptionWhenVideoNotFound))]
    [Trait("Application", "GetVideo - Uses Cases")]
    public async Task ThrowsExceptionWhenVideoNotFound()
    {
        // Arrange
        var repositoryMock = new Mock<IVideoRepository>();
        var input = new GetVideoInput(Guid.NewGuid());

        repositoryMock.Setup(x => x.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException($"Video with id {input.Id} not found"));

        var useCase = new GetVideo(repositoryMock.Object);

        // Act
        var action = () => useCase.Handle(input, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Video with id {input.Id} not found");

        repositoryMock.Verify(x => x.Get(It.Is<Guid>(x => x == input.Id), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = nameof(GetVideoWithAllProperties))]
    [Trait("Application", "GetVideo - Uses Cases")]
    public async Task GetVideoWithAllProperties()
    {
        // Arrange
        var repositoryMock = new Mock<IVideoRepository>();
        var exampleVideo = _fixture.GetValidVideoWithAllProperties();
        repositoryMock.Setup(x => x.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);
        var input = new GetVideoInput(Guid.NewGuid());
        var useCase = new GetVideo(repositoryMock.Object);

        // Act
        var output = await useCase.Handle(input, CancellationToken.None);

        // Assert
        output.Should().NotBeNull();
        output.Id.Should().Be(exampleVideo.Id);
        output.Title.Should().Be(exampleVideo.Title);
        output.Description.Should().Be(exampleVideo.Description);
        output.Published.Should().Be(exampleVideo.Published);
        output.Duration.Should().Be(exampleVideo.Duration);
        output.Rating.Should().Be(exampleVideo.MovieRating);
        output.YearLaunched.Should().Be(exampleVideo.YearLaunched);
        output.Opened.Should().Be(exampleVideo.Opened);
        output.CreatedAt.Should().Be(exampleVideo.CreatedAt);
        output.CategoriesIds.Should().BeEquivalentTo(exampleVideo.Categories);
        output.GenresIds.Should().BeEquivalentTo(exampleVideo.Genres);
        output.CastMembersIds.Should().BeEquivalentTo(exampleVideo.CastMembers);
        output.Thumb.Should().Be(exampleVideo.Thumb?.Path);
        output.Banner.Should().Be(exampleVideo.Banner?.Path);
        output.ThumbHalf.Should().Be(exampleVideo.ThumbHalf?.Path);
        output.Media.Should().Be(exampleVideo.Media?.FilePath);
        output.Trailer.Should().Be(exampleVideo.Trailer?.FilePath);

        repositoryMock.Verify(x => x.Get(It.Is<Guid>(x => x == input.Id), It.IsAny<CancellationToken>()), Times.Once);
    }
}
