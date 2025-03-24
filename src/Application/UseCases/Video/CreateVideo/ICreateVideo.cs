using MediatR;

namespace Application.UseCases.Video.CreateVideo;

public interface ICreateVideo : IRequestHandler<CreateVideoInput, CreateVideoOutput>
{
}
