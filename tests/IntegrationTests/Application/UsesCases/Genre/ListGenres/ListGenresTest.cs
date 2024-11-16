using Application.UseCases.Genre.Common;
using Application.UseCases.Genre.ListGenres;
using Domain.SeedWork.SearchableRepository;
using FluentAssertions;
using Infra.Data.EF.Models;
using Infra.Data.EF.Repositories;

namespace IntegrationTest.Application.UseCases.Genre;

[Collection(nameof(ListGenresTestFixture))]
public class ListGenresTest
{
    private readonly ListGenresTestFixture _fixture;

    public ListGenresTest(ListGenresTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(SearchReturnsListAndTotal))]
    [Trait("Integration/Application", "ListGenres - Use Cases")]
    public async void SearchReturnsListAndTotal()
    {
        // Given
        var dbContext = _fixture.CreateDbContext();
        var repository = new GenreRepository(dbContext);
        var genresExampleList = _fixture.GetExampleGenresList(10);
        await dbContext.Genres.AddRangeAsync(genresExampleList);
        await dbContext.SaveChangesAsync();
        var input = new ListGenresInput(1, 20);
        var useCase = new ListGenres(repository, new CategoryRepository(dbContext));

        // When
        var output = await useCase.Handle(input, CancellationToken.None);

        // Then
        output.Should().NotBeNull();
        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Total.Should().Be(genresExampleList.Count);
        output.Items.Should().HaveCount(genresExampleList.Count);
        output.Items.ToList().ForEach(outputItem =>
        {
            var targetGenre = genresExampleList.First(x => x.Id == outputItem.Id);
            outputItem.Should().NotBeNull();
            outputItem.Name.Should().Be(targetGenre.Name);
            outputItem.IsActive.Should().Be(targetGenre.IsActive);
            outputItem.CreatedAt.Should().Be(targetGenre.CreatedAt);
        });
    }

    [Fact(DisplayName = nameof(SearchReturnsListAndTotal))]
    [Trait("Integration/Application", "ListGenres - Use Cases")]
    public async void SearchReturnsEmpty()
    {
        // Given
        var dbContext = _fixture.CreateDbContext();
        var repository = new GenreRepository(dbContext);
        var input = new ListGenresInput(1, 20);
        var useCase = new ListGenres(repository, new CategoryRepository(dbContext));

        // When
        var output = await useCase.Handle(input, CancellationToken.None);

        // Then
        output.Should().NotBeNull();
        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Total.Should().Be(0);
        output.Items.Should().HaveCount(0);
    }

    [Fact(DisplayName = nameof(SearchReturnsListAndTotalWithRelations))]
    [Trait("Integration/Application", "ListGenres - Use Cases")]
    public async void SearchReturnsListAndTotalWithRelations()
    {
        // Given
        var dbContext = _fixture.CreateDbContext();
        var repository = new GenreRepository(dbContext);
        var genresExampleList = _fixture.GetExampleGenresList(10);
        var categoriesExampleList = _fixture.GetExampleCategoriesList(10);
        await dbContext.Genres.AddRangeAsync(genresExampleList);
        await dbContext.Categories.AddRangeAsync(categoriesExampleList);
        genresExampleList.ForEach(async genre =>
        {
            var random = new Random();
            var startPoint = random.Next(0, categoriesExampleList.Count);
            var itemsToTake = random.Next(0, 4);
            for (int i = startPoint; i < categoriesExampleList.Count; i++)
            {
                if (itemsToTake > 0)
                {
                    var categoryId = categoriesExampleList[i].Id;
                    genre.AddCategory(categoryId);
                    itemsToTake--;
                    await dbContext.GenresCategories.AddAsync(new GenresCategories(categoryId, genre.Id));
                }
            }
        });
        await dbContext.SaveChangesAsync();
        var input = new ListGenresInput(1, 20);
        var useCase = new ListGenres(repository, new CategoryRepository(dbContext));

        // When
        var output = await useCase.Handle(input, CancellationToken.None);

        // Then
        output.Should().NotBeNull();
        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Total.Should().Be(genresExampleList.Count);
        output.Items.Should().HaveCount(genresExampleList.Count);
        output.Items.ToList().ForEach(outputItem =>
        {
            var targetGenre = genresExampleList.First(x => x.Id == outputItem.Id);
            outputItem.Should().NotBeNull();
            outputItem.Name.Should().Be(targetGenre.Name);
            outputItem.IsActive.Should().Be(targetGenre.IsActive);
            outputItem.CreatedAt.Should().Be(targetGenre.CreatedAt);
            outputItem.Catetories.Should().HaveCount(targetGenre.Categories.Count);
            outputItem.Catetories.ToList().ForEach(category =>
            {
                var targetCategory = categoriesExampleList.First(c => c.Id == category.Id);
                category.Should().NotBeNull();
                category.Name.Should().Be(targetCategory.Name);
            });
        });
    }

    [Theory(DisplayName = nameof(ListGenresPaginated))]
    [Trait("Integration/Application", "ListGenres - Use Cases")]
    [InlineData(10, 1, 5, 5)]
    [InlineData(10, 2, 5, 5)]
    [InlineData(7, 2, 5, 2)]
    [InlineData(7, 3, 5, 0)]
    public async void ListGenresPaginated(int numberOfGenresToGenerate, int page, int perPage, int expectedNumberOfItems)
    {
        // Given
        var dbContext = _fixture.CreateDbContext();
        var repository = new GenreRepository(dbContext);
        var genresExampleList = _fixture.GetExampleGenresList(numberOfGenresToGenerate);
        var categoriesExampleList = _fixture.GetExampleCategoriesList(10);
        await dbContext.Genres.AddRangeAsync(genresExampleList);
        await dbContext.Categories.AddRangeAsync(categoriesExampleList);
        genresExampleList.ForEach(async genre =>
        {
            var random = new Random();
            var startPoint = random.Next(0, categoriesExampleList.Count);
            var itemsToTake = random.Next(0, 4);
            for (int i = startPoint; i < categoriesExampleList.Count; i++)
            {
                if (itemsToTake > 0)
                {
                    var categoryId = categoriesExampleList[i].Id;
                    genre.AddCategory(categoryId);
                    itemsToTake--;
                    await dbContext.GenresCategories.AddAsync(new GenresCategories(categoryId, genre.Id));
                }
            }
        });
        await dbContext.SaveChangesAsync();
        var input = new ListGenresInput(page, perPage);
        var useCase = new ListGenres(repository, new CategoryRepository(dbContext));

        // When
        var output = await useCase.Handle(input, CancellationToken.None);

        // Then
        output.Should().NotBeNull();
        output.Page.Should().Be(page);
        output.PerPage.Should().Be(perPage);
        output.Total.Should().Be(numberOfGenresToGenerate);
        output.Items.Should().HaveCount(expectedNumberOfItems);
        output.Items.ToList().ForEach(outputItem =>
        {
            var targetGenre = genresExampleList.First(x => x.Id == outputItem.Id);
            outputItem.Should().NotBeNull();
            outputItem.Name.Should().Be(targetGenre.Name);
            outputItem.IsActive.Should().Be(targetGenre.IsActive);
            outputItem.CreatedAt.Should().Be(targetGenre.CreatedAt);
            outputItem.Catetories.Should().HaveCount(targetGenre.Categories.Count);
            outputItem.Catetories.ToList().ForEach(category =>
            {
                var targetCategory = categoriesExampleList.First(c => c.Id == category.Id);
                category.Should().NotBeNull();
                category.Name.Should().Be(targetCategory.Name);
            });
        });
    }

    [Theory(DisplayName = nameof(SearchByText))]
    [Trait("Integration/Application", "ListGenres - Use Cases")]
    [InlineData("Action", 1, 5, 1, 1)]
    [InlineData("Horror", 1, 5, 3, 3)]
    [InlineData("Horror", 2, 5, 0, 3)]
    [InlineData("Sci-fi", 1, 5, 4, 4)]
    [InlineData("Sci-fi", 1, 2, 2, 4)]
    [InlineData("Sci-fi", 2, 3, 1, 4)]
    [InlineData("Romance", 1, 5, 0, 0)]
    [InlineData("Robots", 1, 5, 2, 2)]
    public async void SearchByText(string search, int page, int perPage, int expectedNumberOfItems, int expectedTotalItems)
    {
        // Given
        var dbContext = _fixture.CreateDbContext();
        var repository = new GenreRepository(dbContext);
        var genresExampleList = _fixture.GetExampleGenresListWithNames(new List<string>(){
            "Action",
            "Horror",
            "Horror - Robots",
            "Horror - Based on Real Facts",
            "Drama",
            "Sci-fi IA",
            "Sci-fi Robots",
            "Sci-fi Space",
            "Sci-fi Future",
        });
        var categoriesExampleList = _fixture.GetExampleCategoriesList(10);
        await dbContext.Genres.AddRangeAsync(genresExampleList);
        await dbContext.Categories.AddRangeAsync(categoriesExampleList);
        genresExampleList.ForEach(async genre =>
        {
            var random = new Random();
            var startPoint = random.Next(0, categoriesExampleList.Count);
            var itemsToTake = random.Next(0, 4);
            for (int i = startPoint; i < categoriesExampleList.Count; i++)
            {
                if (itemsToTake > 0)
                {
                    var categoryId = categoriesExampleList[i].Id;
                    genre.AddCategory(categoryId);
                    itemsToTake--;
                    await dbContext.GenresCategories.AddAsync(new GenresCategories(categoryId, genre.Id));
                }
            }
        });
        await dbContext.SaveChangesAsync();
        var input = new ListGenresInput(page, perPage, search);
        var useCase = new ListGenres(repository, new CategoryRepository(dbContext));

        // When
        var output = await useCase.Handle(input, CancellationToken.None);

        // Then
        output.Should().NotBeNull();
        output.Page.Should().Be(page);
        output.PerPage.Should().Be(perPage);
        output.Total.Should().Be(expectedTotalItems);
        output.Items.Should().HaveCount(expectedNumberOfItems);
        output.Items.ToList().ForEach(outputItem =>
        {
            var targetGenre = genresExampleList.First(x => x.Id == outputItem.Id);
            outputItem.Should().NotBeNull();
            outputItem.Name.Should().Be(targetGenre.Name);
            outputItem.IsActive.Should().Be(targetGenre.IsActive);
            outputItem.CreatedAt.Should().Be(targetGenre.CreatedAt);
            outputItem.Catetories.Should().HaveCount(targetGenre.Categories.Count);
            outputItem.Catetories.ToList().ForEach(category =>
            {
                var targetCategory = categoriesExampleList.First(c => c.Id == category.Id);
                category.Should().NotBeNull();
                category.Name.Should().Be(targetCategory.Name);
            });
        });
    }

    [Theory(DisplayName = nameof(SearchOrdered))]
    [Trait("Integration/Application", "ListGenres - Use Cases")]
    [InlineData("name", "asc")]
    [InlineData("name", "desc")]
    [InlineData("id", "asc")]
    [InlineData("id", "desc")]
    [InlineData("createdAt", "asc")]
    [InlineData("createdAt", "desc")]
    [InlineData("", "asc")]
    public async Task SearchOrdered(string orderBy, string order)
    {
        // Given
        var dbContext = _fixture.CreateDbContext();
        var repository = new GenreRepository(dbContext);
        var genresExampleList = _fixture.GetExampleGenresList(10);
        var categoriesExampleList = _fixture.GetExampleCategoriesList(10);
        await dbContext.Genres.AddRangeAsync(genresExampleList);
        await dbContext.Categories.AddRangeAsync(categoriesExampleList);
        genresExampleList.ForEach(async genre =>
        {
            var random = new Random();
            var startPoint = random.Next(0, categoriesExampleList.Count);
            var itemsToTake = random.Next(0, 4);
            for (int i = startPoint; i < categoriesExampleList.Count; i++)
            {
                if (itemsToTake > 0)
                {
                    var categoryId = categoriesExampleList[i].Id;
                    genre.AddCategory(categoryId);
                    itemsToTake--;
                    await dbContext.GenresCategories.AddAsync(new GenresCategories(categoryId, genre.Id));
                }
            }
        });
        await dbContext.SaveChangesAsync();

        var searchOrder = order.ToLower() == "asc" ? SearchOrder.Asc : SearchOrder.Desc;
        var input = new ListGenresInput(1, 20, "", orderBy, searchOrder);
        var useCase = new ListGenres(repository, new CategoryRepository(dbContext));
        var expectedOrderedList = _fixture.CloneGenreListOrdered(genresExampleList, orderBy, searchOrder);

        // When
        var output = await useCase.Handle(input, CancellationToken.None);

        // Then
        output.Should().NotBeNull();
        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Total.Should().Be(genresExampleList.Count);
        output.Items.Should().HaveCount(genresExampleList.Count);

        for (int index = 0; index < expectedOrderedList.Count(); index++)
        {
            var expecetedItem = expectedOrderedList[index];
            var outputItem = output.Items[index];
            expecetedItem.Should().NotBeNull();
            outputItem.Should().NotBeNull();
            outputItem!.Id.Should().Be(expecetedItem!.Id);
            outputItem.Name.Should().Be(expecetedItem.Name);
            outputItem.IsActive.Should().Be(expecetedItem.IsActive);
            outputItem.CreatedAt.Should().Be(expecetedItem.CreatedAt);
            outputItem.Catetories.Should().HaveCount(expecetedItem.Categories.Count);
            outputItem.Catetories.ToList().ForEach(category =>
            {
                var targetCategory = categoriesExampleList.First(c => c.Id == category.Id);
                category.Should().NotBeNull();
                category.Name.Should().Be(targetCategory.Name);
            });
        }
    }
}