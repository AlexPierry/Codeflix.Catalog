using Domain.Enum;
using MediatR;

namespace Application.UseCases.Video.CreateVideo;

public record CreateVideoInput(
    string Title,
    string Description,
    int YearLaunched,
    bool Opened,
    bool Published,
    int Duration,
    MovieRating MovieRating
) : IRequest<CreateVideoOutput>;

