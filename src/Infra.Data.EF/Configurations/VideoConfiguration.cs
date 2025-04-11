using Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace Infra.Data.EF.Configurations;

public class VideoConfiguration : IEntityTypeConfiguration<Video>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Video> builder)
    {
        builder.HasKey(video => video.Id);
        builder.Navigation(video => video.Media).AutoInclude();
        builder.Navigation(video => video.Trailer).AutoInclude();
        builder.Property(video => video.Id).ValueGeneratedNever();
        builder.Property(video => video.Title).HasMaxLength(255);
        builder.Property(video => video.Description).HasMaxLength(4_000);
        builder.OwnsOne(video => video.Thumb, thumb =>
        {
            thumb.Property(image => image.Path).HasColumnName("ThumbPath");
        });
        builder.OwnsOne(video => video.ThumbHalf, thumbHalf =>
        {
            thumbHalf.Property(image => image.Path).HasColumnName("ThumbHalfPath");
        });
        builder.OwnsOne(video => video.Banner, banner =>
        {
            banner.Property(image => image.Path).HasColumnName("BannerPath");
        });
        builder.HasOne(video => video.Media).WithOne().HasForeignKey<Media>();
        builder.HasOne(video => video.Trailer).WithOne().HasForeignKey<Media>();
    }
}
