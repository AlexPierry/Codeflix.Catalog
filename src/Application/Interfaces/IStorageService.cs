using System;

namespace Application.Interfaces;

public interface IStorageService
{
    Task Delete(string filePath, CancellationToken cancellationToken);
    Task<string> Upload(string fileName, Stream stream, string contentType, CancellationToken cancellationToken);
}
