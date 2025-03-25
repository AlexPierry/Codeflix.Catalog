using Application.UseCases.Video.Common;
using MediatR;

namespace Application.UseCases.Video.GetVideo;

public record GetVideoInput(Guid Id) : IRequest<VideoModelOutput>
{

}
