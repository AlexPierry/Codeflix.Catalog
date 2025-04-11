using Application.Exceptions;
using Domain.Entity;
using Domain.SeedWork.SearchableRepository;
using FluentAssertions;
using Infra.Data.EF;
using Infra.Data.EF.Models;
using Infra.Data.EF.Repositories;
using Microsoft.EntityFrameworkCore;

namespace IntegrationTests.Infra.Data.EF.Repositories;

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

    [Fact(DisplayName = nameof(GetOk))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    public async Task GetOk()
    {
        // Given
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleGenre = _fixture.GetExampleGenre();
        var categoriesExampleList = _fixture.GetExampleCategoriesList(3);
        categoriesExampleList.ForEach(category => exampleGenre.AddCategory(category.Id));
        await dbContext.Categories.AddRangeAsync(categoriesExampleList);
        await dbContext.Genres.AddAsync(exampleGenre);
        foreach (var categoryId in exampleGenre.Categories)
        {
            var relation = new GenresCategories(categoryId, exampleGenre.Id);
            await dbContext.GenresCategories.AddAsync(relation);
        }
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var genreRepository = new GenreRepository(_fixture.CreateDbContext(true));

        // When
        var genreFromRepository = await genreRepository.Get(exampleGenre.Id, CancellationToken.None);

        // Then
        genreFromRepository.Should().NotBeNull();
        genreFromRepository!.Name.Should().Be(exampleGenre.Name);
        genreFromRepository.IsActive.Should().Be(exampleGenre.IsActive);
        genreFromRepository.CreatedAt.Should().Be(exampleGenre.CreatedAt);
        genreFromRepository.Categories.Should().HaveCount(categoriesExampleList.Count);
        genreFromRepository.Categories.Except(exampleGenre.Categories).Should().HaveCount(0);
    }

    [Fact(DisplayName = nameof(GetThrowsExceptionWhenNotFound))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    public async Task GetThrowsExceptionWhenNotFound()
    {
        // Given
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleGenre = _fixture.GetExampleGenre();
        var categoriesExampleList = _fixture.GetExampleCategoriesList(3);
        categoriesExampleList.ForEach(category => exampleGenre.AddCategory(category.Id));
        await dbContext.Categories.AddRangeAsync(categoriesExampleList);
        await dbContext.Genres.AddAsync(exampleGenre);
        foreach (var categoryId in exampleGenre.Categories)
        {
            var relation = new GenresCategories(categoryId, exampleGenre.Id);
            await dbContext.GenresCategories.AddAsync(relation);
        }
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var genreRepository = new GenreRepository(_fixture.CreateDbContext(true));
        var randomId = Guid.NewGuid();

        // When
        var task = async () => await genreRepository.Get(randomId, CancellationToken.None);

        // Then
        await task.Should().ThrowAsync<NotFoundException>().WithMessage($"Genre '{randomId}' not found.");
    }

    [Fact(DisplayName = nameof(DeleteOk))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    public async Task DeleteOk()
    {
        // Given
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleGenre = _fixture.GetExampleGenre();
        var categoriesExampleList = _fixture.GetExampleCategoriesList(3);
        categoriesExampleList.ForEach(category => exampleGenre.AddCategory(category.Id));
        await dbContext.Categories.AddRangeAsync(categoriesExampleList);
        await dbContext.Genres.AddAsync(exampleGenre);
        foreach (var categoryId in exampleGenre.Categories)
        {
            var relation = new GenresCategories(categoryId, exampleGenre.Id);
            await dbContext.GenresCategories.AddAsync(relation);
        }
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var repositoryDbContext = _fixture.CreateDbContext(true);
        var genreRepository = new GenreRepository(repositoryDbContext);

        // When
        await genreRepository.Delete(exampleGenre, CancellationToken.None);
        await repositoryDbContext.SaveChangesAsync();

        // Then
        var assertDbContext = _fixture.CreateDbContext(true);
        var dbGenre = assertDbContext.Genres.AsNoTracking().FirstOrDefault(x => x.Id == exampleGenre.Id);

        dbGenre.Should().BeNull();
        var categoriesIdsList = await assertDbContext.GenresCategories
            .AsNoTracking()
            .Where(x => x.GenreId == exampleGenre.Id)
            .Select(x => x.CategoryId)
            .ToListAsync();
        categoriesIdsList.Should().HaveCount(0);
    }

    [Fact(DisplayName = nameof(UpdateOk))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    public async Task UpdateOk()
    {
        // Given
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleGenre = _fixture.GetExampleGenre();
        var categoriesExampleList = _fixture.GetExampleCategoriesList(3);
        categoriesExampleList.ForEach(category => exampleGenre.AddCategory(category.Id));
        await dbContext.Categories.AddRangeAsync(categoriesExampleList);
        await dbContext.Genres.AddAsync(exampleGenre);
        foreach (var categoryId in exampleGenre.Categories)
        {
            var relation = new GenresCategories(categoryId, exampleGenre.Id);
            await dbContext.GenresCategories.AddAsync(relation);
        }
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var givenDbContext = _fixture.CreateDbContext(true);
        var genreRepository = new GenreRepository(givenDbContext);
        exampleGenre.Update(_fixture.GetValidGenreName());
        if (exampleGenre.IsActive)
        {
            exampleGenre.Deactivate();
        }
        else
        {
            exampleGenre.Activate();
        }

        // When
        await genreRepository.Update(exampleGenre, CancellationToken.None);
        await givenDbContext.SaveChangesAsync();

        // Then
        var assertsDbContext = _fixture.CreateDbContext(true);
        var genreFromDb = await assertsDbContext.Genres.FindAsync(exampleGenre.Id);
        genreFromDb.Should().NotBeNull();
        genreFromDb!.Name.Should().Be(exampleGenre.Name);
        genreFromDb.IsActive.Should().Be(exampleGenre.IsActive);
        genreFromDb.CreatedAt.Should().Be(exampleGenre.CreatedAt);
        var genresCategoriesRelation = await assertsDbContext
            .GenresCategories.Where(r => r.GenreId == exampleGenre.Id).ToListAsync();
        genresCategoriesRelation.Should().HaveCount(categoriesExampleList.Count);
        genresCategoriesRelation.Select(x => x.CategoryId).ToList()
            .Except(exampleGenre.Categories).Should().HaveCount(0);
    }

    [Fact(DisplayName = nameof(UpdateRemovingRelations))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    public async Task UpdateRemovingRelations()
    {
        // Given
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleGenre = _fixture.GetExampleGenre();
        var categoriesExampleList = _fixture.GetExampleCategoriesList(3);
        categoriesExampleList.ForEach(category => exampleGenre.AddCategory(category.Id));
        await dbContext.Categories.AddRangeAsync(categoriesExampleList);
        await dbContext.Genres.AddAsync(exampleGenre);
        foreach (var categoryId in exampleGenre.Categories)
        {
            var relation = new GenresCategories(categoryId, exampleGenre.Id);
            await dbContext.GenresCategories.AddAsync(relation);
        }
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var givenDbContext = _fixture.CreateDbContext(true);
        var genreRepository = new GenreRepository(givenDbContext);
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

        // When
        await genreRepository.Update(exampleGenre, CancellationToken.None);
        await givenDbContext.SaveChangesAsync();

        // Then
        var assertsDbContext = _fixture.CreateDbContext(true);
        var genreFromDb = await assertsDbContext.Genres.FindAsync(exampleGenre.Id);
        genreFromDb.Should().NotBeNull();
        genreFromDb!.Name.Should().Be(exampleGenre.Name);
        genreFromDb.IsActive.Should().Be(exampleGenre.IsActive);
        genreFromDb.CreatedAt.Should().Be(exampleGenre.CreatedAt);
        var genresCategoriesRelation = await assertsDbContext
            .GenresCategories.Where(r => r.GenreId == exampleGenre.Id).ToListAsync();
        genresCategoriesRelation.Should().HaveCount(0);
    }

    [Fact(DisplayName = nameof(UpdateReplacingRelations))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    public async Task UpdateReplacingRelations()
    {
        // Given
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleGenre = _fixture.GetExampleGenre();
        var categoriesExampleList = _fixture.GetExampleCategoriesList(3);
        var updateCategoriesExampleList = _fixture.GetExampleCategoriesList(5);
        categoriesExampleList.ForEach(category => exampleGenre.AddCategory(category.Id));
        await dbContext.Categories.AddRangeAsync(categoriesExampleList);
        await dbContext.Categories.AddRangeAsync(updateCategoriesExampleList);
        await dbContext.Genres.AddAsync(exampleGenre);
        foreach (var categoryId in exampleGenre.Categories)
        {
            var relation = new GenresCategories(categoryId, exampleGenre.Id);
            await dbContext.GenresCategories.AddAsync(relation);
        }
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var givenDbContext = _fixture.CreateDbContext(true);
        var genreRepository = new GenreRepository(givenDbContext);
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
        updateCategoriesExampleList.ForEach(category => exampleGenre.AddCategory(category.Id));

        // When
        await genreRepository.Update(exampleGenre, CancellationToken.None);
        await givenDbContext.SaveChangesAsync();

        // Then
        var assertsDbContext = _fixture.CreateDbContext(true);
        var genreFromDb = await assertsDbContext.Genres.FindAsync(exampleGenre.Id);
        genreFromDb.Should().NotBeNull();
        genreFromDb!.Name.Should().Be(exampleGenre.Name);
        genreFromDb.IsActive.Should().Be(exampleGenre.IsActive);
        genreFromDb.CreatedAt.Should().Be(exampleGenre.CreatedAt);
        var genresCategoriesRelation = await assertsDbContext
            .GenresCategories.Where(r => r.GenreId == exampleGenre.Id).ToListAsync();
        genresCategoriesRelation.Should().HaveCount(updateCategoriesExampleList.Count);
        genresCategoriesRelation.Select(x => x.CategoryId).ToList()
            .Except(exampleGenre.Categories).Should().HaveCount(0);
    }

    [Fact(DisplayName = nameof(SearchReturnsEmptyWhenPersistenceIsEmpty))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    public async Task SearchReturnsEmptyWhenPersistenceIsEmpty()
    {
        // Given
        var dbContext = _fixture.CreateDbContext();
        var genreRepository = new GenreRepository(dbContext);
        var searchInput = new SearchInput(1, 20, "", "", SearchOrder.Asc);

        // When
        var output = await genreRepository.Search(searchInput, CancellationToken.None);

        // Then        
        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.CurrentPage.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(0);
        output.Items.Should().HaveCount(0);
    }

    [Fact(DisplayName = nameof(SearchReturnsListAndTotal))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    public async Task SearchReturnsListAndTotal()
    {
        // Given
        var dbContext = _fixture.CreateDbContext();
        var exampleGenresList = _fixture.GetExampleGenresList(15);
        await dbContext.AddRangeAsync(exampleGenresList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var genreRepository = new GenreRepository(dbContext);
        var searchInput = new SearchInput(1, 20, "", "", SearchOrder.Asc);

        // When
        var output = await genreRepository.Search(searchInput, CancellationToken.None);

        // Then        
        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.CurrentPage.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(exampleGenresList.Count);
        output.Items.Should().HaveCount(exampleGenresList.Count);
        foreach (Genre outputItem in output.Items)
        {
            var exampleItem = exampleGenresList.Find(x => x.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();
            outputItem!.Name.Should().Be(exampleItem!.Name);
            outputItem.IsActive.Should().Be(exampleItem.IsActive);
            outputItem.CreatedAt.Should().Be(exampleItem.CreatedAt);
        }
    }

    [Fact(DisplayName = nameof(SearchReturnsRelations))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    public async Task SearchReturnsRelations()
    {
        // Given
        var dbContext = _fixture.CreateDbContext();
        var exampleGenresList = _fixture.GetExampleGenresList(15);
        await dbContext.AddRangeAsync(exampleGenresList);
        var random = new Random();
        exampleGenresList.ForEach(exampleGenre =>
        {
            var categoriesListToRelation = _fixture.GetExampleCategoriesList(random.Next(0, 5));
            dbContext.Categories.AddRange(categoriesListToRelation);
            categoriesListToRelation.ForEach(async category =>
            {
                await dbContext.GenresCategories
                    .AddAsync(new GenresCategories(category.Id, exampleGenre.Id), CancellationToken.None);
                exampleGenre.AddCategory(category.Id);
            });
        });
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var genreRepository = new GenreRepository(dbContext);
        var searchInput = new SearchInput(1, 20, "", "", SearchOrder.Asc);

        // When
        var output = await genreRepository.Search(searchInput, CancellationToken.None);

        // Then        
        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.CurrentPage.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(exampleGenresList.Count);
        output.Items.Should().HaveCount(exampleGenresList.Count);
        foreach (Genre outputItem in output.Items)
        {
            var exampleGenre = exampleGenresList.Find(x => x.Id == outputItem.Id);
            exampleGenre.Should().NotBeNull();
            outputItem!.Name.Should().Be(exampleGenre!.Name);
            outputItem.IsActive.Should().Be(exampleGenre.IsActive);
            outputItem.CreatedAt.Should().Be(exampleGenre.CreatedAt);
            outputItem.Categories.Should().BeEquivalentTo(exampleGenre.Categories);
            outputItem.Categories.Should().HaveCount(exampleGenre.Categories.Count);
        }
    }

    [Theory(DisplayName = nameof(SearchReturnsPaginated))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    [InlineData(10, 1, 5, 5)]
    [InlineData(10, 2, 5, 5)]
    [InlineData(7, 2, 5, 2)]
    [InlineData(7, 3, 5, 0)]
    public async Task SearchReturnsPaginated(int numberOfGenresToGenerate, int page, int perPage, int expectedNumberOfItems)
    {
        {
            // Given
            var dbContext = _fixture.CreateDbContext();
            var exampleGenresList = _fixture.GetExampleGenresList(numberOfGenresToGenerate);
            await dbContext.AddRangeAsync(exampleGenresList);
            var random = new Random();
            exampleGenresList.ForEach(exampleGenre =>
            {
                var categoriesListToRelation = _fixture.GetExampleCategoriesList(random.Next(0, 5));
                dbContext.Categories.AddRange(categoriesListToRelation);
                categoriesListToRelation.ForEach(async category =>
                {
                    await dbContext.GenresCategories
                        .AddAsync(new GenresCategories(category.Id, exampleGenre.Id), CancellationToken.None);
                    exampleGenre.AddCategory(category.Id);
                });
            });
            await dbContext.SaveChangesAsync(CancellationToken.None);
            var genreRepository = new GenreRepository(dbContext);
            var searchInput = new SearchInput(page, perPage, "", "", SearchOrder.Asc);

            // When
            var output = await genreRepository.Search(searchInput, CancellationToken.None);

            // Then        
            output.Should().NotBeNull();
            output.Items.Should().NotBeNull();
            output.CurrentPage.Should().Be(searchInput.Page);
            output.PerPage.Should().Be(searchInput.PerPage);
            output.Total.Should().Be(exampleGenresList.Count);
            output.Items.Should().HaveCount(expectedNumberOfItems);
            foreach (Genre outputItem in output.Items)
            {
                var exampleGenre = exampleGenresList.Find(x => x.Id == outputItem.Id);
                exampleGenre.Should().NotBeNull();
                outputItem!.Name.Should().Be(exampleGenre!.Name);
                outputItem.IsActive.Should().Be(exampleGenre.IsActive);
                outputItem.CreatedAt.Should().Be(exampleGenre.CreatedAt);
                outputItem.Categories.Should().BeEquivalentTo(exampleGenre.Categories);
                outputItem.Categories.Should().HaveCount(exampleGenre.Categories.Count);
            }
        }
    }

    [Theory(DisplayName = nameof(SearchByText))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    [InlineData("Action", 1, 5, 1, 1)]
    [InlineData("Horror", 1, 5, 3, 3)]
    [InlineData("Horror", 2, 5, 0, 3)]
    [InlineData("Sci-fi", 1, 5, 4, 4)]
    [InlineData("Sci-fi", 1, 2, 2, 4)]
    [InlineData("Sci-fi", 2, 3, 1, 4)]
    [InlineData("Romance", 1, 5, 0, 0)]
    [InlineData("Robots", 1, 5, 2, 2)]
    public async Task SearchByText(string search, int page, int perPage, int expectedNumberOfItems, int expectedTotalItems)
    {
        // Given
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleGenresList = _fixture.GetExampleGenresListWithNames(new List<string>(){
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
        await dbContext.AddRangeAsync(exampleGenresList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var genreRepository = new GenreRepository(dbContext);
        var searchInput = new SearchInput(page, perPage, search, "", SearchOrder.Asc);

        // When
        var output = await genreRepository.Search(searchInput, CancellationToken.None);

        // Then        
        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.CurrentPage.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(expectedTotalItems);
        output.Items.Should().HaveCount(expectedNumberOfItems);
        foreach (Genre outputItem in output.Items)
        {
            var exampleItem = exampleGenresList.Find(x => x.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();
            outputItem!.Name.Should().Be(exampleItem!.Name);
            outputItem.IsActive.Should().Be(exampleItem.IsActive);
            outputItem.CreatedAt.Should().Be(exampleItem.CreatedAt);
        }
    }

    [Theory(DisplayName = nameof(SearchOrdered))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
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
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleGenresList = _fixture.GetExampleGenresList();
        await dbContext.AddRangeAsync(exampleGenresList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var genreRepository = new GenreRepository(dbContext);
        var searchOrder = order.ToLower() == "asc" ? SearchOrder.Asc : SearchOrder.Desc;
        var searchInput = new SearchInput(1, 20, "", orderBy, searchOrder);
        var expectedOrderedList = _fixture.CloneGenreListOrdered(exampleGenresList, orderBy, searchOrder);

        // When
        var output = await genreRepository.Search(searchInput, CancellationToken.None);

        // Then
        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.CurrentPage.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(exampleGenresList.Count());
        output.Items.Should().HaveCount(exampleGenresList.Count);
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
        }
    }

    [Fact(DisplayName = nameof(GetIdsListByIds))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    public async Task GetIdsListByIds()
    {
        // Given
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleGenresList = _fixture.GetExampleGenresList(10);
        await dbContext.AddRangeAsync(exampleGenresList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var genreRepository = new GenreRepository(_fixture.CreateDbContext(true));
        var idsToSearch = exampleGenresList.Select(x => x.Id).Take(5).ToList();

        // When
        var output = await genreRepository.GetIdsListByIds(idsToSearch, CancellationToken.None);

        // Then
        output.Should().NotBeNull();
        output.Should().HaveCount(idsToSearch.Count);
        output.Should().BeEquivalentTo(idsToSearch);
    }

    [Fact(DisplayName = nameof(GetIdsListByIdsWhenOnlyThreeIdsMatched))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    public async Task GetIdsListByIdsWhenOnlyThreeIdsMatched()
    {
        // Given
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleGenresList = _fixture.GetExampleGenresList(10);
        await dbContext.AddRangeAsync(exampleGenresList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var genreRepository = new GenreRepository(_fixture.CreateDbContext(true));
        var idsToSearch = exampleGenresList.Select(x => x.Id).Take(3).ToList();
        var idsToCheck = idsToSearch.ToList();
        idsToSearch.Add(Guid.NewGuid());
        idsToSearch.Add(Guid.NewGuid());

        // When
        var output = await genreRepository.GetIdsListByIds(idsToSearch, CancellationToken.None);

        // Then
        output.Should().NotBeNull();
        output.Should().HaveCount(3);
        output.Should().BeEquivalentTo(idsToCheck);
    }

    [Fact(DisplayName = nameof(GetListByIds))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    public async Task GetListByIds()
    {
        // Given
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleGenresList = _fixture.GetExampleGenresList(10);
        await dbContext.AddRangeAsync(exampleGenresList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var genreRepository = new GenreRepository(_fixture.CreateDbContext(true));
        var idsToSearch = exampleGenresList.Select(x => x.Id).Take(5).ToList();

        // When
        var output = await genreRepository.GetListByIds(idsToSearch, CancellationToken.None);

        // Then
        output.Should().NotBeNull();
        output.Should().HaveCount(idsToSearch.Count);
        output.Should().BeEquivalentTo(exampleGenresList.Where(x => idsToSearch.Contains(x.Id)));
        foreach (var outputItem in output)
        {
            var exampleItem = exampleGenresList.Find(x => x.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();
            outputItem!.Name.Should().Be(exampleItem!.Name);
            outputItem.IsActive.Should().Be(exampleItem.IsActive);
            outputItem.CreatedAt.Should().Be(exampleItem.CreatedAt);
        }
    }

    [Fact(DisplayName = nameof(GetListByIdsWhenOnlyThreeIdsMatched))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    public async Task GetListByIdsWhenOnlyThreeIdsMatched()
    {
        // Given
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleGenresList = _fixture.GetExampleGenresList(10);
        await dbContext.AddRangeAsync(exampleGenresList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var genreRepository = new GenreRepository(_fixture.CreateDbContext(true));
        var idsToSearch = exampleGenresList.Select(x => x.Id).Take(3).ToList();
        var idsToCheck = idsToSearch.ToList();
        idsToSearch.Add(Guid.NewGuid());
        idsToSearch.Add(Guid.NewGuid());

        // When
        var output = await genreRepository.GetListByIds(idsToSearch, CancellationToken.None);

        // Then
        output.Should().NotBeNull();
        output.Should().HaveCount(3);
        output.Should().BeEquivalentTo(exampleGenresList.Where(x => idsToCheck.Contains(x.Id)));
    }        
}