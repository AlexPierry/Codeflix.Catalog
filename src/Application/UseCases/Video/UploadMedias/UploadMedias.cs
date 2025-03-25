using Application.Common;
using Application.Interfaces;
using Domain.Repository;
using MediatR;

namespace Application.UseCases.Video.UploadMedias;

public class UploadMedias : IUploadMedias
{
    private readonly IVideoRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IStorageService _storageService;

    public UploadMedias(
        IVideoRepository repository,
        IUnitOfWork unitOfWork,
        IStorageService storageService)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _storageService = storageService;
    }

    public async Task Handle(UploadMediasInput input, CancellationToken cancellationToken)
    {
        var video = await _repository.Get(input.videoId, cancellationToken);
        try
        {
            await UploadVideo(input, video, cancellationToken);

            await UploadTrailer(input, video, cancellationToken);

            await _repository.Update(video, cancellationToken);
            await _unitOfWork.Commit(cancellationToken);
        }
        catch (Exception)
        {
            await ClearStorage(input, video, cancellationToken);
            throw;
        }
    }

    private async Task ClearStorage(UploadMediasInput input, Domain.Entity.Video video, CancellationToken cancellationToken)
    {
        if (input.videoFile is not null && video.Media is not null)
        {
            await _storageService.Delete(video.Media.FilePath, cancellationToken);
        }

        if (input.trailerFile is not null && video.Trailer is not null)
        {
            await _storageService.Delete(video.Trailer.FilePath, cancellationToken);
        }
    }

    private async Task UploadTrailer(UploadMediasInput input, Domain.Entity.Video video, CancellationToken cancellationToken)
    {
        if (input.trailerFile is not null)
        {
            var fileName = StorageFileName.Create(video.Id, nameof(video.Trailer), input.trailerFile.Extension);
            var uploadedFilePath = await _storageService.Upload(fileName, input.trailerFile.FileStream, cancellationToken);
            video.UpdateTrailer(uploadedFilePath);
        }
    }

    private async Task UploadVideo(UploadMediasInput input, Domain.Entity.Video video, CancellationToken cancellationToken)
    {
        if (input.videoFile is not null)
        {
            var fileName = StorageFileName.Create(video.Id, nameof(video.Media), input.videoFile.Extension);
            var uploadedFilePath = await _storageService.Upload(fileName, input.videoFile.FileStream, cancellationToken);
            video.UpdateMedia(uploadedFilePath);
        }
    }
}
