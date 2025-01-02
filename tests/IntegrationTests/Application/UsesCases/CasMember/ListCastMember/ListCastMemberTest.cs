using Application.UseCases.CastMember;
using Domain.SeedWork.SearchableRepository;
using FluentAssertions;
using Infra.Data.EF.Repositories;
using IntegrationTest.Application.UseCases.CastMember.Common;

namespace IntegrationTest.Application.UseCases.CastMember;

[Collection(nameof(CastMemberUseCaseBaseFixture))]
public class ListCastMemberTest
{
    private readonly CastMemberUseCaseBaseFixture _fixture;

    public ListCastMemberTest(CastMemberUseCaseBaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(SearchReturnsListAndTotal))]
    [Trait("Integration/Application", "ListCastMembers - Use Cases")]
    public async void SearchReturnsListAndTotal()
    {
        // Given
        var dbContext = _fixture.CreateDbContext();
        var repository = new CastMemberRepository(dbContext);
        var castMemberExampleList = _fixture.GetExampleCastMembersList(10);
        await dbContext.AddRangeAsync(castMemberExampleList);
        await dbContext.SaveChangesAsync();

        var input = new ListCastMembersInput(1, 20);

        var useCase = new ListCastMembers(repository);

        // When
        var output = await useCase.Handle(input, CancellationToken.None);

        // Then
        output.Should().NotBeNull();
        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Total.Should().Be(castMemberExampleList.Count);
        output.Items.Should().HaveCount(castMemberExampleList.Count);
        foreach (var outputItem in output.Items)
        {
            var repositoryCastMember = castMemberExampleList.First(x => x.Id == outputItem.Id);
            outputItem.Should().NotBeNull();
            outputItem.Id.Should().Be(repositoryCastMember.Id);
            outputItem.Name.Should().Be(repositoryCastMember.Name);
            outputItem.Type.Should().Be(repositoryCastMember.Type);
            outputItem.CreatedAt.Should().Be(repositoryCastMember.CreatedAt);
        };
    }

    [Fact(DisplayName = nameof(SearchReturnsEmptyWhenEmpty))]
    [Trait("Integration/Application", "ListCastMembers - Use Cases")]
    public async void SearchReturnsEmptyWhenEmpty()
    {
        // Given
        var dbContext = _fixture.CreateDbContext();
        var repository = new CastMemberRepository(dbContext);
        var castMembersExampleList = _fixture.GetExampleCastMembersList(0);
        await dbContext.AddRangeAsync(castMembersExampleList);
        await dbContext.SaveChangesAsync();

        var input = new ListCastMembersInput(1, 20);

        var useCase = new ListCastMembers(repository);

        // When
        var output = await useCase.Handle(input, CancellationToken.None);

        // Then
        output.Should().NotBeNull();
        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Total.Should().Be(0);
        output.Items.Should().HaveCount(0);
    }

    [Theory(DisplayName = nameof(SearchReturnsPaginated))]
    [Trait("Integration/Application", "ListCastMembers - Use Cases")]
    [InlineData(10, 1, 5, 5)]
    [InlineData(10, 2, 5, 5)]
    [InlineData(7, 2, 5, 2)]
    [InlineData(7, 3, 5, 0)]
    public async Task SearchReturnsPaginated(int numberOfCastMembersToGenerate, int page, int perPage, int expectedNumberOfItems)
    {
        // Given
        var dbContext = _fixture.CreateDbContext();
        var exampleCategoriesList = _fixture.GetExampleCastMembersList(numberOfCastMembersToGenerate);
        await dbContext.AddRangeAsync(exampleCategoriesList);
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
        output.Total.Should().Be(exampleCategoriesList.Count);
        output.Items.Should().HaveCount(expectedNumberOfItems);
        foreach (var outputItem in output.Items)
        {
            var exampleItem = exampleCategoriesList.Find(x => x.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();
            outputItem!.Name.Should().Be(exampleItem!.Name);
            outputItem.Type.Should().Be(exampleItem.Type);
            outputItem.CreatedAt.Should().Be(exampleItem.CreatedAt);
        }
    }

    [Theory(DisplayName = nameof(SearchByText))]
    [Trait("Integration/Application", "ListCastMembers - Use Cases")]
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
        var input = new ListCastMembersInput(page, perPage, search, "", SearchOrder.Asc);
        var useCase = new ListCastMembers(castMemberRepository);

        // When
        var output = await useCase.Handle(input, CancellationToken.None);

        // Then        
        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Total.Should().Be(expectedTotalItems);
        output.Items.Should().HaveCount(expectedNumberOfItems);
        foreach (var outputItem in output.Items)
        {
            var exampleItem = exampleCategoriesList.Find(x => x.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();
            outputItem!.Name.Should().Be(exampleItem!.Name);
            outputItem.Type.Should().Be(exampleItem.Type);
            outputItem.CreatedAt.Should().Be(exampleItem.CreatedAt);
        }
    }

    [Theory(DisplayName = nameof(SearchOrdered))]
    [Trait("Integration/Application", "ListCastMembers - Use Cases")]
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
        var input = new ListCastMembersInput(1, 20, "", orderBy, searchOrder);
        var expectedOrderedList = _fixture.CloneCastMembersListOrdered(exampleCastMembersList, orderBy, searchOrder);
        var useCase = new ListCastMembers(castMemberRepository);

        // When
        var output = await useCase.Handle(input, CancellationToken.None);

        // Then
        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
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