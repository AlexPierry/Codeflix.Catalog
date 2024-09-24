using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using UnitOfWorkInfra = Infra.Data.EF;
namespace IntegrationTest.Infra.Data.EF.UnitOfWork;

[Collection(nameof(UnitOfWorkTestFixture))]
public class UnitOfWorkTest
{
    private readonly UnitOfWorkTestFixture _fixture;

    public UnitOfWorkTest(UnitOfWorkTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(CommitOk))]
    [Trait("Integration/Infra.Data", "UnitOfWork - Persistence")]
    public async void CommitOk()
    {
        // Given
        var dbId = Guid.NewGuid().ToString();
        var dbContext = _fixture.CreateDbContext(false, dbId);
        var exampleCategoriesList = _fixture.GetExampleCategoriesList();
        await dbContext.AddRangeAsync(exampleCategoriesList);
        var unitOfWork = new UnitOfWorkInfra.UnitOfWork(dbContext);

        // When
        await unitOfWork.Commit(CancellationToken.None);

        // Then
        var assertDbContext = _fixture.CreateDbContext(true, dbId);
        var savedCategories = assertDbContext.Categories.AsNoTracking().ToList();

        savedCategories.Should().HaveCount(exampleCategoriesList.Count);
    }

    [Fact(DisplayName = nameof(RollbackOk))]
    [Trait("Integration/Infra.Data", "UnitOfWork - Persistence")]
    public async void RollbackOk()
    {
        // Given
        var dbId = Guid.NewGuid().ToString();
        var dbContext = _fixture.CreateDbContext(false, dbId);
        var unitOfWork = new UnitOfWorkInfra.UnitOfWork(dbContext);

        // When
        var task = async () => await unitOfWork.Rollback(CancellationToken.None);

        // Then
        await task.Should().NotThrowAsync();
    }
}