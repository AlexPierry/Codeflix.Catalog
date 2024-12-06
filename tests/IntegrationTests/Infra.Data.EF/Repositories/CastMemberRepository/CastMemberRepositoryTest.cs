using Application.Exceptions;
using Domain.Entity;
using Domain.SeedWork.SearchableRepository;
using FluentAssertions;
using Infra.Data.EF.Repositories;
using Microsoft.EntityFrameworkCore;

namespace IntegrationTest.Infra.Data.EF.Repositories;

[Collection(nameof(CastMemberRepositoryTestFixture))]
public class CastMemberRepositoryTest
{
    private readonly CastMemberRepositoryTestFixture _fixture;

    public CastMemberRepositoryTest(CastMemberRepositoryTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(InsertOk))]
    [Trait("Integration/Infra.Data", "CastMemberRepository - Repositories")]
    public async Task InsertOk()
    {
        // Given
        var dbContext = _fixture.CreateDbContext();
        var exampleCastMember = _fixture.GetExampleCastMember();
        var castMemberRepository = new CastMemberRepository(dbContext);

        // When
        await castMemberRepository.Insert(exampleCastMember, CancellationToken.None);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        // Then
        var dbCastMember = await dbContext.CastMembers.AsNoTracking().FirstOrDefaultAsync(x => x.Id == exampleCastMember.Id);
        dbCastMember.Should().NotBeNull();
        dbCastMember!.Name.Should().Be(exampleCastMember.Name);
        dbCastMember.Type.Should().Be(exampleCastMember.Type);
        dbCastMember.CreatedAt.Should().Be(exampleCastMember.CreatedAt);
    }

    [Fact(DisplayName = nameof(GetOk))]
    [Trait("Integration/Infra.Data", "CastMemberRepository - Repositories")]
    public async Task GetOk()
    {
        // Given
        var dbContext = _fixture.CreateDbContext();
        var exampleCastMembersList = _fixture.GetExampleCastMembersList(15);
        var exampleCastMember = exampleCastMembersList[5];
        await dbContext.AddRangeAsync(exampleCastMembersList);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var castMemberRepository = new CastMemberRepository(_fixture.CreateDbContext(true)); // new dbContext to avoid cache

        // When
        var dbCastMember = await castMemberRepository.Get(exampleCastMember.Id, CancellationToken.None);

        // Then        
        dbCastMember.Should().NotBeNull();
        dbCastMember!.Id.Should().Be(exampleCastMember.Id);
        dbCastMember.Name.Should().Be(exampleCastMember.Name);
        dbCastMember.Type.Should().Be(exampleCastMember.Type);
        dbCastMember.CreatedAt.Should().Be(exampleCastMember.CreatedAt);
    }

    [Fact(DisplayName = nameof(GetThrowsExceptionWhenNotFound))]
    [Trait("Integration/Infra.Data", "CastMemberRepository - Repositories")]
    public async Task GetThrowsExceptionWhenNotFound()
    {
        // Given
        var dbContext = _fixture.CreateDbContext();
        var exampleId = Guid.NewGuid();
        await dbContext.AddRangeAsync(_fixture.GetExampleCastMembersList(15));
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var castMemberRepository = new CastMemberRepository(dbContext);

        // When
        var task = async () => await castMemberRepository.Get(exampleId, CancellationToken.None);

        // Then        
        await task.Should().ThrowAsync<NotFoundException>($"CastMember '{exampleId}' not found.");
    }

    [Fact(DisplayName = nameof(DeleteOk))]
    [Trait("Integration/Infra.Data", "CastMemberRepository - Repositories")]
    public async Task DeleteOk()
    {
        // Given
        var dbContext = _fixture.CreateDbContext();
        var exampleCastMembersList = _fixture.GetExampleCastMembersList(15);
        var exampleCastMember = exampleCastMembersList[5];
        await dbContext.AddRangeAsync(exampleCastMembersList);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var castMemberRepository = new CastMemberRepository(dbContext);

        // When
        await castMemberRepository.Delete(exampleCastMember, CancellationToken.None);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        // Then        
        var assertionDbContext = _fixture.CreateDbContext(true); // to avoid cache
        var dbCastMember = await assertionDbContext.Categories.FindAsync(exampleCastMember.Id);
        dbCastMember.Should().BeNull();
        assertionDbContext.CastMembers.Should().HaveCount(exampleCastMembersList.Count - 1);
        assertionDbContext.CastMembers.Should().NotContain(exampleCastMember);
    }

    [Fact(DisplayName = nameof(UpdateOk))]
    [Trait("Integration/Infra.Data", "CastMemberRepository - Repositories")]
    public async Task UpdateOk()
    {
        // Given
        var dbContext = _fixture.CreateDbContext();
        var exampleCastMembersList = _fixture.GetExampleCastMembersList(15);
        var exampleCastMember = exampleCastMembersList[5];
        await dbContext.AddRangeAsync(exampleCastMembersList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var castMemberRepository = new CastMemberRepository(dbContext);
        var newCastMemberValues = _fixture.GetExampleCastMember();
        exampleCastMember.Update(newCastMemberValues.Name, newCastMemberValues.Type);

        // When
        await castMemberRepository.Update(exampleCastMember, CancellationToken.None);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        // Then        
        var assertDbContext = _fixture.CreateDbContext(true); // to avoid cache
        var dbCastMember = await assertDbContext.CastMembers.FindAsync(exampleCastMember.Id);
        dbCastMember.Should().NotBeNull();
        dbCastMember!.Id.Should().Be(exampleCastMember.Id);
        dbCastMember.Name.Should().Be(exampleCastMember.Name);
        dbCastMember.Type.Should().Be(exampleCastMember.Type);
        dbCastMember.CreatedAt.Should().Be(exampleCastMember.CreatedAt);
    }

    [Fact(DisplayName = nameof(SearchReturnsListAndTotal))]
    [Trait("Integration/Infra.Data", "CastMemberRepository - Repositories")]
    public async Task SearchReturnsListAndTotal()
    {
        // Given
        var dbContext = _fixture.CreateDbContext();
        var exampleCastMembersList = _fixture.GetExampleCastMembersList(15);
        await dbContext.AddRangeAsync(exampleCastMembersList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var castMemberRepository = new CastMemberRepository(dbContext);
        var searchInput = new SearchInput(1, 20, "", "", SearchOrder.Asc);

        // When
        var output = await castMemberRepository.Search(searchInput, CancellationToken.None);

        // Then        
        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.CurrentPage.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(exampleCastMembersList.Count);
        output.Items.Should().HaveCount(exampleCastMembersList.Count);
        foreach (var outputItem in output.Items)
        {
            var exampleItem = exampleCastMembersList.Find(x => x.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();
            outputItem!.Name.Should().Be(exampleItem!.Name);
            outputItem.Type.Should().Be(exampleItem.Type);
            outputItem.CreatedAt.Should().Be(exampleItem.CreatedAt);
        }
    }

    [Fact(DisplayName = nameof(SearchReturnsEmptyWhenPersistenceIsEmpty))]
    [Trait("Integration/Infra.Data", "CastMemberRepository - Repositories")]
    public async Task SearchReturnsEmptyWhenPersistenceIsEmpty()
    {
        // Given
        var dbContext = _fixture.CreateDbContext();
        var castMemberRepository = new CastMemberRepository(dbContext);
        var searchInput = new SearchInput(1, 20, "", "", SearchOrder.Asc);

        // When
        var output = await castMemberRepository.Search(searchInput, CancellationToken.None);

        // Then        
        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.CurrentPage.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(0);
        output.Items.Should().HaveCount(0);
    }

    [Theory(DisplayName = nameof(SearchReturnsPaginated))]
    [Trait("Integration/Infra.Data", "CastMemberRepository - Repositories")]
    [InlineData(10, 1, 5, 5)]
    [InlineData(10, 2, 5, 5)]
    [InlineData(7, 2, 5, 2)]
    [InlineData(7, 3, 5, 0)]
    public async Task SearchReturnsPaginated(int numberOfCastMembersToGenerate, int page, int perPage, int expectedNumberOfItems)
    {
        // Given
        var dbContext = _fixture.CreateDbContext();
        var exampleCastMembersList = _fixture.GetExampleCastMembersList(numberOfCastMembersToGenerate);
        await dbContext.AddRangeAsync(exampleCastMembersList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var castMemberRepository = new CastMemberRepository(dbContext);
        var searchInput = new SearchInput(page, perPage, "", "", SearchOrder.Asc);

        // When
        var output = await castMemberRepository.Search(searchInput, CancellationToken.None);

        // Then        
        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.CurrentPage.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(numberOfCastMembersToGenerate);
        output.Items.Should().HaveCount(expectedNumberOfItems);
        foreach (CastMember outputItem in output.Items)
        {
            var exampleItem = exampleCastMembersList.Find(x => x.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();
            outputItem!.Name.Should().Be(exampleItem!.Name);
            outputItem.Type.Should().Be(exampleItem.Type);
            outputItem.CreatedAt.Should().Be(exampleItem.CreatedAt);
        }
    }

    [Theory(DisplayName = nameof(SearchByText))]
    [Trait("Integration/Infra.Data", "CastMemberRepository - Repositories")]
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
        var dbContext = _fixture.CreateDbContext(false, Guid.NewGuid().ToString());
        var exampleCategoriesList = _fixture.GetExampleCastMembersListWithNames(new List<string>(){
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
        await dbContext.AddRangeAsync(exampleCategoriesList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var castMemberRepository = new CastMemberRepository(dbContext);
        var searchInput = new SearchInput(page, perPage, search, "", SearchOrder.Asc);

        // When
        var output = await castMemberRepository.Search(searchInput, CancellationToken.None);

        // Then        
        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.CurrentPage.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(expectedTotalItems);
        output.Items.Should().HaveCount(expectedNumberOfItems);
        foreach (CastMember outputItem in output.Items)
        {
            var exampleItem = exampleCategoriesList.Find(x => x.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();
            outputItem!.Name.Should().Be(exampleItem!.Name);
            outputItem.Type.Should().Be(exampleItem.Type);
            outputItem.CreatedAt.Should().Be(exampleItem.CreatedAt);
        }
    }

    [Theory(DisplayName = nameof(SearchOrdered))]
    [Trait("Integration/Infra.Data", "CastMemberRepository - Repositories")]
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
        var exampleCastMembersList = _fixture.GetExampleCastMembersList();
        await dbContext.AddRangeAsync(exampleCastMembersList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var castMemberRepository = new CastMemberRepository(dbContext);
        var searchOrder = order.ToLower() == "asc" ? SearchOrder.Asc : SearchOrder.Desc;
        var searchInput = new SearchInput(1, 20, "", orderBy, searchOrder);
        var expectedOrderedList = _fixture.CloneCastMembersListOrdered(exampleCastMembersList, orderBy, searchOrder);

        // When
        var output = await castMemberRepository.Search(searchInput, CancellationToken.None);

        // Then
        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.CurrentPage.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(exampleCastMembersList.Count());
        output.Items.Should().HaveCount(exampleCastMembersList.Count);
        for (int index = 0; index < expectedOrderedList.Count(); index++)
        {
            var expecetedItem = expectedOrderedList[index];
            var outputItem = output.Items[index];
            expecetedItem.Should().NotBeNull();
            outputItem.Should().NotBeNull();
            outputItem!.Id.Should().Be(expecetedItem!.Id);
            outputItem.Name.Should().Be(expecetedItem.Name);
            outputItem.Type.Should().Be(expecetedItem.Type);
            outputItem.CreatedAt.Should().Be(expecetedItem.CreatedAt);
        }
    }
}