using Application.Exceptions;
using Application.Interfaces;
using Application.UseCases.Video.UpdateVideo;
using Domain.Exceptions;
using Domain.Extensions;
using Domain.Repository;
using FluentAssertions;
using Moq;
using Entities = Domain.Entity;

namespace UnitTests.Application.Video;

[Collection(nameof(UpdateVideoTestFixture))]
public class UpdateVideoTest
{
    private readonly UpdateVideoTestFixture _fixture;
    private readonly IUpdateVideo _useCase;
    private readonly Mock<IVideoRepository> _videoRepositoryMock;
    private readonly Mock<IGenreRepository> _genreRepositoryMock;
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly Mock<ICastMemberRepository> _castMemberRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public UpdateVideoTest(UpdateVideoTestFixture fixture)
    {
        _fixture = fixture;
        _videoRepositoryMock = new Mock<IVideoRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _genreRepositoryMock = new Mock<IGenreRepository>();
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _castMemberRepositoryMock = new Mock<ICastMemberRepository>();
        _useCase = new UpdateVideo(
            _videoRepositoryMock.Object,
            _genreRepositoryMock.Object,
            _categoryRepositoryMock.Object,
            _castMemberRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact(DisplayName = nameof(UpdateVideo))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateVideo()
    {
        // Arrange
        var exampleVideo = _fixture.GetValidVideo();
        var input = _fixture.GetUpdateVideoInput(exampleVideo.Id, true);
        _videoRepositoryMock
            .Setup(x => x.Get(input.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);

        // Act
        var output = await _useCase.Handle(input, CancellationToken.None);

        // Assert
        output.Should().NotBeNull();
        output.Id.Should().Be(input.Id);
        output.Title.Should().Be(input.Title);
        output.Description.Should().Be(input.Description);
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.MovieRating!.Value.ToFriendlyString());
        output.Opened.Should().Be(input.Opened);
        output.Published.Should().Be(input.Published);

        _videoRepositoryMock.Verify(x => x.Update(
            It.Is<Entities.Video>(v => v.Id == input.Id),
            It.IsAny<CancellationToken>()),
        Times.Once);

        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = nameof(UpdateVideoThrowNotFound))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateVideoThrowNotFound()
    {
        // Arrange
        var input = _fixture.GetUpdateVideoInput(Guid.NewGuid(), true);
        _videoRepositoryMock
            .Setup(x => x.Get(input.Id, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException($"Video with id {input.Id} not found."));

        // Act
        var action = async () => await _useCase.Handle(input, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Video with id {input.Id} not found.");

        _videoRepositoryMock.Verify(x => x.Get(input.Id, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Never);
        _videoRepositoryMock.Verify(x => x.Update(It.IsAny<Entities.Video>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact(DisplayName = nameof(UpdateVideoWithoutMovieRating))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateVideoWithoutMovieRating()
    {
        // Arrange
        var exampleVideo = _fixture.GetValidVideo();
        var input = _fixture.GetUpdateVideoInput(exampleVideo.Id, false);
        _videoRepositoryMock
            .Setup(x => x.Get(input.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);

        // Act
        var output = await _useCase.Handle(input, CancellationToken.None);

        // Assert
        output.Should().NotBeNull();
        output.Id.Should().Be(input.Id);
        output.Title.Should().Be(input.Title);
        output.Description.Should().Be(input.Description);
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(exampleVideo.MovieRating.ToFriendlyString());
        output.Opened.Should().Be(input.Opened);
        output.Published.Should().Be(input.Published);

        _videoRepositoryMock.Verify(x => x.Update(
            It.Is<Entities.Video>(v => v.Id == input.Id),
            It.IsAny<CancellationToken>()),
        Times.Once);

        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory(DisplayName = nameof(UpdateVideoThrowsWithInvalidInput))]
    [Trait("Application", "UpdateVideo - Uses Cases")]
    [ClassData(typeof(UpdateVideoTestDataGenerator))]
    public async Task UpdateVideoThrowsWithInvalidInput(UpdateVideoInput input, string expectedValidationError)
    {
        // Arrange
        _videoRepositoryMock
            .Setup(x => x.Get(input.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_fixture.GetValidVideo());

        // Act
        var action = async () => await _useCase.Handle(input, CancellationToken.None);

        // Assert
        var exceptionAssertion = await action.Should().ThrowAsync<EntityValidationException>();
        exceptionAssertion.WithMessage("There are validation errors")
            .Which.Errors!.ToList()[0].Message.Should().Be(expectedValidationError);

        _videoRepositoryMock.Verify(x => x.Insert(It.IsAny<Entities.Video>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact(DisplayName = nameof(UpdateVideoWithGenreIds))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateVideoWithGenreIds()
    {
        // Arrange
        var exampleVideo = _fixture.GetValidVideo();
        var exampleGenreIds = Enumerable.Range(1, 5).Select(x => Guid.NewGuid()).ToList();

        var input = _fixture.GetUpdateVideoInput(exampleVideo.Id, true, exampleGenreIds);
        _videoRepositoryMock
            .Setup(x => x.Get(exampleVideo.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);

        _genreRepositoryMock.Setup(x => x.GetIdsListByIds(
            It.Is<List<Guid>>(list => list.Count == exampleGenreIds.Count && list.All(id => exampleGenreIds.Contains(id))),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleGenreIds);

        // Act
        var output = await _useCase.Handle(input, CancellationToken.None);

        // Assert
        output.Should().NotBeNull();
        output.Id.Should().Be(input.Id);
        output.Title.Should().Be(input.Title);
        output.Description.Should().Be(input.Description);
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.MovieRating!.Value.ToFriendlyString());
        output.Opened.Should().Be(input.Opened);
        output.Published.Should().Be(input.Published);
        output.Genres.Should().NotBeNullOrEmpty();
        output.Genres!.Count.Should().Be(exampleGenreIds.Count);
        output.Genres!.All(g => exampleGenreIds.Contains(g.Id)).Should().BeTrue();

        _videoRepositoryMock.Verify(x => x.Update(
            It.Is<Entities.Video>(v => v.Id == input.Id),
            It.IsAny<CancellationToken>()), Times.Once);

        _genreRepositoryMock.Verify(x => x.GetIdsListByIds(
            It.Is<List<Guid>>(list => list.Count == exampleGenreIds.Count && list.All(id => exampleGenreIds.Contains(id))),
            It.IsAny<CancellationToken>()), Times.Once);

        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = nameof(ThrowsWhenGenreIdInvalid))]
    [Trait("Application", "UpdateVideo - Uses Cases")]
    public async Task ThrowsWhenGenreIdInvalid()
    {
        // Arrange
        var exampleVideo = _fixture.GetValidVideo();
        var exampleGenreIds = Enumerable.Range(1, 5).Select(_ => Guid.NewGuid()).ToList();

        _genreRepositoryMock
            .Setup(x => x.GetIdsListByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleGenreIds.Take(3).ToList().AsReadOnly());

        _videoRepositoryMock
            .Setup(x => x.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);

        var input = _fixture.GetUpdateVideoInput(exampleVideo.Id, true, exampleGenreIds);

        // Act
        var action = async () => await _useCase.Handle(input, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<RelatedAggregateException>()
            .WithMessage($"Related genre id (or ids) not found: {string.Join(", ", exampleGenreIds.Skip(3))}");

        _videoRepositoryMock.Verify(x => x.Insert(It.IsAny<Entities.Video>(),
            It.IsAny<CancellationToken>()), Times.Never);

        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Never);

        _genreRepositoryMock.VerifyAll();
    }

    [Fact(DisplayName = nameof(UpdateVideoWithCategoryIds))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateVideoWithCategoryIds()
    {
        // Arrange
        var exampleVideo = _fixture.GetValidVideo();
        var exampleCategoryIds = Enumerable.Range(1, 5).Select(x => Guid.NewGuid()).ToList();

        var input = _fixture.GetUpdateVideoInput(exampleVideo.Id, true, categoryIds: exampleCategoryIds);
        _videoRepositoryMock
            .Setup(x => x.Get(exampleVideo.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);

        _categoryRepositoryMock.Setup(x => x.GetIdsListByIds(
            It.Is<List<Guid>>(list => list.Count == exampleCategoryIds.Count && list.All(id => exampleCategoryIds.Contains(id))),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleCategoryIds);

        // Act
        var output = await _useCase.Handle(input, CancellationToken.None);

        // Assert
        output.Should().NotBeNull();
        output.Id.Should().Be(input.Id);
        output.Title.Should().Be(input.Title);
        output.Description.Should().Be(input.Description);
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.MovieRating!.Value.ToFriendlyString());
        output.Opened.Should().Be(input.Opened);
        output.Published.Should().Be(input.Published);
        output.Genres.Should().BeNullOrEmpty();
        output.Categories.Should().NotBeNullOrEmpty();
        output.Categories!.Count.Should().Be(exampleCategoryIds.Count);
        output.Categories!.All(c => exampleCategoryIds.Contains(c.Id)).Should().BeTrue();

        _videoRepositoryMock.Verify(x => x.Update(
            It.Is<Entities.Video>(v => v.Id == input.Id),
            It.IsAny<CancellationToken>()), Times.Once);

        _categoryRepositoryMock.Verify(x => x.GetIdsListByIds(
            It.Is<List<Guid>>(list => list.Count == exampleCategoryIds.Count && list.All(id => exampleCategoryIds.Contains(id))),
            It.IsAny<CancellationToken>()), Times.Once);

        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = nameof(ThrowsWhenCategoryIdInvalid))]
    [Trait("Application", "UpdateVideo - Uses Cases")]
    public async Task ThrowsWhenCategoryIdInvalid()
    {
        // Arrange
        var exampleVideo = _fixture.GetValidVideo();
        var exampleCategoryIds = Enumerable.Range(1, 5).Select(_ => Guid.NewGuid()).ToList();

        _categoryRepositoryMock
            .Setup(x => x.GetIdsListByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleCategoryIds.Take(3).ToList().AsReadOnly());

        _videoRepositoryMock
            .Setup(x => x.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);

        var input = _fixture.GetUpdateVideoInput(exampleVideo.Id, true, categoryIds: exampleCategoryIds);

        // Act
        var action = async () => await _useCase.Handle(input, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<RelatedAggregateException>()
            .WithMessage($"Related category id (or ids) not found: {string.Join(", ", exampleCategoryIds.Skip(3))}");

        _videoRepositoryMock.Verify(x => x.Insert(It.IsAny<Entities.Video>(),
            It.IsAny<CancellationToken>()), Times.Never);

        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Never);

        _categoryRepositoryMock.VerifyAll();
    }

    [Fact(DisplayName = nameof(UpdateVideoWithCastMemberIds))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateVideoWithCastMemberIds()
    {
        // Arrange
        var exampleVideo = _fixture.GetValidVideo();
        var exampleCastMemberIds = Enumerable.Range(1, 5).Select(x => Guid.NewGuid()).ToList();

        var input = _fixture.GetUpdateVideoInput(exampleVideo.Id, true, castMemberIds: exampleCastMemberIds);
        _videoRepositoryMock
            .Setup(x => x.Get(exampleVideo.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);

        _castMemberRepositoryMock.Setup(x => x.GetIdsListByIds(
            It.Is<List<Guid>>(list => list.Count == exampleCastMemberIds.Count && list.All(id => exampleCastMemberIds.Contains(id))),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleCastMemberIds);

        // Act
        var output = await _useCase.Handle(input, CancellationToken.None);

        // Assert
        output.Should().NotBeNull();
        output.Id.Should().Be(input.Id);
        output.Title.Should().Be(input.Title);
        output.Description.Should().Be(input.Description);
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.MovieRating!.Value.ToFriendlyString());
        output.Opened.Should().Be(input.Opened);
        output.Published.Should().Be(input.Published);
        output.Genres.Should().BeNullOrEmpty();
        output.Categories.Should().BeNullOrEmpty();
        output.CastMembers.Should().NotBeNullOrEmpty();
        output.CastMembers!.Count.Should().Be(exampleCastMemberIds.Count);
        output.CastMembers!.All(c => exampleCastMemberIds.Contains(c.Id)).Should().BeTrue();

        _videoRepositoryMock.Verify(x => x.Update(
            It.Is<Entities.Video>(v => v.Id == input.Id),
            It.IsAny<CancellationToken>()), Times.Once);

        _castMemberRepositoryMock.Verify(x => x.GetIdsListByIds(
            It.Is<List<Guid>>(list => list.Count == exampleCastMemberIds.Count && list.All(id => exampleCastMemberIds.Contains(id))),
            It.IsAny<CancellationToken>()), Times.Once);

        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = nameof(ThrowsWhenCastMemberIdInvalid))]
    [Trait("Application", "UpdateVideo - Uses Cases")]
    public async Task ThrowsWhenCastMemberIdInvalid()
    {
        // Arrange
        var exampleVideo = _fixture.GetValidVideo();
        var exampleCastMemberIds = Enumerable.Range(1, 5).Select(_ => Guid.NewGuid()).ToList();

        _castMemberRepositoryMock
            .Setup(x => x.GetIdsListByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleCastMemberIds.Take(3).ToList().AsReadOnly());

        _videoRepositoryMock
            .Setup(x => x.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);

        var input = _fixture.GetUpdateVideoInput(exampleVideo.Id, true, castMemberIds: exampleCastMemberIds);

        // Act
        var action = async () => await _useCase.Handle(input, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<RelatedAggregateException>()
            .WithMessage($"Related cast member id (or ids) not found: {string.Join(", ", exampleCastMemberIds.Skip(3))}");

        _videoRepositoryMock.Verify(x => x.Insert(It.IsAny<Entities.Video>(),
            It.IsAny<CancellationToken>()), Times.Never);

        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Never);

        _castMemberRepositoryMock.VerifyAll();
    }    
}
