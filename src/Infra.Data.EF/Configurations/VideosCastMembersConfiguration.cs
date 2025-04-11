using Infra.Data.EF.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.Data.EF.Configurations;

public class VideosCastMembersConfiguration : IEntityTypeConfiguration<VideosCastMembers>
{
    public void Configure(EntityTypeBuilder<VideosCastMembers> builder)
    {
        builder.HasKey(relation => new { relation.CastMemberId, relation.VideoId });
    }
}
