using Domain.SeedWork.SearchableRepository;
using EndToEndTests.Api.Genre.Common;
using Entities = Domain.Entity;

namespace EndToEndTests.Api.Genre;

[CollectionDefinition(nameof(ListGenresApiTestFixture))]
public class ListGenresApiTestFixtureCollection : ICollectionFixture<ListGenresApiTestFixture> { }

public class ListGenresApiTestFixture : GenreBaseFixture
{
    public List<Entities.Genre> GetExampleGenresListWithNames(List<string> names)
    {
        return names.Select(name =>
        {
            var genre = GetExampleGenre();
            genre.Update(name);
            return genre;
        }).ToList();
    }

    public List<Entities.Genre> CloneGenreListOrdered(List<Entities.Genre> genres, string orderBy, SearchOrder order)
    {
        var listClone = new List<Entities.Genre>(genres);
        var orderedEnumarable = (orderBy.ToLower(), order) switch
        {
            ("name", SearchOrder.Asc) => listClone.OrderBy(x => x.Name),
            ("name", SearchOrder.Desc) => listClone.OrderByDescending(x => x.Name),
            ("id", SearchOrder.Asc) => listClone.OrderBy(x => x.Id),
            ("id", SearchOrder.Desc) => listClone.OrderByDescending(x => x.Id),
            ("createdat", SearchOrder.Asc) => listClone.OrderBy(x => x.CreatedAt),
            ("createdat", SearchOrder.Desc) => listClone.OrderByDescending(x => x.CreatedAt),
            _ => listClone.OrderBy(x => x.Name)
        };

        return orderedEnumarable.ThenBy(x => x.CreatedAt).ToList();
    }
}