using Domain.Repository;

namespace Application.UseCases.Video.ListVideos;

public class ListVideos : IListVideos
{
    private readonly IVideoRepository _videoRepository;

    public ListVideos(IVideoRepository videoRepository)
    {
        _videoRepository = videoRepository;
    }

    public async Task<ListVideosOutput> Handle(ListVideosInput input, CancellationToken cancellationToken)
    {
        var searchOutput = await _videoRepository.Search(input.ToSearchInput(), cancellationToken);

        return ListVideosOutput.FromSearchOutput(searchOutput);
    }
}
