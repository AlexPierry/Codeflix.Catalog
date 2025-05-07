using System.Text;
using FluentAssertions;
using Google.Apis.Upload;
using Google.Cloud.Storage.V1;
using Infra.Storage.Configuration;
using Infra.Storage.Services;
using Microsoft.Extensions.Options;
using Moq;

namespace UnitTests.Infra.Storage;

[Collection(nameof(StorageServiceTestFixture))]
public class StorageServiceTest
{
    private readonly StorageServiceTestFixture _fixture;

    public StorageServiceTest(StorageServiceTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = "Upload")]
    [Trait("Infra.Storage", "StorageService")]
    public async Task Upload()
    {
        // Arrange
        var storageClientMock = new Mock<StorageClient>();
        storageClientMock
            .Setup(client => client.UploadObjectAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<Stream>(),
                It.IsAny<UploadObjectOptions>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<IProgress<IUploadProgress>>()))
            .Returns(Task.FromResult(new Google.Apis.Storage.v1.Data.Object()));

        var storageOptions = new StorageServiceOptions
        {
            BucketName = _fixture.GetBucketName(),
        };
        var options = Options.Create(storageOptions);
        var service = new StorageService(storageClientMock.Object, options);
        var fileName = _fixture.GetFileName();
        var fileStream = new MemoryStream(Encoding.UTF8.GetBytes(_fixture.GetFileContent()));
        var contentType = _fixture.GetContentType();

        // Act
        var filePath = await service.Upload(fileName, fileStream, contentType, CancellationToken.None);

        // Assert
        filePath.Should().NotBeNullOrEmpty();
        filePath.Should().Be(fileName);
        storageClientMock.Verify(client => client.UploadObjectAsync(
            It.Is<string>(x => x == storageOptions.BucketName),
            It.Is<string>(x => x == fileName),
            It.Is<string>(x => x == contentType),
            It.Is<Stream>(x => x == fileStream),
            It.IsAny<UploadObjectOptions>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<IProgress<IUploadProgress>>()), Times.Once);
    }

    [Fact(DisplayName = "Delete")]
    [Trait("Infra.Storage", "StorageService")]
    public async Task Delete()
    {
        // Arrange
        var storageClientMock = new Mock<StorageClient>();
        storageClientMock
            .Setup(client => client.DeleteObjectAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<DeleteObjectOptions>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var storageOptions = new StorageServiceOptions
        {
            BucketName = _fixture.GetBucketName()
        };
        var options = Options.Create(storageOptions);
        var service = new StorageService(storageClientMock.Object, options);
        var fileName = _fixture.GetFileName();

        // Act
        await service.Delete(fileName, CancellationToken.None);

        // Assert
        storageClientMock.Verify(client => client.DeleteObjectAsync(
            It.Is<string>(x => x == storageOptions.BucketName),
            It.Is<string>(x => x == fileName),
            It.IsAny<DeleteObjectOptions>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
