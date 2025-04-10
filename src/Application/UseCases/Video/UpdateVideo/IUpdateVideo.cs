using Application.UseCases.Video.Common;
using MediatR;

namespace Application.UseCases.Video.UpdateVideo;

public interface IUpdateVideo : IRequestHandler<UpdateVideoInput, VideoModelOutput>
{

}
