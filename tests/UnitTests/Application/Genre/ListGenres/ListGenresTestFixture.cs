using Application.UseCases.Genre.ListGenres;
using Domain.SeedWork.SearchableRepository;
using UnitTests.Application.Genre.Common;
using Entities = Domain.Entity;

namespace UnitTests.Application.Genre;

[CollectionDefinition(nameof(ListGenresTestFixture))]
public class ListGenresTestFixtureCollection : ICollectionFixture<ListGenresTestFixture>
{

}

public class ListGenresTestFixture : GenreUseCasesBaseFixture
{

    public List<Entities.Genre> GetExampleGenresList(int length = 10)
    {
        var list = new List<Entities.Genre>();
        for (int index = 0; index < length; index++)
        {
            list.Add(
                GetExampleGenre(
                    GetRandomBoolean(),
                    Enumerable.Range(0, 5).Select(_ => Guid.NewGuid()).ToList()));
        }

        return list;
    }

    public ListGenresInput GetExampleInput()
    {
        var random = new Random();
        return new ListGenresInput(
            page: random.Next(1, 10),
            perPage: random.Next(15, 100),
            search: Faker.Commerce.ProductName(),
            sort: Faker.Commerce.ProductName(),
            dir: random.Next(0, 10) > 5
                ? SearchOrder.Asc
                : SearchOrder.Desc
        );
    }
}