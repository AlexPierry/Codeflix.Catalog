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
    DateTime CreatedAt)
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
            video.CreatedAt
        );
    }
}
