using UseCases = Application.UseCases.Video.UploadMedias;
using Application.Interfaces;
using Domain.Repository;
using Moq;
using Application.Exceptions;
using FluentAssertions;
using Application.Common;

namespace UnitTests.Application.Video;

[Collection(nameof(UploadMediasTestFixture))]
public class UploadMediasTest
{
    private readonly UploadMediasTestFixture _fixture;
    private readonly UseCases.UploadMedias _useCase;
    private readonly Mock<IVideoRepository> _videoRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IStorageService> _storageServiceMock;

    public UploadMediasTest(UploadMediasTestFixture fixture)
    {
        _fixture = fixture;
        _videoRepositoryMock = new Mock<IVideoRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _storageServiceMock = new Mock<IStorageService>();

        _useCase = new UseCases.UploadMedias(
            _videoRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _storageServiceMock.Object
        );
    }

    [Fact(DisplayName = nameof(UploadMediasOk))]
    [Trait("Application", "UploadMedias - Uses Cases")]
    public async void UploadMediasOk()
    {
        // Arrange
        var video = _fixture.GetValidVideo();
        var validInput = _fixture.GetValidUploadMediasInput(video.Id);
        _videoRepositoryMock.Setup(x => x.Get(It.Is<Guid>(x => x == video.Id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(video);
        _storageServiceMock.Setup(x => x.Upload(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Guid.NewGuid().ToString());

        // Act
        await _useCase.Handle(validInput, CancellationToken.None);

        // Assert
        _videoRepositoryMock.VerifyAll();
        _unitOfWorkMock.Verify(x => x.Commit(CancellationToken.None), Times.Once);
        _storageServiceMock.Verify(x => x.Upload(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact(DisplayName = nameof(ThrowsWhenVideoNotFound))]
    [Trait("Application", "UploadMedias - Uses Cases")]
    public async void ThrowsWhenVideoNotFound()
    {
        // Arrange
        var video = _fixture.GetValidVideo();
        var validInput = _fixture.GetValidUploadMediasInput(video.Id);
        _videoRepositoryMock.Setup(x => x.Get(It.Is<Guid>(x => x == video.Id), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException("Video not found"));

        // Act
        var action = async () => await _useCase.Handle(validInput, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Video not found");
    }

    [Fact(DisplayName = nameof(ClearStorageInUploadErrorCase))]
    [Trait("Application", "UploadMedias - Uses Cases")]
    public async void ClearStorageInUploadErrorCase()
    {
        // Arrange
        var video = _fixture.GetValidVideo();
        var validInput = _fixture.GetValidUploadMediasInput(video.Id);
        var videoFileName = StorageFileName.Create(video.Id, nameof(video.Media), validInput.videoFile!.Extension);
        var trailerFileName = StorageFileName.Create(video.Id, nameof(video.Trailer), validInput.trailerFile!.Extension);
        _videoRepositoryMock.Setup(x => x.Get(It.Is<Guid>(x => x == video.Id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(video);
        _storageServiceMock.Setup(x => x.Upload(It.Is<string>(x => x == videoFileName), It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(videoFileName);
        _storageServiceMock.Setup(x => x.Upload(It.Is<string>(x => x == trailerFileName), It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Upload error"));

        // Act
        var action = async () => await _useCase.Handle(validInput, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<Exception>().WithMessage("Upload error");
        _videoRepositoryMock.VerifyAll();
        _unitOfWorkMock.Verify(x => x.Commit(CancellationToken.None), Times.Never);
        _storageServiceMock.Verify(x => x.Upload(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        _storageServiceMock.Verify(x => x.Delete(It.Is<string>(x => x == videoFileName), It.IsAny<CancellationToken>()), Times.Exactly(1));
    }

    [Fact(DisplayName = nameof(ClearStorageInCommitErrorCase))]
    [Trait("Application", "UploadMedias - Uses Cases")]
    public async void ClearStorageInCommitErrorCase()
    {
        // Arrange
        var video = _fixture.GetValidVideo();
        var validInput = _fixture.GetValidUploadMediasInput(video.Id);
        var videoFileName = StorageFileName.Create(video.Id, nameof(video.Media), validInput.videoFile!.Extension);
        var trailerFileName = StorageFileName.Create(video.Id, nameof(video.Trailer), validInput.trailerFile!.Extension);
        _videoRepositoryMock.Setup(x => x.Get(It.Is<Guid>(x => x == video.Id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(video);
        _storageServiceMock.Setup(x => x.Upload(It.Is<string>(x => x == videoFileName), It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(videoFileName);
        _storageServiceMock.Setup(x => x.Upload(It.Is<string>(x => x == trailerFileName), It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(trailerFileName);
        _unitOfWorkMock.Setup(x => x.Commit(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Commit error"));

        // Act
        var action = async () => await _useCase.Handle(validInput, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<Exception>().WithMessage("Commit error");
        _videoRepositoryMock.VerifyAll();
        _unitOfWorkMock.Verify(x => x.Commit(CancellationToken.None), Times.Once);
        _storageServiceMock.Verify(x => x.Upload(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        _storageServiceMock.Verify(x => x.Delete(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact(DisplayName = nameof(ClearStorageInCommitErrorCase))]
    [Trait("Application", "UploadMedias - Uses Cases")]
    public async void ClearOnlyOneStorageFileInCommitErrorCase()
    {
        // Arrange
        var video = _fixture.GetValidVideo();
        video.UpdateMedia(_fixture.GetValidMediaPath());
        video.UpdateTrailer(_fixture.GetValidMediaPath());
        var validInput = _fixture.GetValidUploadMediasInput(video.Id, false);
        var videoFileName = StorageFileName.Create(video.Id, nameof(video.Media), validInput.videoFile!.Extension);
        _videoRepositoryMock.Setup(x => x.Get(It.Is<Guid>(x => x == video.Id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(video);
        _storageServiceMock.Setup(x => x.Upload(It.Is<string>(x => x == videoFileName), It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(videoFileName);
        _unitOfWorkMock.Setup(x => x.Commit(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Commit error"));

        // Act
        var action = async () => await _useCase.Handle(validInput, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<Exception>().WithMessage("Commit error");
        _videoRepositoryMock.VerifyAll();
        _unitOfWorkMock.Verify(x => x.Commit(CancellationToken.None), Times.Once);
        _storageServiceMock.Verify(x => x.Upload(It.Is<string>(x => x == videoFileName), It.IsAny<Stream>(), It.IsAny<CancellationToken>()), Times.Exactly(1));
        _storageServiceMock.Verify(x => x.Delete(It.Is<string>(x => x == videoFileName), It.IsAny<CancellationToken>()), Times.Exactly(1));
    }
}
