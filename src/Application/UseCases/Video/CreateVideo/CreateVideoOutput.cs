using Domain.Enum;
using Entities = Domain.Entity;

namespace Application.UseCases.Video.CreateVideo;

public record CreateVideoOutput(
    Guid Id,
    string Title,
    string Description,
    bool Published,
    int Duration,
    MovieRating Rating,
    int YearLaunched,
    bool Opened,
    DateTime CreatedAt,
    IReadOnlyCollection<Guid>? CategoriesIds,
    IReadOnlyCollection<Guid>? GenresIds,
    IReadOnlyCollection<Guid>? CastMembersIds,
    string? Thumb,
    string? Banner,
    string? ThumbHalf,
    string? Media,
    string? Trailer
)
{
    public static CreateVideoOutput FromVideo(Entities.Video video)
    {
        return new CreateVideoOutput(
            video.Id,
            video.Title,
            video.Description,
            video.Published,
            video.Duration,
            video.MovieRating,
            video.YearLaunched,
            video.Opened,
            video.CreatedAt,
            video.Categories,
            video.Genres,
            video.CastMembers,
            video.Thumb?.Path,
            video.Banner?.Path,
            video.ThumbHalf?.Path,
            video.Media?.FilePath,
            video.Trailer?.FilePath
        );
    }
}
