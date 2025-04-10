using Application.Interfaces;
using Domain.Repository;
using FluentAssertions;
using Moq;
using Entities = Domain.Entity;
using Domain.Exceptions;
using Application.UseCases.Video.CreateVideo;
using Application.Exceptions;
using Domain.Extensions;

namespace UnitTests.Application.Video;

[Collection(nameof(CreateVideoTestFixture))]
public class CreateVideoTest
{
    private readonly CreateVideoTestFixture _testFixture;

    public CreateVideoTest(CreateVideoTestFixture testFixture)
    {
        _testFixture = testFixture;
    }

    [Fact(DisplayName = nameof(CreateVideoOk))]
    [Trait("Application", "CreateVideo - Uses Cases")]
    public async Task CreateVideoOk()
    {
        // Arrange
        var videoRepositoryMock = new Mock<IVideoRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var useCase = new CreateVideo(
            videoRepositoryMock.Object,
            Mock.Of<ICategoryRepository>(),
            Mock.Of<IGenreRepository>(),
            Mock.Of<ICastMemberRepository>(),
            unitOfWorkMock.Object,
            Mock.Of<IStorageService>()
        );
        var input = _testFixture.GetValidCreateVideoInput();

        // Act
        var output = await useCase.Handle(input, CancellationToken.None);

        // Assert
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().BeSameDateAs(DateTime.Now);
        output.Title.Should().Be(input.Title);
        output.Description.Should().Be(input.Description);
        output.Published.Should().Be(input.Published);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.MovieRating.ToFriendlyString());
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);

        videoRepositoryMock.Verify(x => x.Insert(It.Is<Entities.Video>(
            video => video.Title == input.Title &&
            video.Description == input.Description &&
            video.Published == input.Published &&
            video.Duration == input.Duration &&
            video.MovieRating == input.MovieRating &&
            video.YearLaunched == input.YearLaunched &&
            video.Opened == input.Opened),
            It.IsAny<CancellationToken>()), Times.Once);

        unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = nameof(CreateVideoWithThumb))]
    [Trait("Application", "CreateVideo - Uses Cases")]
    public async Task CreateVideoWithThumb()
    {
        // Arrange
        var videoRepositoryMock = new Mock<IVideoRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var storageServiceMock = new Mock<IStorageService>();
        var expectedThumbName = "thumb.jpg";
        storageServiceMock.Setup(x => x.Upload(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedThumbName);
        var useCase = new CreateVideo(
            videoRepositoryMock.Object,
            Mock.Of<ICategoryRepository>(),
            Mock.Of<IGenreRepository>(),
            Mock.Of<ICastMemberRepository>(),
            unitOfWorkMock.Object,
            storageServiceMock.Object
        );
        var input = _testFixture.GetValidCreateVideoInput(thumb: _testFixture.GetValidImageFileInput());

        // Act
        var output = await useCase.Handle(input, CancellationToken.None);

        // Assert
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().BeSameDateAs(DateTime.Now);
        output.Title.Should().Be(input.Title);
        output.Description.Should().Be(input.Description);
        output.Published.Should().Be(input.Published);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.MovieRating.ToFriendlyString());
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
        output.ThumbFileUrl.Should().Be(expectedThumbName);

        videoRepositoryMock.Verify(x => x.Insert(It.Is<Entities.Video>(
            video => video.Title == input.Title &&
            video.Description == input.Description &&
            video.Published == input.Published &&
            video.Duration == input.Duration &&
            video.MovieRating == input.MovieRating &&
            video.YearLaunched == input.YearLaunched &&
            video.Opened == input.Opened),
            It.IsAny<CancellationToken>()), Times.Once);

        unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
        storageServiceMock.Verify(x => x.Upload(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = nameof(CreateVideoWithBanner))]
    [Trait("Application", "CreateVideo - Uses Cases")]
    public async Task CreateVideoWithBanner()
    {
        // Arrange
        var videoRepositoryMock = new Mock<IVideoRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var storageServiceMock = new Mock<IStorageService>();
        var expectedBannerName = "banner.jpg";
        storageServiceMock.Setup(x => x.Upload(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedBannerName);
        var useCase = new CreateVideo(
            videoRepositoryMock.Object,
            Mock.Of<ICategoryRepository>(),
            Mock.Of<IGenreRepository>(),
            Mock.Of<ICastMemberRepository>(),
            unitOfWorkMock.Object,
            storageServiceMock.Object
        );
        var input = _testFixture.GetValidCreateVideoInput(banner: _testFixture.GetValidImageFileInput());

        // Act
        var output = await useCase.Handle(input, CancellationToken.None);

        // Assert
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().BeSameDateAs(DateTime.Now);
        output.Title.Should().Be(input.Title);
        output.Description.Should().Be(input.Description);
        output.Published.Should().Be(input.Published);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.MovieRating.ToFriendlyString());
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
        output.BannerFileUrl.Should().Be(expectedBannerName);

        videoRepositoryMock.Verify(x => x.Insert(It.Is<Entities.Video>(
            video => video.Title == input.Title &&
            video.Description == input.Description &&
            video.Published == input.Published &&
            video.Duration == input.Duration &&
            video.MovieRating == input.MovieRating &&
            video.YearLaunched == input.YearLaunched &&
            video.Opened == input.Opened),
            It.IsAny<CancellationToken>()), Times.Once);

        unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
        storageServiceMock.Verify(x => x.Upload(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = nameof(CreateVideoWithThumbHalf))]
    [Trait("Application", "CreateVideo - Uses Cases")]
    public async Task CreateVideoWithThumbHalf()
    {
        // Arrange
        var videoRepositoryMock = new Mock<IVideoRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var storageServiceMock = new Mock<IStorageService>();
        var expectedThumbHalfName = "thumb_half.jpg";
        storageServiceMock.Setup(x => x.Upload(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedThumbHalfName);
        var useCase = new CreateVideo(
            videoRepositoryMock.Object,
            Mock.Of<ICategoryRepository>(),
            Mock.Of<IGenreRepository>(),
            Mock.Of<ICastMemberRepository>(),
            unitOfWorkMock.Object,
            storageServiceMock.Object
        );
        var input = _testFixture.GetValidCreateVideoInput(thumbHalf: _testFixture.GetValidImageFileInput());

        // Act
        var output = await useCase.Handle(input, CancellationToken.None);

        // Assert
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().BeSameDateAs(DateTime.Now);
        output.Title.Should().Be(input.Title);
        output.Description.Should().Be(input.Description);
        output.Published.Should().Be(input.Published);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.MovieRating.ToFriendlyString());
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
        output.ThumbHalfFileUrl.Should().Be(expectedThumbHalfName);

        videoRepositoryMock.Verify(x => x.Insert(It.Is<Entities.Video>(
            video => video.Title == input.Title &&
            video.Description == input.Description &&
            video.Published == input.Published &&
            video.Duration == input.Duration &&
            video.MovieRating == input.MovieRating &&
            video.YearLaunched == input.YearLaunched &&
            video.Opened == input.Opened),
            It.IsAny<CancellationToken>()), Times.Once);

        unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
        storageServiceMock.Verify(x => x.Upload(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = nameof(CreateVideoWithAllImages))]
    [Trait("Application", "CreateVideo - Uses Cases")]
    public async Task CreateVideoWithAllImages()
    {
        // Arrange
        var videoRepositoryMock = new Mock<IVideoRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var storageServiceMock = new Mock<IStorageService>();
        var expectedThumbHalfName = "thumb-half.jpg";
        var expectedThumbName = "banner.jpg";
        var expectedBannerName = "thumb.jpg";
        storageServiceMock.Setup(x => x.Upload(It.Is<string>(x => x.EndsWith("thumbhalf.jpg")), It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedThumbHalfName);
        storageServiceMock.Setup(x => x.Upload(It.Is<string>(x => x.EndsWith("thumb.jpg")), It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedThumbName);
        storageServiceMock.Setup(x => x.Upload(It.Is<string>(x => x.EndsWith("banner.jpg")), It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedBannerName);
        var useCase = new CreateVideo(
            videoRepositoryMock.Object,
            Mock.Of<ICategoryRepository>(),
            Mock.Of<IGenreRepository>(),
            Mock.Of<ICastMemberRepository>(),
            unitOfWorkMock.Object,
            storageServiceMock.Object
        );
        var input = _testFixture.GetCreateVideoInputWithAllImages();

        // Act
        var output = await useCase.Handle(input, CancellationToken.None);

        // Assert
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().BeSameDateAs(DateTime.Now);
        output.Title.Should().Be(input.Title);
        output.Description.Should().Be(input.Description);
        output.Published.Should().Be(input.Published);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.MovieRating.ToFriendlyString());
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
        output.BannerFileUrl.Should().Be(expectedBannerName);
        output.ThumbFileUrl.Should().Be(expectedThumbName);
        output.ThumbHalfFileUrl.Should().Be(expectedThumbHalfName);

        videoRepositoryMock.Verify(x => x.Insert(It.Is<Entities.Video>(
            video => video.Title == input.Title &&
            video.Description == input.Description &&
            video.Published == input.Published &&
            video.Duration == input.Duration &&
            video.MovieRating == input.MovieRating &&
            video.YearLaunched == input.YearLaunched &&
            video.Opened == input.Opened),
            It.IsAny<CancellationToken>()), Times.Once);

        unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
        storageServiceMock.Verify(x => x.Upload(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()), Times.Exactly(3));
    }

    [Fact(DisplayName = nameof(ThrowsExceptionInUploadErrorCases))]
    [Trait("Application", "CreateVideo - Uses Cases")]
    public async Task ThrowsExceptionInUploadErrorCases()
    {
        // Arrange
        var storageServiceMock = new Mock<IStorageService>();
        storageServiceMock.Setup(x => x.Upload(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Something went wrong in upload"));
        var useCase = new CreateVideo(
            Mock.Of<IVideoRepository>(),
            Mock.Of<ICategoryRepository>(),
            Mock.Of<IGenreRepository>(),
            Mock.Of<ICastMemberRepository>(),
            Mock.Of<IUnitOfWork>(),
            storageServiceMock.Object
        );
        var input = _testFixture.GetCreateVideoInputWithAllImages();

        // Act
        var action = async () => await useCase.Handle(input, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<Exception>()
            .WithMessage("Something went wrong in upload");
    }

    [Fact(DisplayName = nameof(ThrowsExceptionAndRollbackUploadInImagesErrorCases))]
    [Trait("Application", "CreateVideo - Uses Cases")]
    public async Task ThrowsExceptionAndRollbackUploadInImagesErrorCases()
    {
        // Arrange
        var storageServiceMock = new Mock<IStorageService>();
        storageServiceMock.Setup(x => x.Upload(It.Is<string>(x => x.EndsWith("banner.jpg")), It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("123-banner.jpg");
        storageServiceMock.Setup(x => x.Upload(It.Is<string>(x => x.EndsWith("thumb.jpg")), It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("123-thumb.jpg");
        storageServiceMock.Setup(x => x.Upload(It.Is<string>(x => x.EndsWith("thumbhalf.jpg")), It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Something went wrong in upload"));
        var useCase = new CreateVideo(
            Mock.Of<IVideoRepository>(),
            Mock.Of<ICategoryRepository>(),
            Mock.Of<IGenreRepository>(),
            Mock.Of<ICastMemberRepository>(),
            Mock.Of<IUnitOfWork>(),
            storageServiceMock.Object
        );
        var input = _testFixture.GetCreateVideoInputWithAllImages();

        // Act
        var action = async () => await useCase.Handle(input, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<Exception>()
            .WithMessage("Something went wrong in upload");

        storageServiceMock.Verify(x => x.Delete(It.Is<string>(x => x == "123-banner.jpg"), It.IsAny<CancellationToken>()), Times.Once);
        storageServiceMock.Verify(x => x.Delete(It.Is<string>(x => x == "123-thumb.jpg"), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory(DisplayName = nameof(CreateVideoThrowsWithInvalidInput))]
    [Trait("Application", "CreateVideo - Uses Cases")]
    [ClassData(typeof(CreateVideoTestDataGenerator))]
    public async Task CreateVideoThrowsWithInvalidInput(CreateVideoInput input, string expectedValidationError)
    {
        // Arrange
        var videoRepositoryMock = new Mock<IVideoRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var useCase = new CreateVideo(
            videoRepositoryMock.Object,
            Mock.Of<ICategoryRepository>(),
            Mock.Of<IGenreRepository>(),
            Mock.Of<ICastMemberRepository>(),
            unitOfWorkMock.Object,
            Mock.Of<IStorageService>());

        // Act
        var action = async () => await useCase.Handle(input, CancellationToken.None);

        // Assert
        var exceptionAssertion = await action.Should().ThrowAsync<EntityValidationException>();
        exceptionAssertion.WithMessage("There are validation errors")
            .Which.Errors!.ToList()[0].Message.Should().Be(expectedValidationError);

        videoRepositoryMock.Verify(x => x.Insert(It.IsAny<Entities.Video>(), It.IsAny<CancellationToken>()), Times.Never);
        unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact(DisplayName = nameof(CreateVideoWithCategoriesIds))]
    [Trait("Application", "CreateVideo - Uses Cases")]
    public async Task CreateVideoWithCategoriesIds()
    {
        // Arrange
        var videoRepositoryMock = new Mock<IVideoRepository>();
        var categoryRepositoryMock = new Mock<ICategoryRepository>();
        var exampleCategoriesIds = Enumerable.Range(1, 5).Select(_ => Guid.NewGuid()).ToList();
        categoryRepositoryMock.Setup(x => x.GetIdsListByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleCategoriesIds);
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var useCase = new CreateVideo(
            videoRepositoryMock.Object,
            categoryRepositoryMock.Object,
            Mock.Of<IGenreRepository>(),
            Mock.Of<ICastMemberRepository>(),
            unitOfWorkMock.Object,
            Mock.Of<IStorageService>());
        var input = _testFixture.GetValidCreateVideoInput(exampleCategoriesIds);

        // Act
        var output = await useCase.Handle(input, CancellationToken.None);

        // Assert
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().BeSameDateAs(DateTime.Now);
        output.Title.Should().Be(input.Title);
        output.Description.Should().Be(input.Description);
        output.Published.Should().Be(input.Published);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.MovieRating.ToFriendlyString());
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
        output.Categories.Select(c => c.Id).Should().BeEquivalentTo(exampleCategoriesIds);

        videoRepositoryMock.Verify(x => x.Insert(It.Is<Entities.Video>(
            video =>
                video.Id != Guid.Empty &&
                video.Title == input.Title &&
                video.Description == input.Description &&
                video.Published == input.Published &&
                video.Duration == input.Duration &&
                video.MovieRating == input.MovieRating &&
                video.YearLaunched == input.YearLaunched &&
                video.Opened == input.Opened &&
                video.Categories.All(categoryId => exampleCategoriesIds.Contains(categoryId))
            ),
            It.IsAny<CancellationToken>()), Times.Once);

        unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);

        categoryRepositoryMock.VerifyAll();
    }

    [Fact(DisplayName = nameof(ThrowsWhenCategoryIdInvalid))]
    [Trait("Application", "CreateVideo - Uses Cases")]
    public async Task ThrowsWhenCategoryIdInvalid()
    {
        // Arrange
        var videoRepositoryMock = new Mock<IVideoRepository>();
        var categoryRepositoryMock = new Mock<ICategoryRepository>();
        var exampleCategoriesIds = Enumerable.Range(1, 5).Select(_ => Guid.NewGuid()).ToList();
        categoryRepositoryMock.Setup(x => x.GetIdsListByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleCategoriesIds.Take(3).ToList().AsReadOnly());
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var useCase = new CreateVideo(
            videoRepositoryMock.Object,
            categoryRepositoryMock.Object,
            Mock.Of<IGenreRepository>(),
            Mock.Of<ICastMemberRepository>(),
            unitOfWorkMock.Object,
            Mock.Of<IStorageService>());

        var input = _testFixture.GetValidCreateVideoInput(exampleCategoriesIds);

        // Act
        var action = async () => await useCase.Handle(input, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<RelatedAggregateException>()
            .WithMessage($"Related category id (or ids) not found: {string.Join(", ", exampleCategoriesIds.Skip(3))}");

        videoRepositoryMock.Verify(x => x.Insert(It.IsAny<Entities.Video>(),
            It.IsAny<CancellationToken>()), Times.Never);

        unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Never);

        categoryRepositoryMock.VerifyAll();
    }

    [Fact(DisplayName = nameof(CreateVideoWithGenres))]
    [Trait("Application", "CreateVideo - Uses Cases")]
    public async Task CreateVideoWithGenres()
    {
        // Arrange
        var videoRepositoryMock = new Mock<IVideoRepository>();
        var genreRepositoryMock = new Mock<IGenreRepository>();
        var exampleIds = Enumerable.Range(1, 5).Select(_ => Guid.NewGuid()).ToList();
        genreRepositoryMock.Setup(x => x.GetIdsListByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleIds);
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var useCase = new CreateVideo(
            videoRepositoryMock.Object,
            Mock.Of<ICategoryRepository>(),
            genreRepositoryMock.Object,
            Mock.Of<ICastMemberRepository>(),
            unitOfWorkMock.Object,
            Mock.Of<IStorageService>());
        var input = _testFixture.GetValidCreateVideoInput(genresIds: exampleIds);

        // Act
        var output = await useCase.Handle(input, CancellationToken.None);

        // Assert
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().BeSameDateAs(DateTime.Now);
        output.Title.Should().Be(input.Title);
        output.Description.Should().Be(input.Description);
        output.Published.Should().Be(input.Published);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.MovieRating.ToFriendlyString());
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
        output.Categories.Should().BeEmpty();
        output.Genres.Select(g => g.Id).Should().BeEquivalentTo(exampleIds);

        videoRepositoryMock.Verify(x => x.Insert(It.Is<Entities.Video>(
            video =>
                video.Id != Guid.Empty &&
                video.Title == input.Title &&
                video.Description == input.Description &&
                video.Published == input.Published &&
                video.Duration == input.Duration &&
                video.MovieRating == input.MovieRating &&
                video.YearLaunched == input.YearLaunched &&
                video.Opened == input.Opened &&
                video.Genres.All(id => exampleIds.Contains(id))
            ),
            It.IsAny<CancellationToken>()), Times.Once);

        unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);

        genreRepositoryMock.VerifyAll();
    }

    [Fact(DisplayName = nameof(ThrowsWhenGenreIdInvalid))]
    [Trait("Application", "CreateVideo - Uses Cases")]
    public async Task ThrowsWhenGenreIdInvalid()
    {
        // Arrange
        var videoRepositoryMock = new Mock<IVideoRepository>();
        var genreRepositoryMock = new Mock<IGenreRepository>();
        var genreIds = Enumerable.Range(1, 5).Select(_ => Guid.NewGuid()).ToList();
        genreRepositoryMock.Setup(x => x.GetIdsListByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(genreIds.Take(3).ToList().AsReadOnly());
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var useCase = new CreateVideo(
            videoRepositoryMock.Object,
            Mock.Of<ICategoryRepository>(),
            genreRepositoryMock.Object,
            Mock.Of<ICastMemberRepository>(),
            unitOfWorkMock.Object,
            Mock.Of<IStorageService>());

        var input = _testFixture.GetValidCreateVideoInput(genresIds: genreIds);

        // Act
        var action = async () => await useCase.Handle(input, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<RelatedAggregateException>()
            .WithMessage($"Related genre id (or ids) not found: {string.Join(", ", genreIds.Skip(3))}");

        videoRepositoryMock.Verify(x => x.Insert(It.IsAny<Entities.Video>(),
            It.IsAny<CancellationToken>()), Times.Never);

        unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Never);

        genreRepositoryMock.VerifyAll();
    }

    [Fact(DisplayName = nameof(CreateVideoWithCastMembers))]
    [Trait("Application", "CreateVideo - Uses Cases")]
    public async Task CreateVideoWithCastMembers()
    {
        // Arrange
        var videoRepositoryMock = new Mock<IVideoRepository>();
        var castMemberRepositoryMock = new Mock<ICastMemberRepository>();
        var exampleIds = Enumerable.Range(1, 5).Select(_ => Guid.NewGuid()).ToList();
        castMemberRepositoryMock.Setup(x => x.GetIdsListByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleIds);
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var useCase = new CreateVideo(
            videoRepositoryMock.Object,
            Mock.Of<ICategoryRepository>(),
            Mock.Of<IGenreRepository>(),
            castMemberRepositoryMock.Object,
            unitOfWorkMock.Object,
            Mock.Of<IStorageService>());
        var input = _testFixture.GetValidCreateVideoInput(castMembersIds: exampleIds);

        // Act
        var output = await useCase.Handle(input, CancellationToken.None);

        // Assert
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().BeSameDateAs(DateTime.Now);
        output.Title.Should().Be(input.Title);
        output.Description.Should().Be(input.Description);
        output.Published.Should().Be(input.Published);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.MovieRating.ToFriendlyString());
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
        output.Categories.Should().BeEmpty();
        output.Genres.Should().BeEmpty();
        output.CastMembers.Select(cm => cm.Id).Should().BeEquivalentTo(exampleIds);

        videoRepositoryMock.Verify(x => x.Insert(It.Is<Entities.Video>(
            video =>
                video.Id != Guid.Empty &&
                video.Title == input.Title &&
                video.Description == input.Description &&
                video.Published == input.Published &&
                video.Duration == input.Duration &&
                video.MovieRating == input.MovieRating &&
                video.YearLaunched == input.YearLaunched &&
                video.Opened == input.Opened &&
                video.CastMembers.All(id => exampleIds.Contains(id))
            ),
            It.IsAny<CancellationToken>()), Times.Once);

        unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);

        castMemberRepositoryMock.VerifyAll();
    }

    [Fact(DisplayName = nameof(ThrowsWhenCastMemberIdInvalid))]
    [Trait("Application", "CreateVideo - Uses Cases")]
    public async Task ThrowsWhenCastMemberIdInvalid()
    {
        // Arrange
        var videoRepositoryMock = new Mock<IVideoRepository>();
        var castMemberRepositoryMock = new Mock<ICastMemberRepository>();
        var castMemberIds = Enumerable.Range(1, 5).Select(_ => Guid.NewGuid()).ToList();
        castMemberRepositoryMock.Setup(x => x.GetIdsListByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(castMemberIds.Take(3).ToList().AsReadOnly());
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var useCase = new CreateVideo(
            videoRepositoryMock.Object,
            Mock.Of<ICategoryRepository>(),
            Mock.Of<IGenreRepository>(),
            castMemberRepositoryMock.Object,
            unitOfWorkMock.Object,
            Mock.Of<IStorageService>());

        var input = _testFixture.GetValidCreateVideoInput(castMembersIds: castMemberIds);

        // Act
        var action = async () => await useCase.Handle(input, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<RelatedAggregateException>()
            .WithMessage($"Related cast member id (or ids) not found: {string.Join(", ", castMemberIds.Skip(3))}");

        videoRepositoryMock.Verify(x => x.Insert(It.IsAny<Entities.Video>(),
            It.IsAny<CancellationToken>()), Times.Never);

        unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Never);

        castMemberRepositoryMock.VerifyAll();
    }

    [Fact(DisplayName = nameof(CreateVideoWithMedia))]
    [Trait("Application", "CreateVideo - Uses Cases")]
    public async Task CreateVideoWithMedia()
    {
        // Arrange
        var videoRepositoryMock = new Mock<IVideoRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var storageServiceMock = new Mock<IStorageService>();
        var expectedMediaName = $"/storage/{_testFixture.GetValidMediaPath()}";
        storageServiceMock.Setup(x => x.Upload(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedMediaName);
        var useCase = new CreateVideo(
            videoRepositoryMock.Object,
            Mock.Of<ICategoryRepository>(),
            Mock.Of<IGenreRepository>(),
            Mock.Of<ICastMemberRepository>(),
            unitOfWorkMock.Object,
            storageServiceMock.Object
        );
        var input = _testFixture.GetValidCreateVideoInput(media: _testFixture.GetValidMediaFileInput());

        // Act
        var output = await useCase.Handle(input, CancellationToken.None);

        // Assert
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().BeSameDateAs(DateTime.Now);
        output.Title.Should().Be(input.Title);
        output.Description.Should().Be(input.Description);
        output.Published.Should().Be(input.Published);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.MovieRating.ToFriendlyString());
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
        output.VideoFileUrl.Should().Be(expectedMediaName);

        videoRepositoryMock.Verify(x => x.Insert(It.Is<Entities.Video>(
            video => video.Title == input.Title &&
            video.Description == input.Description &&
            video.Published == input.Published &&
            video.Duration == input.Duration &&
            video.MovieRating == input.MovieRating &&
            video.YearLaunched == input.YearLaunched &&
            video.Opened == input.Opened),
            It.IsAny<CancellationToken>()), Times.Once);

        unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
        storageServiceMock.Verify(x => x.Upload(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = nameof(CreateVideoWithTrailer))]
    [Trait("Application", "CreateVideo - Uses Cases")]
    public async Task CreateVideoWithTrailer()
    {
        // Arrange
        var videoRepositoryMock = new Mock<IVideoRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var storageServiceMock = new Mock<IStorageService>();
        var expectedTrailerName = $"/storage/{_testFixture.GetValidMediaPath()}";
        storageServiceMock.Setup(x => x.Upload(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedTrailerName);
        var useCase = new CreateVideo(
            videoRepositoryMock.Object,
            Mock.Of<ICategoryRepository>(),
            Mock.Of<IGenreRepository>(),
            Mock.Of<ICastMemberRepository>(),
            unitOfWorkMock.Object,
            storageServiceMock.Object
        );
        var input = _testFixture.GetValidCreateVideoInput(trailer: _testFixture.GetValidMediaFileInput());

        // Act
        var output = await useCase.Handle(input, CancellationToken.None);

        // Assert
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().BeSameDateAs(DateTime.Now);
        output.Title.Should().Be(input.Title);
        output.Description.Should().Be(input.Description);
        output.Published.Should().Be(input.Published);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.MovieRating.ToFriendlyString());
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
        output.TrailerFileUrl.Should().Be(expectedTrailerName);
        output.VideoFileUrl.Should().BeNull();

        videoRepositoryMock.Verify(x => x.Insert(It.Is<Entities.Video>(
            video => video.Title == input.Title &&
            video.Description == input.Description &&
            video.Published == input.Published &&
            video.Duration == input.Duration &&
            video.MovieRating == input.MovieRating &&
            video.YearLaunched == input.YearLaunched &&
            video.Opened == input.Opened),
            It.IsAny<CancellationToken>()), Times.Once);

        unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
        storageServiceMock.Verify(x => x.Upload(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = nameof(ThrowsExceptionAndRollbackUploadInMediaErrorCases))]
    [Trait("Application", "CreateVideo - Uses Cases")]
    public async Task ThrowsExceptionAndRollbackUploadInMediaErrorCases()
    {
        // Arrange
        var storageServiceMock = new Mock<IStorageService>();
        storageServiceMock.Setup(x => x.Upload(It.Is<string>(x => x.EndsWith("media.mp4")), It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("123-media.mp4");
        storageServiceMock.Setup(x => x.Upload(It.Is<string>(x => x.EndsWith("trailer.mp4")), It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("123-trailer.mp4");
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(x => x.Commit(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Something went wrong with the commit"));

        var useCase = new CreateVideo(
            Mock.Of<IVideoRepository>(),
            Mock.Of<ICategoryRepository>(),
            Mock.Of<IGenreRepository>(),
            Mock.Of<ICastMemberRepository>(),
            unitOfWorkMock.Object,
            storageServiceMock.Object
        );
        var input = _testFixture.GetCreateVideoInputWithAllMedias();

        // Act
        var action = async () => await useCase.Handle(input, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<Exception>()
            .WithMessage("Something went wrong with the commit");

        storageServiceMock.Verify(x => x.Delete(It.Is<string>(x => x == "123-media.mp4"), It.IsAny<CancellationToken>()), Times.Once);
        storageServiceMock.Verify(x => x.Delete(It.Is<string>(x => x == "123-trailer.mp4"), It.IsAny<CancellationToken>()), Times.Once);
        storageServiceMock.Verify(x => x.Delete(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }
}
