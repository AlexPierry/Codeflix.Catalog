using Application.Exceptions;
using Application.UseCases.Video.Common;
using Domain.Repository;

namespace Application.UseCases.Video.GetVideo;

public class GetVideo : IGetVideo
{
    private readonly IVideoRepository _videoRepository;

    public GetVideo(IVideoRepository videoRepository)
    {
        _videoRepository = videoRepository;
    }

    public async Task<VideoModelOutput> Handle(GetVideoInput input, CancellationToken cancellationToken)
    {
        var video = await _videoRepository.Get(input.Id, cancellationToken);
        if (video is null)
            throw new NotFoundException($"Video with id {input.Id} not found");

        return VideoModelOutput.FromVideo(video);
    }
}
