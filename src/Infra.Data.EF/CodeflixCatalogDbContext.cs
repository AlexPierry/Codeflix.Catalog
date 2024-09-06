using Domain.Entity;
using Infra.Data.EF.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Infra.Data.EF;

public class CodeflixCatalogDbContext : DbContext
{
    public DbSet<Category> Categories => Set<Category>();

    public CodeflixCatalogDbContext(DbContextOptions<CodeflixCatalogDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CategoryConfiguration());
    }
}