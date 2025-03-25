using Application.Interfaces;
using Application.UseCases.Video.DeleteVideo;
using Domain.Repository;
using Moq;
using UseCases = Application.UseCases.Video.DeleteVideo;
using Entities = Domain.Entity;
using Xunit.Sdk;
using Application.Exceptions;
using FluentAssertions;

namespace UnitTests.Application.Video;

[Collection(nameof(DeleteVideoTestFixture))]
public class DeleteVideoTest
{
    private readonly DeleteVideoTestFixture _fixture;
    private readonly UseCases.DeleteVideo _useCase;
    private readonly Mock<IVideoRepository> _videoRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IStorageService> _storageServiceMock;

    public DeleteVideoTest(DeleteVideoTestFixture fixture)
    {
        _fixture = fixture;
        _videoRepositoryMock = new Mock<IVideoRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _storageServiceMock = new Mock<IStorageService>();

        _useCase = new DeleteVideo(
            _videoRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _storageServiceMock.Object
        );
    }

    [Fact(DisplayName = nameof(DeleteVideoOk))]
    [Trait("Application", "DeleteVideo - Uses Cases")]
    public async Task DeleteVideoOk()
    {
        // Arrange
        var validVideo = _fixture.GetValidVideo();
        var validInput = new DeleteVideoInput(validVideo.Id);
        _videoRepositoryMock.Setup(x => x.Get(It.Is<Guid>(x => x == validVideo.Id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(validVideo);

        // Act
        await _useCase.Handle(validInput, CancellationToken.None);

        // Assert
        _videoRepositoryMock.VerifyAll();
        _videoRepositoryMock.Verify(x => x.Delete(
            It.Is<Entities.Video>(video => video.Id == validVideo.Id),
            It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.Commit(CancellationToken.None), Times.Once);
        _storageServiceMock.Verify(x => x.Delete(
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact(DisplayName = nameof(DeleteVideoWithAllMediasAndCleanStorage))]
    [Trait("Application", "DeleteVideo - Uses Cases")]
    public async Task DeleteVideoWithAllMediasAndCleanStorage()
    {
        // Arrange
        var validVideo = _fixture.GetValidVideo();
        validVideo.UpdateMedia(_fixture.GetValidMediaPath());
        validVideo.UpdateTrailer(_fixture.GetValidMediaPath());
        var validInput = new DeleteVideoInput(validVideo.Id);
        _videoRepositoryMock.Setup(x => x.Get(It.Is<Guid>(x => x == validVideo.Id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(validVideo);
        _storageServiceMock.Setup(x => x.Delete(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _useCase.Handle(validInput, CancellationToken.None);

        // Assert
        _videoRepositoryMock.VerifyAll();
        _videoRepositoryMock.Verify(x => x.Delete(
            It.Is<Entities.Video>(video => video.Id == validVideo.Id),
            It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.Commit(CancellationToken.None), Times.Once);
        _storageServiceMock.Verify(x => x.Delete(
            It.Is<string>(x => x == validVideo.Media!.FilePath),
            It.IsAny<CancellationToken>()), Times.Once);
        _storageServiceMock.Verify(x => x.Delete(
            It.Is<string>(x => x == validVideo.Trailer!.FilePath),
            It.IsAny<CancellationToken>()), Times.Once);
        _storageServiceMock.Verify(x => x.Delete(
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact(DisplayName = nameof(DeleteVideoWithOnlyMediaAndCleanStorage))]
    [Trait("Application", "DeleteVideo - Uses Cases")]
    public async Task DeleteVideoWithOnlyMediaAndCleanStorage()
    {
        // Arrange
        var validVideo = _fixture.GetValidVideo();
        validVideo.UpdateMedia(_fixture.GetValidMediaPath());
        var validInput = new DeleteVideoInput(validVideo.Id);
        _videoRepositoryMock.Setup(x => x.Get(It.Is<Guid>(x => x == validVideo.Id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(validVideo);
        _storageServiceMock.Setup(x => x.Delete(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _useCase.Handle(validInput, CancellationToken.None);

        // Assert
        _videoRepositoryMock.VerifyAll();
        _videoRepositoryMock.Verify(x => x.Delete(
            It.Is<Entities.Video>(video => video.Id == validVideo.Id),
            It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.Commit(CancellationToken.None), Times.Once);
        _storageServiceMock.Verify(x => x.Delete(
            It.Is<string>(x => x == validVideo.Media!.FilePath),
            It.IsAny<CancellationToken>()), Times.Once);
        _storageServiceMock.Verify(x => x.Delete(
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = nameof(DeleteVideoWithOnlyTrailerAndCleanStorage))]
    [Trait("Application", "DeleteVideo - Uses Cases")]
    public async Task DeleteVideoWithOnlyTrailerAndCleanStorage()
    {
        // Arrange
        var validVideo = _fixture.GetValidVideo();
        validVideo.UpdateTrailer(_fixture.GetValidMediaPath());
        var validInput = new DeleteVideoInput(validVideo.Id);
        _videoRepositoryMock.Setup(x => x.Get(It.Is<Guid>(x => x == validVideo.Id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(validVideo);
        _storageServiceMock.Setup(x => x.Delete(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _useCase.Handle(validInput, CancellationToken.None);

        // Assert
        _videoRepositoryMock.VerifyAll();
        _videoRepositoryMock.Verify(x => x.Delete(
            It.Is<Entities.Video>(video => video.Id == validVideo.Id),
            It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.Commit(CancellationToken.None), Times.Once);
        _storageServiceMock.Verify(x => x.Delete(
            It.Is<string>(x => x == validVideo.Trailer!.FilePath),
            It.IsAny<CancellationToken>()), Times.Once);
        _storageServiceMock.Verify(x => x.Delete(
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = nameof(ThrowsExceptionWhenVideoNotFound))]
    [Trait("Application", "DeleteVideo - Uses Cases")]
    public async Task ThrowsExceptionWhenVideoNotFound()
    {
        // Arrange
        var validInput = new DeleteVideoInput(Guid.NewGuid());
        _videoRepositoryMock.Setup(x => x.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException("Video not found"));

        // Act
        var action = async () => await _useCase.Handle(validInput, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<NotFoundException>().WithMessage("Video not found");
        _videoRepositoryMock.Verify(x => x.Delete(
            It.IsAny<Entities.Video>(),
            It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.Commit(CancellationToken.None), Times.Never);
        _storageServiceMock.Verify(x => x.Delete(
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

}
