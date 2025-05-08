using Application;
using Application.Exceptions;
using Application.UseCases.Genre.DeleteGenre;
using FluentAssertions;
using Infra.Data.EF;
using Infra.Data.EF.Models;
using Infra.Data.EF.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace IntegrationTest.Application.UseCases.Genre;

[Collection(nameof(DeleteGenreTestFixture))]
public class DeleteGenreTest
{
    private readonly DeleteGenreTestFixture _fixture;

    public DeleteGenreTest(DeleteGenreTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(DeleteGenreOk))]
    [Trait("Integration/Application", "DeleteGenre - Use Cases")]
    public async Task DeleteGenreOk()
    {
        // Given
        var dbContext = _fixture.CreateDbContext();
        var exampleGenreList = _fixture.GetExampleGenresList(10);
        var exampleGenre = exampleGenreList[5];
        await dbContext.AddRangeAsync(exampleGenreList);
        await dbContext.SaveChangesAsync();
        var genreRepository = new GenreRepository(dbContext);
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging();
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var eventPublisher = new DomainEventPublisher(serviceProvider);
        var unitOfWork = new UnitOfWork(
            dbContext,
            eventPublisher,
            serviceProvider.GetRequiredService<ILogger<UnitOfWork>>()
        );
        var useCase = new DeleteGenre(genreRepository, unitOfWork);
        var input = _fixture.GetInput(exampleGenre.Id);

        // When
        await useCase.Handle(input, CancellationToken.None);

        // Then
        var assertDbContext = _fixture.CreateDbContext(true);
        var dbGenre = await assertDbContext.Genres.FindAsync(exampleGenre.Id);
        dbGenre.Should().BeNull();
        var allGenres = await assertDbContext.Genres.ToListAsync();
        allGenres.Should().HaveCount(exampleGenreList.Count - 1);
    }

    [Fact(DisplayName = nameof(ThrowsExceptionWhenNotFound))]
    [Trait("Integration/Application", "DeleteGenre - Use Cases")]
    public async Task ThrowsExceptionWhenNotFound()
    {
        // Given
        var dbContext = _fixture.CreateDbContext();
        var exampleGenreList = _fixture.GetExampleGenresList(10);
        await dbContext.AddRangeAsync(exampleGenreList);
        await dbContext.SaveChangesAsync();
        var genreRepository = new GenreRepository(dbContext);
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging();
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var eventPublisher = new DomainEventPublisher(serviceProvider);
        var unitOfWork = new UnitOfWork(
            dbContext,
            eventPublisher,
            serviceProvider.GetRequiredService<ILogger<UnitOfWork>>()
        );
        var useCase = new DeleteGenre(genreRepository, unitOfWork);
        var randomGenreId = Guid.NewGuid();
        var input = _fixture.GetInput(randomGenreId);

        // When
        var action = async () => await useCase.Handle(input, CancellationToken.None);

        // Then
        await action.Should().ThrowAsync<NotFoundException>().WithMessage($"Genre '{randomGenreId}' not found.");
    }

    [Fact(DisplayName = nameof(DeleteGenreWithRelations))]
    [Trait("Integration/Application", "DeleteGenre - Use Cases")]
    public async Task DeleteGenreWithRelations()
    {
        // Given
        var dbContext = _fixture.CreateDbContext();
        var exampleGenreList = _fixture.GetExampleGenresList(5);
        var exampleCategoriesList = _fixture.GetExampleCategoriesList(5);
        var exampleGenre = exampleGenreList[3];
        exampleCategoriesList.ForEach(category =>
        {
            exampleGenre.AddCategory(category.Id);
        });
        await dbContext.Categories.AddRangeAsync(exampleCategoriesList);
        await dbContext.Genres.AddRangeAsync(exampleGenreList);
        await dbContext.GenresCategories.AddRangeAsync(
            exampleGenre.Categories.Select(categoryId => new GenresCategories(categoryId, exampleGenre.Id))
        );
        await dbContext.SaveChangesAsync();
        var genreRepository = new GenreRepository(dbContext);
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging();
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var eventPublisher = new DomainEventPublisher(serviceProvider);
        var unitOfWork = new UnitOfWork(
            dbContext,
            eventPublisher,
            serviceProvider.GetRequiredService<ILogger<UnitOfWork>>()
        );
        var useCase = new DeleteGenre(genreRepository, unitOfWork);
        var input = _fixture.GetInput(exampleGenre.Id);

        // When
        await useCase.Handle(input, CancellationToken.None);

        // Then
        var assertDbContext = _fixture.CreateDbContext(true);
        var dbGenre = await assertDbContext.Genres.FindAsync(exampleGenre.Id);
        dbGenre.Should().BeNull();
        var allGenres = await assertDbContext.Genres.ToListAsync();
        allGenres.Should().HaveCount(exampleGenreList.Count - 1);
        var relations = await assertDbContext.GenresCategories.ToListAsync();
        relations.Should().HaveCount(0);
        var categories = await assertDbContext.Categories.ToListAsync();
        categories.Should().HaveCount(exampleCategoriesList.Count);
    }
}