using FluentAssertions;
using Infra.Data.EF;
using Infra.Data.EF.Repositories;
using Microsoft.EntityFrameworkCore;

namespace IntegrationTest.Infra.Data.EF.Repositories;

[Collection(nameof(GenreRepositoryTestFixture))]
public class GenreRepositoryTest
{
    private readonly GenreRepositoryTestFixture _fixture;

    public GenreRepositoryTest(GenreRepositoryTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(InsertOk))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    public async Task InsertOk()
    {
        // Given
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleGenre = _fixture.GetExampleGenre();
        var categoriesExampleList = _fixture.GetExampleCategoriesList(3);
        categoriesExampleList.ForEach(category => exampleGenre.AddCategory(category.Id));
        await dbContext.Categories.AddRangeAsync(categoriesExampleList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var genreRepository = new GenreRepository(dbContext);

        // When
        await genreRepository.Insert(exampleGenre, CancellationToken.None);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        // Then
        var assertsDbContext = _fixture.CreateDbContext(true);
        var dbGenre = await assertsDbContext.Genres.FindAsync(exampleGenre.Id);
        dbGenre.Should().NotBeNull();
        dbGenre!.Name.Should().Be(exampleGenre.Name);
        dbGenre.IsActive.Should().Be(exampleGenre.IsActive);
        dbGenre.CreatedAt.Should().Be(exampleGenre.CreatedAt);
        var genresCategoriesRelation = await assertsDbContext
            .GenresCategories.Where(r => r.GenreId == exampleGenre.Id).ToListAsync();
        genresCategoriesRelation.Should().HaveCount(categoriesExampleList.Count);
        genresCategoriesRelation.Select(x => x.CategoryId).ToList()
            .Except(exampleGenre.Categories).Should().HaveCount(0);
    }
}