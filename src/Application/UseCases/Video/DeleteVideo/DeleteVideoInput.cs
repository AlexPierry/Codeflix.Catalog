using MediatR;

namespace Application.UseCases.Video.DeleteVideo;

public record DeleteVideoInput(Guid videoId) : IRequest
{
}
