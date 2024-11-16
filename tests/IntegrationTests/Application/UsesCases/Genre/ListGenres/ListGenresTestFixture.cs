using Domain.SeedWork.SearchableRepository;
using Entities = Domain.Entity;
namespace IntegrationTest.Application.UseCases.Genre;

[CollectionDefinition(nameof(ListGenresTestFixture))]
public class ListGenresTestFixtureCollection : ICollectionFixture<ListGenresTestFixture> { }

public class ListGenresTestFixture : GenreUseCasesBaseFixture
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

        return orderedEnumarable.ToList();
    }
}