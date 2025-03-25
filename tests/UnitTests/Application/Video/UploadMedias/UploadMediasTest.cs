using UseCases = Application.UseCases.Video.UploadMedias;
using Application.Interfaces;
using Domain.Repository;
using Moq;

namespace UnitTests.Application.Video.UploadMedias;

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
}
