using Application.Interfaces;
using Google.Cloud.Storage.V1;
using Infra.Storage.Configuration;
using Microsoft.Extensions.Options;

namespace Infra.Storage.Services;

public class StorageService : IStorageService
{
    private readonly StorageClient _storageClient;
    private readonly StorageServiceOptions _storageOptions;

    public StorageService(StorageClient storageClient, IOptions<StorageServiceOptions> storageOptions)
    {
        _storageClient = storageClient;
        _storageOptions = storageOptions.Value;
    }

    public async Task Delete(string filePath, CancellationToken cancellationToken)
    {
        await _storageClient.DeleteObjectAsync(_storageOptions.BucketName, filePath, cancellationToken: cancellationToken);
    }

    public async Task<string> Upload(string fileName, Stream stream, string contentType, CancellationToken cancellationToken)
    {
        await _storageClient.UploadObjectAsync(
            _storageOptions.BucketName,
            fileName,
            contentType,
            stream,
            cancellationToken: cancellationToken);

        return fileName;
    }
}
