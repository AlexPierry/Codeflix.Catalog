using Application.Exceptions;
using Application.UseCases.Genre.CreateGenre;
using FluentAssertions;
using Infra.Data.EF;
using Infra.Data.EF.Repositories;
using Microsoft.EntityFrameworkCore;

namespace IntegrationTest.Application.UseCases.Genre;

[Collection(nameof(CreateGenreTestFixture))]
public class CreateGenreTest
{
    private readonly CreateGenreTestFixture _fixture;

    public CreateGenreTest(CreateGenreTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(CreateGenre))]
    [Trait("Integration/Application", "CreateGenre - Use Cases")]
    public async void CreateGenre()
    {
        // Given
        var dbContext = _fixture.CreateDbContext();
        var genreRepository = new GenreRepository(dbContext);
        var categoryRepository = new CategoryRepository(dbContext);
        var unitOfWork = new UnitOfWork(dbContext);

        var useCase = new CreateGenre(genreRepository, unitOfWork, categoryRepository);
        var input = _fixture.GetInput();

        // When
        var output = await useCase.Handle(input, CancellationToken.None);

        // Then
        output.Should().NotBeNull();
        output.Id.Should().NotBe(Guid.Empty);
        output.Name.Should().Be(input.Name);
        output.IsActive.Should().Be(input.IsActive);
        output.CreatedAt.Should().NotBe(default);

        var dbGenre = await _fixture.CreateDbContext(true).Genres.FindAsync(output.Id);
        dbGenre.Should().NotBeNull();
        dbGenre!.Name.Should().Be(input.Name);
        dbGenre.IsActive.Should().Be(input.IsActive);
        dbGenre.CreatedAt.Should().Be(output.CreatedAt);
    }

    [Fact(DisplayName = nameof(CreateGenreWithNameOnly))]
    [Trait("Integration/Application", "CreateGenre - Use Cases")]
    public async void CreateGenreWithNameOnly()
    {
        // Given
        var dbContext = _fixture.CreateDbContext();
        var genreRepository = new GenreRepository(dbContext);
        var categoryRepository = new CategoryRepository(dbContext);

        var unitOfWork = new UnitOfWork(dbContext);

        var useCase = new CreateGenre(genreRepository, unitOfWork, categoryRepository);
        var input = new CreateGenreInput(_fixture.GetInput().Name);

        // When
        var output = await useCase.Handle(input, CancellationToken.None);

        // Then
        output.Should().NotBeNull();
        output.Id.Should().NotBe(Guid.Empty);
        output.Name.Should().Be(input.Name);
        output.IsActive.Should().Be(true);
        output.CreatedAt.Should().NotBe(default);

        var dbGenre = await _fixture.CreateDbContext(true).Genres.FindAsync(output.Id);
        dbGenre.Should().NotBeNull();
        dbGenre!.Name.Should().Be(input.Name);
        dbGenre.IsActive.Should().Be(true);
        dbGenre.CreatedAt.Should().Be(output.CreatedAt);
    }

    [Fact(DisplayName = nameof(CreateGenreWithCategoriesRelation))]
    [Trait("Integration/Application", "CreateGenre - Use Cases")]
    public async void CreateGenreWithCategoriesRelation()
    {
        // Given
        var dbContext = _fixture.CreateDbContext();
        var exampleCategoriesList = _fixture.GetExampleCategoriesList(5);
        await dbContext.Categories.AddRangeAsync(exampleCategoriesList);
        await dbContext.SaveChangesAsync();
        var genreRepository = new GenreRepository(dbContext);
        var categoryRepository = new CategoryRepository(dbContext);
        var unitOfWork = new UnitOfWork(dbContext);

        var useCase = new CreateGenre(genreRepository, unitOfWork, categoryRepository);
        var input = _fixture.GetInput(exampleCategoriesList.Select(c => c.Id).ToList());

        // When
        var output = await useCase.Handle(input, CancellationToken.None);

        // Then
        output.Should().NotBeNull();
        output.Id.Should().NotBe(Guid.Empty);
        output.Name.Should().Be(input.Name);
        output.IsActive.Should().Be(input.IsActive);
        output.CreatedAt.Should().NotBe(default);
        output.Catetories.Should().HaveCount(exampleCategoriesList.Count());
        output.Catetories.Select(c => c.Id).Should()
            .BeEquivalentTo(exampleCategoriesList.Select(c => c.Id));

        var assertDbContext = _fixture.CreateDbContext(true);
        var dbGenre = await assertDbContext.Genres.FindAsync(output.Id);
        dbGenre.Should().NotBeNull();
        dbGenre!.Name.Should().Be(input.Name);
        dbGenre.IsActive.Should().Be(input.IsActive);
        dbGenre.CreatedAt.Should().Be(output.CreatedAt);
        var relations = await assertDbContext.GenresCategories
            .AsNoTracking().Where(x => x.GenreId == output.Id).ToListAsync();
        relations.Should().HaveCount(input.Categories!.Count);
        relations.Select(c => c.CategoryId).Except(input.Categories).Should().HaveCount(0);
    }

    [Fact(DisplayName = nameof(ThrowsWhenRelatedCategoryNotFound))]
    [Trait("Integration/Application", "CreateGenre - Use Cases")]
    public async void ThrowsWhenRelatedCategoryNotFound()
    {
        // Given
        var dbContext = _fixture.CreateDbContext();
        var exampleCategoriesList = _fixture.GetExampleCategoriesList(5);
        await dbContext.Categories.AddRangeAsync(exampleCategoriesList);
        await dbContext.SaveChangesAsync();
        var genreRepository = new GenreRepository(dbContext);
        var categoryRepository = new CategoryRepository(dbContext);
        var unitOfWork = new UnitOfWork(dbContext);

        var useCase = new CreateGenre(genreRepository, unitOfWork, categoryRepository);
        var input = _fixture.GetInput(exampleCategoriesList.Select(c => c.Id).ToList());
        var randomGuid = Guid.NewGuid();
        input.Categories!.Add(randomGuid);

        // When
        var action = async () => await useCase.Handle(input, CancellationToken.None);

        // Then
        await action.Should().ThrowAsync<RelatedAggregateException>().WithMessage($"Related category id(s) not found: {randomGuid}");
    }
}