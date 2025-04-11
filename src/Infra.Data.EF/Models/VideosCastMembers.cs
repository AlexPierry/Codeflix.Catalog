using Domain.Entity;

namespace Infra.Data.EF.Models;

public class VideosCastMembers
{
    public Guid CastMemberId { get; set; }
    public Guid VideoId { get; set; }
    public CastMember? CastMember { get; set; }
    public Video? Video { get; set; }

    public VideosCastMembers(Guid castMemberId, Guid videoId)
    {
        CastMemberId = castMemberId;
        VideoId = videoId;
    }
}
