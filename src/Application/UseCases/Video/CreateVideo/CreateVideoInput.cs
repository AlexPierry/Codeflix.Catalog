using Application.UseCases.Video.Common;
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
    MovieRating MovieRating,
    IReadOnlyCollection<Guid>? CategoriesIds = null,
    IReadOnlyCollection<Guid>? GenresIds = null,
    IReadOnlyCollection<Guid>? CastMembersIds = null,
    FileInput? Thumb = null,
    FileInput? Banner = null,
    FileInput? ThumbHalf = null,
    FileInput? Media = null,
    FileInput? Trailer = null
) : IRequest<VideoModelOutput>;

