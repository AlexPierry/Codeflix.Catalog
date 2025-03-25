using Application.UseCases.Video.Common;
using MediatR;

namespace Application.UseCases.Video.GetVideo;

public interface IGetVideo : IRequestHandler<GetVideoInput, VideoModelOutput>
{
}
