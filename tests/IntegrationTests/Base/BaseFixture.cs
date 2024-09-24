using Bogus;
using Infra.Data.EF;
using Microsoft.EntityFrameworkCore;

namespace IntegrationTest.Base;

public class BaseFixture
{
    protected Faker Faker { get; set; }

    public BaseFixture()
    {
        Faker = new Faker("pt_BR");
    }

    public CodeflixCatalogDbContext CreateDbContext(bool preserveData = false, string dbId = "")
    {
        var dbContext = new CodeflixCatalogDbContext(
            new DbContextOptionsBuilder<CodeflixCatalogDbContext>()
            .UseInMemoryDatabase($"integration-tests-db{dbId}")
            .Options
        );

        if (preserveData == false)
        {
            dbContext.Database.EnsureDeleted();
        }

        return dbContext;
    }
}