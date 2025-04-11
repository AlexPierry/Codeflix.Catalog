using Application.Common;
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
    private readonly Mock<IStorageService> _storageServiceMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public UpdateVideoTest(UpdateVideoTestFixture fixture)
    {
        _fixture = fixture;
        _videoRepositoryMock = new Mock<IVideoRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _genreRepositoryMock = new Mock<IGenreRepository>();
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _castMemberRepositoryMock = new Mock<ICastMemberRepository>();
        _storageServiceMock = new Mock<IStorageService>();
        _useCase = new UpdateVideo(
            _videoRepositoryMock.Object,
            _genreRepositoryMock.Object,
            _categoryRepositoryMock.Object,
            _castMemberRepositoryMock.Object,
            _storageServiceMock.Object,
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

    [Fact(DisplayName = nameof(UpdateVideoWithoutRelationsWithRelations))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateVideoWithoutRelationsWithRelations()
    {
        // Arrange
        var exampleVideo = _fixture.GetValidVideo();
        var exampleGenreIds = Enumerable.Range(1, 5).Select(x => Guid.NewGuid()).ToList();
        var exampleCategoryIds = Enumerable.Range(1, 5).Select(x => Guid.NewGuid()).ToList();
        var exampleCastMemberIds = Enumerable.Range(1, 5).Select(x => Guid.NewGuid()).ToList();

        var input = _fixture.GetUpdateVideoInput(exampleVideo.Id, true, exampleGenreIds, exampleCategoryIds, exampleCastMemberIds);
        _videoRepositoryMock
            .Setup(x => x.Get(exampleVideo.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);

        _genreRepositoryMock.Setup(x => x.GetIdsListByIds(
            It.Is<List<Guid>>(list => list.Count == exampleGenreIds.Count && list.All(id => exampleGenreIds.Contains(id))),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleGenreIds);

        _categoryRepositoryMock.Setup(x => x.GetIdsListByIds(
            It.Is<List<Guid>>(list => list.Count == exampleCategoryIds.Count && list.All(id => exampleCategoryIds.Contains(id))),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleCategoryIds);

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
        output.Genres.Should().NotBeNullOrEmpty();
        output.Genres!.Count.Should().Be(exampleGenreIds.Count);
        output.Genres!.All(g => exampleGenreIds.Contains(g.Id)).Should().BeTrue();
        output.Categories.Should().NotBeNullOrEmpty();
        output.Categories!.Count.Should().Be(exampleCategoryIds.Count);
        output.Categories!.All(c => exampleCategoryIds.Contains(c.Id)).Should().BeTrue();
        output.CastMembers.Should().NotBeNullOrEmpty();
        output.CastMembers!.Count.Should().Be(exampleCastMemberIds.Count);
        output.CastMembers!.All(c => exampleCastMemberIds.Contains(c.Id)).Should().BeTrue();

        _videoRepositoryMock.Verify(x => x.Update(
            It.Is<Entities.Video>(v => v.Id == input.Id),
            It.IsAny<CancellationToken>()), Times.Once);

        _genreRepositoryMock.Verify(x => x.GetIdsListByIds(
            It.Is<List<Guid>>(list => list.Count == exampleGenreIds.Count && list.All(id => exampleGenreIds.Contains(id))),
            It.IsAny<CancellationToken>()), Times.Once);

        _categoryRepositoryMock.Verify(x => x.GetIdsListByIds(
            It.Is<List<Guid>>(list => list.Count == exampleCategoryIds.Count && list.All(id => exampleCategoryIds.Contains(id))),
            It.IsAny<CancellationToken>()), Times.Once);

        _castMemberRepositoryMock.Verify(x => x.GetIdsListByIds(
            It.Is<List<Guid>>(list => list.Count == exampleCastMemberIds.Count && list.All(id => exampleCastMemberIds.Contains(id))),
            It.IsAny<CancellationToken>()), Times.Once);

        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = nameof(UpdateVideoChangeRelations))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateVideoChangeRelations()
    {
        // Arrange
        var exampleVideo = _fixture.GetValidVideoWithAllProperties();
        var exampleGenreIds = Enumerable.Range(1, 5).Select(x => Guid.NewGuid()).ToList();
        var exampleCategoryIds = Enumerable.Range(1, 5).Select(x => Guid.NewGuid()).ToList();
        var exampleCastMemberIds = Enumerable.Range(1, 5).Select(x => Guid.NewGuid()).ToList();

        var input = _fixture.GetUpdateVideoInput(exampleVideo.Id, true, exampleGenreIds, exampleCategoryIds, exampleCastMemberIds);
        _videoRepositoryMock
            .Setup(x => x.Get(exampleVideo.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);

        _genreRepositoryMock.Setup(x => x.GetIdsListByIds(
            It.Is<List<Guid>>(list => list.Count == exampleGenreIds.Count && list.All(id => exampleGenreIds.Contains(id))),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleGenreIds);

        _categoryRepositoryMock.Setup(x => x.GetIdsListByIds(
            It.Is<List<Guid>>(list => list.Count == exampleCategoryIds.Count && list.All(id => exampleCategoryIds.Contains(id))),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleCategoryIds);

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
        output.Genres.Should().NotBeNullOrEmpty();
        output.Genres!.Count.Should().Be(exampleGenreIds.Count);
        output.Genres!.All(g => exampleGenreIds.Contains(g.Id)).Should().BeTrue();
        output.Categories.Should().NotBeNullOrEmpty();
        output.Categories!.Count.Should().Be(exampleCategoryIds.Count);
        output.Categories!.All(c => exampleCategoryIds.Contains(c.Id)).Should().BeTrue();
        output.CastMembers.Should().NotBeNullOrEmpty();
        output.CastMembers!.Count.Should().Be(exampleCastMemberIds.Count);
        output.CastMembers!.All(c => exampleCastMemberIds.Contains(c.Id)).Should().BeTrue();

        _videoRepositoryMock.Verify(x => x.Update(
            It.Is<Entities.Video>(v => v.Id == input.Id),
            It.IsAny<CancellationToken>()), Times.Once);

        _genreRepositoryMock.Verify(x => x.GetIdsListByIds(
            It.Is<List<Guid>>(list => list.Count == exampleGenreIds.Count && list.All(id => exampleGenreIds.Contains(id))),
            It.IsAny<CancellationToken>()), Times.Once);

        _categoryRepositoryMock.Verify(x => x.GetIdsListByIds(
            It.Is<List<Guid>>(list => list.Count == exampleCategoryIds.Count && list.All(id => exampleCategoryIds.Contains(id))),
            It.IsAny<CancellationToken>()), Times.Once);

        _castMemberRepositoryMock.Verify(x => x.GetIdsListByIds(
            It.Is<List<Guid>>(list => list.Count == exampleCastMemberIds.Count && list.All(id => exampleCastMemberIds.Contains(id))),
            It.IsAny<CancellationToken>()), Times.Once);

        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = nameof(UpdateVideoRemoveRelations))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateVideoRemoveRelations()
    {
        // Arrange
        var exampleVideo = _fixture.GetValidVideoWithAllProperties();
        var input = _fixture.GetUpdateVideoInput(exampleVideo.Id, true, new(), new(), new());
        _videoRepositoryMock
            .Setup(x => x.Get(exampleVideo.Id, It.IsAny<CancellationToken>()))
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
        output.Genres.Should().BeNullOrEmpty();
        output.Categories.Should().BeNullOrEmpty();
        output.CastMembers.Should().BeNullOrEmpty();

        _videoRepositoryMock.Verify(x => x.Update(
            It.Is<Entities.Video>(v => v.Id == input.Id),
            It.IsAny<CancellationToken>()), Times.Once);

        _genreRepositoryMock.Verify(x => x.GetIdsListByIds(
            It.IsAny<List<Guid>>(),
            It.IsAny<CancellationToken>()), Times.Never);

        _categoryRepositoryMock.Verify(x => x.GetIdsListByIds(
            It.IsAny<List<Guid>>(),
            It.IsAny<CancellationToken>()), Times.Never);

        _castMemberRepositoryMock.Verify(x => x.GetIdsListByIds(
            It.IsAny<List<Guid>>(),
            It.IsAny<CancellationToken>()), Times.Never);

        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = nameof(UpdateVideoKeepRelationsWhenReceiveNull))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateVideoKeepRelationsWhenReceiveNull()
    {
        // Arrange
        var exampleVideo = _fixture.GetValidVideoWithAllProperties();
        var input = _fixture.GetUpdateVideoInput(exampleVideo.Id, true, null, null, null);
        _videoRepositoryMock
            .Setup(x => x.Get(exampleVideo.Id, It.IsAny<CancellationToken>()))
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
        output.Genres.Should().NotBeNullOrEmpty();
        output.Genres!.Count.Should().Be(exampleVideo.Genres.Count);
        output.Genres!.All(g => exampleVideo.Genres.Contains(g.Id)).Should().BeTrue();
        output.Categories.Should().NotBeNullOrEmpty();
        output.Categories!.Count.Should().Be(exampleVideo.Categories.Count);
        output.Categories!.All(c => exampleVideo.Categories.Contains(c.Id)).Should().BeTrue();
        output.CastMembers.Should().NotBeNullOrEmpty();
        output.CastMembers!.Count.Should().Be(exampleVideo.CastMembers.Count);
        output.CastMembers!.All(c => exampleVideo.CastMembers.Contains(c.Id)).Should().BeTrue();

        _videoRepositoryMock.Verify(x => x.Update(
            It.Is<Entities.Video>(v => v.Id == input.Id),
            It.IsAny<CancellationToken>()), Times.Once);

        _genreRepositoryMock.Verify(x => x.GetIdsListByIds(
            It.IsAny<List<Guid>>(),
            It.IsAny<CancellationToken>()), Times.Never);

        _categoryRepositoryMock.Verify(x => x.GetIdsListByIds(
            It.IsAny<List<Guid>>(),
            It.IsAny<CancellationToken>()), Times.Never);

        _castMemberRepositoryMock.Verify(x => x.GetIdsListByIds(
            It.IsAny<List<Guid>>(),
            It.IsAny<CancellationToken>()), Times.Never);

        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = nameof(UpdateVideoAddBanner))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateVideoAddBanner()
    {
        // Arrange
        var exampleVideo = _fixture.GetValidVideo();
        var input = _fixture.GetUpdateVideoInput(exampleVideo.Id, true, banner: _fixture.GetValidImageFileInput());
        var bannerPath = $"storage/banner.{input.Banner!.Extension}";
        _videoRepositoryMock
            .Setup(x => x.Get(exampleVideo.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);

        _storageServiceMock
            .Setup(x => x.Upload(
                It.Is<string>(name => name == StorageFileName.Create(exampleVideo.Id, nameof(input.Banner), input.Banner.Extension)),
                It.IsAny<Stream>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(bannerPath);

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
        output.BannerFileUrl.Should().NotBeNullOrEmpty();
        output.BannerFileUrl.Should().Be(bannerPath);

        _videoRepositoryMock.Verify(x => x.Update(
            It.Is<Entities.Video>(v => v.Id == input.Id),
            It.IsAny<CancellationToken>()), Times.Once);

        _storageServiceMock.Verify(x => x.Upload(
            It.Is<string>(name => name == StorageFileName.Create(exampleVideo.Id, nameof(input.Banner), input.Banner.Extension)),
            It.IsAny<Stream>(),
            It.IsAny<CancellationToken>()), Times.Once);

        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = nameof(UpdateVideoShouldKeepBannerWhenReceiveNull))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateVideoShouldKeepBannerWhenReceiveNull()
    {
        // Arrange
        var exampleVideo = _fixture.GetValidVideo();
        var bannerPath = $"storage/banner.jpg";
        exampleVideo.UpdateBanner(bannerPath);

        var input = _fixture.GetUpdateVideoInput(exampleVideo.Id, true, banner: null);

        _videoRepositoryMock
            .Setup(x => x.Get(exampleVideo.Id, It.IsAny<CancellationToken>()))
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
        output.BannerFileUrl.Should().NotBeNullOrEmpty();
        output.BannerFileUrl.Should().Be(bannerPath);

        _videoRepositoryMock.Verify(x => x.Update(
            It.Is<Entities.Video>(v => v.Id == input.Id),
            It.IsAny<CancellationToken>()), Times.Once);

        _storageServiceMock.Verify(x => x.Upload(
            It.IsAny<string>(),
            It.IsAny<Stream>(),
            It.IsAny<CancellationToken>()), Times.Never);

        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = nameof(UpdateVideoAddThumb))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateVideoAddThumb()
    {
        // Arrange
        var exampleVideo = _fixture.GetValidVideo();
        var input = _fixture.GetUpdateVideoInput(exampleVideo.Id, true, thumb: _fixture.GetValidImageFileInput());
        var thumbPath = $"storage/thumb.{input.Thumb!.Extension}";
        _videoRepositoryMock
            .Setup(x => x.Get(exampleVideo.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);

        _storageServiceMock
            .Setup(x => x.Upload(
                It.Is<string>(name => name == StorageFileName.Create(exampleVideo.Id, nameof(input.Thumb), input.Thumb.Extension)),
                It.IsAny<Stream>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(thumbPath);

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
        output.ThumbFileUrl.Should().NotBeNullOrEmpty();
        output.ThumbFileUrl.Should().Be(thumbPath);

        _videoRepositoryMock.Verify(x => x.Update(
            It.Is<Entities.Video>(v => v.Id == input.Id),
            It.IsAny<CancellationToken>()), Times.Once);

        _storageServiceMock.Verify(x => x.Upload(
            It.Is<string>(name => name == StorageFileName.Create(exampleVideo.Id, nameof(input.Thumb), input.Thumb.Extension)),
            It.IsAny<Stream>(),
            It.IsAny<CancellationToken>()), Times.Once);

        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = nameof(UpdateVideoShouldKeepThumbWhenReceiveNull))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateVideoShouldKeepThumbWhenReceiveNull()
    {
        // Arrange
        var exampleVideo = _fixture.GetValidVideo();
        var thumbPath = $"storage/banner.jpg";
        exampleVideo.UpdateThumb(thumbPath);

        var input = _fixture.GetUpdateVideoInput(exampleVideo.Id, true, thumb: null);

        _videoRepositoryMock
            .Setup(x => x.Get(exampleVideo.Id, It.IsAny<CancellationToken>()))
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
        output.ThumbFileUrl.Should().NotBeNullOrEmpty();
        output.ThumbFileUrl.Should().Be(thumbPath);

        _videoRepositoryMock.Verify(x => x.Update(
            It.Is<Entities.Video>(v => v.Id == input.Id),
            It.IsAny<CancellationToken>()), Times.Once);

        _storageServiceMock.Verify(x => x.Upload(
            It.IsAny<string>(),
            It.IsAny<Stream>(),
            It.IsAny<CancellationToken>()), Times.Never);

        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = nameof(UpdateVideoAddHalfThumb))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateVideoAddHalfThumb()
    {
        // Arrange
        var exampleVideo = _fixture.GetValidVideo();
        var input = _fixture.GetUpdateVideoInput(exampleVideo.Id, true, thumbHalf: _fixture.GetValidImageFileInput());
        var thumbHalfPath = $"storage/thumb.{input.ThumbHalf!.Extension}";
        _videoRepositoryMock
            .Setup(x => x.Get(exampleVideo.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);

        _storageServiceMock
            .Setup(x => x.Upload(
                It.Is<string>(name => name == StorageFileName.Create(exampleVideo.Id, nameof(input.ThumbHalf), input.ThumbHalf.Extension)),
                It.IsAny<Stream>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(thumbHalfPath);

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
        output.ThumbHalfFileUrl.Should().NotBeNullOrEmpty();
        output.ThumbHalfFileUrl.Should().Be(thumbHalfPath);

        _videoRepositoryMock.Verify(x => x.Update(
            It.Is<Entities.Video>(v => v.Id == input.Id),
            It.IsAny<CancellationToken>()), Times.Once);

        _storageServiceMock.Verify(x => x.Upload(
            It.Is<string>(name => name == StorageFileName.Create(exampleVideo.Id, nameof(input.ThumbHalf), input.ThumbHalf.Extension)),
            It.IsAny<Stream>(),
            It.IsAny<CancellationToken>()), Times.Once);

        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = nameof(UpdateVideoShouldKeepThumbHalfWhenReceiveNull))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateVideoShouldKeepThumbHalfWhenReceiveNull()
    {
        // Arrange
        var exampleVideo = _fixture.GetValidVideo();
        var thumbHalfPath = $"storage/banner.jpg";
        exampleVideo.UpdateThumbHalf(thumbHalfPath);

        var input = _fixture.GetUpdateVideoInput(exampleVideo.Id, true, thumbHalf: null);

        _videoRepositoryMock
            .Setup(x => x.Get(exampleVideo.Id, It.IsAny<CancellationToken>()))
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
        output.ThumbHalfFileUrl.Should().NotBeNullOrEmpty();
        output.ThumbHalfFileUrl.Should().Be(thumbHalfPath);

        _videoRepositoryMock.Verify(x => x.Update(
            It.Is<Entities.Video>(v => v.Id == input.Id),
            It.IsAny<CancellationToken>()), Times.Once);

        _storageServiceMock.Verify(x => x.Upload(
            It.IsAny<string>(),
            It.IsAny<Stream>(),
            It.IsAny<CancellationToken>()), Times.Never);

        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }
}
