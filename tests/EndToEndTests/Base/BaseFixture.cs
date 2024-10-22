using System.Net.WebSockets;
using Bogus;
using Infra.Data.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EndToEndTests.Base;

public class BaseFixture
{
    protected Faker Faker { get; set; }

    private readonly string _dbConnectionString;

    public CustomWebApplicationFactory<Program> WebAppFactory { get; set; }
    public HttpClient HttpClient { get; set; }
    public ApiClient ApiClient { get; set; }

    public BaseFixture()
    {
        Faker = new Faker("pt_BR");
        WebAppFactory = new CustomWebApplicationFactory<Program>();
        HttpClient = WebAppFactory.CreateClient();
        ApiClient = new ApiClient(HttpClient);

        var configuration = WebAppFactory.Services.GetService(typeof(IConfiguration));
        ArgumentNullException.ThrowIfNull(configuration);
        _dbConnectionString = ((IConfiguration)configuration).GetConnectionString("CatalogDb")
            ?? throw new ArgumentNullException(nameof(_dbConnectionString));
    }

    public CodeflixCatalogDbContext CreateDbContext()
    {
        var dbContext = new CodeflixCatalogDbContext(
            new DbContextOptionsBuilder<CodeflixCatalogDbContext>()
            .UseMySql(_dbConnectionString, ServerVersion.AutoDetect(_dbConnectionString))
            .Options
        );

        return dbContext;
    }

    public void CleanPercistence()
    {
        var dbContext = CreateDbContext();
        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();
    }
}