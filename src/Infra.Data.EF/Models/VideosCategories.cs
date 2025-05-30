using Domain.Entity;

namespace Infra.Data.EF.Models;

public class VideosCategories
{
    public Guid CategoryId { get; set; }
    public Guid VideoId { get; set; }
    public Category? Category { get; set; }
    public Video? Video { get; set; }

    public VideosCategories(Guid categoryId, Guid videoId)
    {
        CategoryId = categoryId;
        VideoId = videoId;
    }
}
