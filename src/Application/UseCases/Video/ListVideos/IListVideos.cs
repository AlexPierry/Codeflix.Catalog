using MediatR;

namespace Application.UseCases.Video.ListVideos;

public interface IListVideos : IRequestHandler<ListVideosInput, ListVideosOutput>
{

}
