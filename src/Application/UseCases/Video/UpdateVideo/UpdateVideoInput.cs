using Application.UseCases.Video.Common;
using Domain.Enum;
using MediatR;

namespace Application.UseCases.Video.UpdateVideo;

public record UpdateVideoInput(
    Guid Id,
    string Title,
    string Description,
    int YearLaunched,
    bool Opened,
    bool Published,
    int Duration,
    MovieRating? MovieRating,
    List<Guid>? GenreIds = null,
    List<Guid>? CategoryIds = null,
    List<Guid>? CastMemberIds = null,
    FileInput? Banner = null,
    FileInput? Thumb = null,
    FileInput? ThumbHalf = null
) : IRequest<VideoModelOutput>;

