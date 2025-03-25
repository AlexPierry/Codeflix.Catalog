using Application.UseCases.Video.Common;
using MediatR;

namespace Application.UseCases.Video.CreateVideo;

public interface ICreateVideo : IRequestHandler<CreateVideoInput, VideoModelOutput>
{
}
