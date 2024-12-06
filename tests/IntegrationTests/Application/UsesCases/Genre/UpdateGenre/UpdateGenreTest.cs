using Application.Exceptions;
using Application.UseCases.Genre.UpdateGenre;
using FluentAssertions;
using Infra.Data.EF;
using Infra.Data.EF.Repositories;
using Microsoft.EntityFrameworkCore;

namespace IntegrationTest.Application.UseCases.Genre;

[Collection(nameof(UpdateGenreTestFixture))]
public class UpdateGenreTest
{
    private readonly UpdateGenreTestFixture _fixture;

    public UpdateGenreTest(UpdateGenreTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(UpdateGenreOk))]
    [Trait("Integration/Application", "UpdateGenre - Use Cases")]
    public async Task UpdateGenreOk()
    {
        //Given
        var exampleGenresList = _fixture.GetExampleGenresList(5);
        var exampleGenre = exampleGenresList[3];
        var dbContext = _fixture.CreateDbContext();
        await dbContext.Genres.AddRangeAsync(exampleGenresList);
        await dbContext.SaveChangesAsync();
        var genreRepository = new GenreRepository(dbContext);
        var categoryRepository = new CategoryRepository(dbContext);
        var unitOfWork = new UnitOfWork(dbContext);
        var useCase = new UpdateGenre(genreRepository, unitOfWork, categoryRepository);
        exampleGenre.Update(_fixture.GetValidGenreName());
        if (exampleGenre.IsActive)
        {
            exampleGenre.Deactivate();
        }
        else
        {
            exampleGenre.Activate();
        }
        var input = _fixture.GetInput(exampleGenre);

        //When
        var output = await useCase.Handle(input, CancellationToken.None);

        //Then
        output.Should().NotBeNull();
        output.Id.Should().Be(exampleGenre.Id);
        output.Name.Should().Be(exampleGenre.Name);
        output.IsActive.Should().Be(exampleGenre.IsActive);
        output.CreatedAt.Should().BeSameDateAs(exampleGenre.CreatedAt);

        var dbGenre = await _fixture.CreateDbContext(true).Genres.FindAsync(exampleGenre.Id);
        dbGenre.Should().NotBeNull();
        dbGenre!.Name.Should().Be(exampleGenre.Name);
        dbGenre.IsActive.Should().Be(exampleGenre.IsActive);
        dbGenre.CreatedAt.Should().BeSameDateAs(exampleGenre.CreatedAt);
    }

    [Fact(DisplayName = nameof(UpdateGenreWithRelations))]
    [Trait("Integration/Application", "UpdateGenre - Use Cases")]
    public async Task UpdateGenreWithRelations()
    {
        //Given
        var exampleGenresList = _fixture.GetExampleGenresList(5);
        var exampleCategoriesList = _fixture.GetExampleCategoriesList(10);
        var exampleGenre = exampleGenresList[3];
        exampleCategoriesList.GetRange(0, 5).ForEach(category => exampleGenre.AddCategory(category.Id));
        var dbContext = _fixture.CreateDbContext();
        await dbContext.Genres.AddRangeAsync(exampleGenresList);
        await dbContext.Categories.AddRangeAsync(exampleCategoriesList);
        await dbContext.SaveChangesAsync();
        var genreRepository = new GenreRepository(dbContext);
        var categoryRepository = new CategoryRepository(dbContext);
        var unitOfWork = new UnitOfWork(dbContext);
        var useCase = new UpdateGenre(genreRepository, unitOfWork, categoryRepository);
        exampleGenre.Update(_fixture.GetValidGenreName());
        if (exampleGenre.IsActive)
        {
            exampleGenre.Deactivate();
        }
        else
        {
            exampleGenre.Activate();
        }
        exampleGenre.RemoveAllCategories();
        exampleCategoriesList.GetRange(5, 3).ForEach(category => exampleGenre.AddCategory(category.Id));
        var input = _fixture.GetInput(exampleGenre);

        //When
        var output = await useCase.Handle(input, CancellationToken.None);

        //Then
        output.Should().NotBeNull();
        output.Id.Should().Be(exampleGenre.Id);
        output.Name.Should().Be(exampleGenre.Name);
        output.IsActive.Should().Be(exampleGenre.IsActive);
        output.CreatedAt.Should().BeSameDateAs(exampleGenre.CreatedAt);
        output.Catetories.Should().HaveCount(exampleGenre.Categories.Count);
        output.Catetories.Select(c => c.Id).Should().BeEquivalentTo(exampleGenre.Categories);

        var assertDbContext = _fixture.CreateDbContext(true);
        var dbGenre = await assertDbContext.Genres.FindAsync(exampleGenre.Id);
        dbGenre.Should().NotBeNull();
        dbGenre!.Name.Should().Be(exampleGenre.Name);
        dbGenre.IsActive.Should().Be(exampleGenre.IsActive);
        dbGenre.CreatedAt.Should().BeSameDateAs(exampleGenre.CreatedAt);
        var relations = await assertDbContext.GenresCategories
            .AsNoTracking().Where(x => x.GenreId == exampleGenre.Id).ToListAsync();
        relations.Should().HaveCount(exampleGenre.Categories.Count);
        relations.Select(c => c.CategoryId).Except(exampleGenre.Categories).Should().HaveCount(0);
    }

    [Fact(DisplayName = nameof(ThrowsExceptionWhenCategoryDoesNotExist))]
    [Trait("Integration/Application", "UpdateGenre - Use Cases")]
    public async Task ThrowsExceptionWhenCategoryDoesNotExist()
    {
        //Given
        var exampleGenresList = _fixture.GetExampleGenresList(5);
        var exampleCategoriesList = _fixture.GetExampleCategoriesList(10);
        var exampleGenre = exampleGenresList[3];
        exampleCategoriesList.GetRange(0, 5).ForEach(category => exampleGenre.AddCategory(category.Id));
        var dbContext = _fixture.CreateDbContext();
        await dbContext.Genres.AddRangeAsync(exampleGenresList);
        await dbContext.Categories.AddRangeAsync(exampleCategoriesList);
        await dbContext.SaveChangesAsync();
        var genreRepository = new GenreRepository(dbContext);
        var categoryRepository = new CategoryRepository(dbContext);
        var unitOfWork = new UnitOfWork(dbContext);
        var useCase = new UpdateGenre(genreRepository, unitOfWork, categoryRepository);
        exampleGenre.Update(_fixture.GetValidGenreName());
        if (exampleGenre.IsActive)
        {
            exampleGenre.Deactivate();
        }
        else
        {
            exampleGenre.Activate();
        }
        exampleGenre.RemoveAllCategories();
        exampleCategoriesList.GetRange(5, 3).ForEach(category => exampleGenre.AddCategory(category.Id));
        var randomCategoryId = Guid.NewGuid();
        exampleGenre.AddCategory(randomCategoryId);
        var input = _fixture.GetInput(exampleGenre);

        //When
        var action = async () => await useCase.Handle(input, CancellationToken.None);

        //Then
        await action.Should().ThrowAsync<RelatedAggregateException>().WithMessage($"Related category id(s) not found: {randomCategoryId}");
    }

    [Fact(DisplayName = nameof(ThrowsExceptionWhenGenreIdDoesNotExist))]
    [Trait("Integration/Application", "UpdateGenre - Use Cases")]
    public async Task ThrowsExceptionWhenGenreIdDoesNotExist()
    {
        //Given
        var exampleGenresList = _fixture.GetExampleGenresList(5);
        var exampleCategoriesList = _fixture.GetExampleCategoriesList(10);
        var exampleGenre = exampleGenresList[3];
        exampleCategoriesList.GetRange(0, 5).ForEach(category => exampleGenre.AddCategory(category.Id));
        var dbContext = _fixture.CreateDbContext();
        await dbContext.Genres.AddRangeAsync(exampleGenresList);
        await dbContext.Categories.AddRangeAsync(exampleCategoriesList);
        await dbContext.SaveChangesAsync();
        var genreRepository = new GenreRepository(dbContext);
        var categoryRepository = new CategoryRepository(dbContext);
        var unitOfWork = new UnitOfWork(dbContext);
        var useCase = new UpdateGenre(genreRepository, unitOfWork, categoryRepository);
        var input = _fixture.GetInput(exampleGenre);
        var randomGenreId = Guid.NewGuid();
        input.Id = randomGenreId;

        //When
        var action = async () => await useCase.Handle(input, CancellationToken.None);

        //Then
        await action.Should().ThrowAsync<NotFoundException>().WithMessage($"Genre '{randomGenreId}' not found.");
    }

    [Fact(DisplayName = nameof(UpdateGenreWithoutNewRelations))]
    [Trait("Integration/Application", "UpdateGenre - Use Cases")]
    public async Task UpdateGenreWithoutNewRelations()
    {
        //Given
        var exampleGenresList = _fixture.GetExampleGenresList(5);
        var exampleCategoriesList = _fixture.GetExampleCategoriesList(10);
        var exampleGenre = exampleGenresList[3];
        exampleCategoriesList.GetRange(0, 5).ForEach(category => exampleGenre.AddCategory(category.Id));
        var dbContext = _fixture.CreateDbContext();
        await dbContext.Genres.AddRangeAsync(exampleGenresList);
        await dbContext.Categories.AddRangeAsync(exampleCategoriesList);
        await dbContext.SaveChangesAsync();
        var genreRepository = new GenreRepository(dbContext);
        var categoryRepository = new CategoryRepository(dbContext);
        var unitOfWork = new UnitOfWork(dbContext);
        var useCase = new UpdateGenre(genreRepository, unitOfWork, categoryRepository);
        exampleGenre.Update(_fixture.GetValidGenreName());
        if (exampleGenre.IsActive)
        {
            exampleGenre.Deactivate();
        }
        else
        {
            exampleGenre.Activate();
        }
        var input = _fixture.GetInputWithoutCategories(exampleGenre);

        //When
        var output = await useCase.Handle(input, CancellationToken.None);

        //Then
        output.Should().NotBeNull();
        output.Id.Should().Be(exampleGenre.Id);
        output.Name.Should().Be(exampleGenre.Name);
        output.IsActive.Should().Be(exampleGenre.IsActive);
        output.CreatedAt.Should().BeSameDateAs(exampleGenre.CreatedAt);
        output.Catetories.Should().HaveCount(exampleGenre.Categories.Count);
        output.Catetories.Select(c => c.Id).Should().BeEquivalentTo(exampleGenre.Categories);

        var assertDbContext = _fixture.CreateDbContext(true);
        var dbGenre = await assertDbContext.Genres.FindAsync(exampleGenre.Id);
        dbGenre.Should().NotBeNull();
        dbGenre!.Name.Should().Be(exampleGenre.Name);
        dbGenre.IsActive.Should().Be(exampleGenre.IsActive);
        dbGenre.CreatedAt.Should().BeSameDateAs(exampleGenre.CreatedAt);
        var relations = await assertDbContext.GenresCategories
            .AsNoTracking().Where(x => x.GenreId == exampleGenre.Id).ToListAsync();
        relations.Should().HaveCount(exampleGenre.Categories.Count);
        relations.Select(c => c.CategoryId).Except(exampleGenre.Categories).Should().HaveCount(0);
    }

    [Fact(DisplayName = nameof(UpdateGenreRemovingRelations))]
    [Trait("Integration/Application", "UpdateGenre - Use Cases")]
    public async Task UpdateGenreRemovingRelations()
    {
        //Given
        var exampleGenresList = _fixture.GetExampleGenresList(5);
        var exampleCategoriesList = _fixture.GetExampleCategoriesList(10);
        var exampleGenre = exampleGenresList[3];
        exampleCategoriesList.GetRange(0, 5).ForEach(category => exampleGenre.AddCategory(category.Id));
        var dbContext = _fixture.CreateDbContext();
        await dbContext.Genres.AddRangeAsync(exampleGenresList);
        await dbContext.Categories.AddRangeAsync(exampleCategoriesList);
        await dbContext.SaveChangesAsync();
        var genreRepository = new GenreRepository(dbContext);
        var categoryRepository = new CategoryRepository(dbContext);
        var unitOfWork = new UnitOfWork(dbContext);
        var useCase = new UpdateGenre(genreRepository, unitOfWork, categoryRepository);
        exampleGenre.Update(_fixture.GetValidGenreName());
        if (exampleGenre.IsActive)
        {
            exampleGenre.Deactivate();
        }
        else
        {
            exampleGenre.Activate();
        }
        exampleGenre.RemoveAllCategories();

        var input = _fixture.GetInput(exampleGenre);

        //When
        var output = await useCase.Handle(input, CancellationToken.None);

        //Then
        output.Should().NotBeNull();
        output.Id.Should().Be(exampleGenre.Id);
        output.Name.Should().Be(exampleGenre.Name);
        output.IsActive.Should().Be(exampleGenre.IsActive);
        output.CreatedAt.Should().BeSameDateAs(exampleGenre.CreatedAt);
        output.Catetories.Should().HaveCount(0);

        var assertDbContext = _fixture.CreateDbContext(true);
        var dbGenre = await assertDbContext.Genres.FindAsync(exampleGenre.Id);
        dbGenre.Should().NotBeNull();
        dbGenre!.Name.Should().Be(exampleGenre.Name);
        dbGenre.IsActive.Should().Be(exampleGenre.IsActive);
        dbGenre.CreatedAt.Should().BeSameDateAs(exampleGenre.CreatedAt);
        var relations = await assertDbContext.GenresCategories
            .AsNoTracking().Where(x => x.GenreId == exampleGenre.Id).ToListAsync();
        relations.Should().HaveCount(0);
    }
}