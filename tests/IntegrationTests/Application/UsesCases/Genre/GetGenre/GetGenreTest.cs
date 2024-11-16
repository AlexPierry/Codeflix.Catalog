using Application.Exceptions;
using Application.UseCases.Genre.GetGenre;
using FluentAssertions;
using Infra.Data.EF.Models;
using Infra.Data.EF.Repositories;

namespace IntegrationTest.Application.UseCases.Genre;

[Collection(nameof(GetGenreTestFixture))]
public class GetGenreTest
{
    private readonly GetGenreTestFixture _fixture;

    public GetGenreTest(GetGenreTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(GetGenreOk))]
    [Trait("Integration/Application", "GetGenre - Use Cases")]
    public async Task GetGenreOk()
    {
        // Given
        var dbContext = _fixture.CreateDbContext();
        var exampleGenreList = _fixture.GetExampleGenresList(10);
        var exampleGenre = exampleGenreList[5];
        await dbContext.AddRangeAsync(exampleGenreList);
        dbContext.SaveChanges();
        var genreRepository = new GenreRepository(_fixture.CreateDbContext(true));
        var input = new GetGenreInput(exampleGenre.Id);
        var useCase = new GetGenre(genreRepository);

        // When
        var output = await useCase.Handle(input, CancellationToken.None);

        // Then
        output.Should().NotBeNull();
        output.Id.Should().Be(exampleGenre.Id);
        output.Name.Should().Be(exampleGenre.Name);
        output.IsActive.Should().Be(exampleGenre.IsActive);
        output.CreatedAt.Should().Be(exampleGenre.CreatedAt);
    }

    [Fact(DisplayName = nameof(GetGenreThrowsWhenNotFound))]
    [Trait("Integration/Application", "GetGenre - Use Cases")]
    public async Task GetGenreThrowsWhenNotFound()
    {
        // Given
        var dbContext = _fixture.CreateDbContext();
        var exampleGenreList = _fixture.GetExampleGenresList(10);
        var exampleGenre = Guid.NewGuid();
        await dbContext.AddRangeAsync(exampleGenreList);
        dbContext.SaveChanges();
        var genreRepository = new GenreRepository(_fixture.CreateDbContext(true));
        var input = new GetGenreInput(exampleGenre);
        var useCase = new GetGenre(genreRepository);

        // When
        var action = async () => await useCase.Handle(input, CancellationToken.None);

        // Then
        await action.Should().ThrowAsync<NotFoundException>().WithMessage($"Genre '{exampleGenre}' not found.");
    }

    [Fact(DisplayName = nameof(GetGenreWithCategoryRelations))]
    [Trait("Integration/Application", "GetGenre - Use Cases")]
    public async Task GetGenreWithCategoryRelations()
    {
        // Given
        var dbContext = _fixture.CreateDbContext();
        var exampleGenreList = _fixture.GetExampleGenresList(10);
        var exampleCategoriesList = _fixture.GetExampleCategoriesList(5);
        var exampleGenre = exampleGenreList[5];
        exampleCategoriesList.ForEach(category =>
        {
            exampleGenre.AddCategory(category.Id);
        });
        await dbContext.Categories.AddRangeAsync(exampleCategoriesList);
        await dbContext.Genres.AddRangeAsync(exampleGenreList);
        await dbContext.GenresCategories.AddRangeAsync(
            exampleGenre.Categories.Select(categoryId => new GenresCategories(categoryId, exampleGenre.Id))
        );
        dbContext.SaveChanges();

        var genreRepository = new GenreRepository(_fixture.CreateDbContext(true));
        var input = new GetGenreInput(exampleGenre.Id);
        var useCase = new GetGenre(genreRepository);

        // When
        var output = await useCase.Handle(input, CancellationToken.None);

        // Then
        output.Should().NotBeNull();
        output.Id.Should().Be(exampleGenre.Id);
        output.Name.Should().Be(exampleGenre.Name);
        output.IsActive.Should().Be(exampleGenre.IsActive);
        output.CreatedAt.Should().Be(exampleGenre.CreatedAt);
        output.Catetories.Should().HaveCount(exampleGenre.Categories.Count);
        output.Catetories.Select(relation => relation.Id).Except(exampleGenre.Categories).Should().HaveCount(0);
    }

}