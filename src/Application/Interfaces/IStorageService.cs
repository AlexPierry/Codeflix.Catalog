using System;

namespace Application.Interfaces;

public interface IStorageService
{
    Task Delete(string v, CancellationToken cancellationToken);
    Task<string> Upload(string fileName, Stream stream, CancellationToken cancellationToken);
}
