using Domain.Extensions;
using Entities = Domain.Entity;
namespace Application.UseCases.Video.Common;

public record VideoModelOutput
(
    Guid Id,
    string Title,
    string Description,
    bool Published,
    int Duration,
    string Rating,
    int YearLaunched,
    bool Opened,
    DateTime CreatedAt,
    IReadOnlyCollection<VideoModelOutputRelatedAggregate> Categories,
    IReadOnlyCollection<VideoModelOutputRelatedAggregate> Genres,
    IReadOnlyCollection<VideoModelOutputRelatedAggregate> CastMembers,
    string? ThumbFileUrl,
    string? BannerFileUrl,
    string? ThumbHalfFileUrl,
    string? VideoFileUrl,
    string? TrailerFileUrl
)
{
    public static VideoModelOutput FromVideo(
        Entities.Video video,
        IReadOnlyList<Entities.Category>? categories = null,
        IReadOnlyList<Entities.Genre>? genres = null,
        IReadOnlyList<Entities.CastMember>? castMembers = null)
    {
        return new VideoModelOutput(
            video.Id,
            video.Title,
            video.Description,
            video.Published,
            video.Duration,
            video.MovieRating.ToFriendlyString(),
            video.YearLaunched,
            video.Opened,
            video.CreatedAt,
            video.Categories.Select(id => new VideoModelOutputRelatedAggregate(id, categories?.FirstOrDefault(c => c.Id == id)?.Name)).ToList(),
            video.Genres.Select(id => new VideoModelOutputRelatedAggregate(id, genres?.FirstOrDefault(g => g.Id == id)?.Name)).ToList(),
            video.CastMembers.Select(id => new VideoModelOutputRelatedAggregate(id, castMembers?.FirstOrDefault(cm => cm.Id == id)?.Name)).ToList(),
            video.Thumb?.Path,
            video.Banner?.Path,
            video.ThumbHalf?.Path,
            video.Media?.FilePath,
            video.Trailer?.FilePath
        );
    }
}

public record VideoModelOutputRelatedAggregate(Guid Id, string? Name = null);