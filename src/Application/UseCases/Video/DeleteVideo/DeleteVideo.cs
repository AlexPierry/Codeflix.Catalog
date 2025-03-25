using Application.Interfaces;
using Domain.Repository;

namespace Application.UseCases.Video.DeleteVideo;

public class DeleteVideo : IDeleteVideo
{
    private readonly IVideoRepository _videoRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IStorageService _storageService;

    public DeleteVideo(
        IVideoRepository videoRepository,
        IUnitOfWork unitOfWork,
        IStorageService storageService)
    {
        _videoRepository = videoRepository;
        _unitOfWork = unitOfWork;
        _storageService = storageService;
    }

    public async Task Handle(DeleteVideoInput input, CancellationToken cancellationToken)
    {
        var video = await _videoRepository.Get(input.videoId, cancellationToken);
        await _videoRepository.Delete(video, cancellationToken);
        await _unitOfWork.Commit(cancellationToken);

        if (video.Media is not null)
        {
            await _storageService.Delete(video.Media.FilePath, cancellationToken);
        }

        if (video.Trailer is not null)
        {
            await _storageService.Delete(video.Trailer.FilePath, cancellationToken);
        }
    }
}
