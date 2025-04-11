using Domain.Entity;
using Infra.Data.EF.Configurations;
using Infra.Data.EF.Models;
using Microsoft.EntityFrameworkCore;

namespace Infra.Data.EF;

public class CodeflixCatalogDbContext : DbContext
{
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Genre> Genres => Set<Genre>();
    public DbSet<GenresCategories> GenresCategories => Set<GenresCategories>();
    public DbSet<CastMember> CastMembers => Set<CastMember>();
    public DbSet<Video> Videos => Set<Video>();
    public DbSet<VideosCategories> VideosCategories => Set<VideosCategories>();
    public DbSet<VideosGenres> VideosGenres => Set<VideosGenres>();
    public DbSet<VideosCastMembers> VideosCastMembers => Set<VideosCastMembers>();

    public CodeflixCatalogDbContext(DbContextOptions<CodeflixCatalogDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CategoryConfiguration());
        modelBuilder.ApplyConfiguration(new GenreConfiguration());
        modelBuilder.ApplyConfiguration(new GenresCategoriesConfiguration());
        modelBuilder.ApplyConfiguration(new CastMemberConfiguration());
        modelBuilder.ApplyConfiguration(new VideoConfiguration());
        modelBuilder.ApplyConfiguration(new VideosCategoriesConfiguration());
        modelBuilder.ApplyConfiguration(new VideosGenresConfiguration());
        modelBuilder.ApplyConfiguration(new VideosCastMembersConfiguration());
    }
}