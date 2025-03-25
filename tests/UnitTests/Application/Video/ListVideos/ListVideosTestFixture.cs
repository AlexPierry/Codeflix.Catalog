using Application.UseCases.Video.ListVideos;
using Domain.SeedWork.SearchableRepository;
using UnitTests.Common.Fixtures;
using Entities = Domain.Entity;

namespace UnitTests.Application.Video;
[CollectionDefinition(nameof(ListVideosTestFixture))]
public class ListVideosTestFixtureCollection : ICollectionFixture<ListVideosTestFixture>
{
}

public class ListVideosTestFixture : VideoTestFixtureBase
{
    internal List<Entities.Video> GetValidVideoList(int numberOfIOtems = 5)
    {
        return Enumerable.Range(1, numberOfIOtems).Select(x => GetValidVideoWithAllProperties()).ToList();
    }

    public ListVideosInput GetExampleInput()
    {
        var random = new Random();
        return new ListVideosInput(
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
