using Infra.Data.EF.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.Data.EF.Configurations;

public class VideosGenresConfiguration : IEntityTypeConfiguration<VideosGenres>
{
    public void Configure(EntityTypeBuilder<VideosGenres> builder)
    {
        builder.HasKey(relation => new { relation.GenreId, relation.VideoId });
    }

}
