using Domain.Entity;
using Domain.SeedWork.SearchableRepository;
using IntegrationTest.Base;

namespace IntegrationTest.Infra.Data.EF.Repositories;

[CollectionDefinition(nameof(GenreRepositoryTestFixture))]
public class GenreRepositoryTestFixtureCollection : ICollectionFixture<GenreRepositoryTestFixture> { }

public class GenreRepositoryTestFixture : BaseFixture
{
    public string GetValidGenreName()
    {
        return Faker.Commerce.Categories(1)[0];
    }

    public bool GetRandomBoolean()
    {
        var randomNumber = new Random().Next(10);
        return randomNumber % 2 == 0;
    }

    public Genre GetExampleGenre(bool? isActive = null, List<Guid>? categoryIds = null)
    {
        var genre = new Genre(GetValidGenreName(), isActive ?? GetRandomBoolean());
        if (categoryIds is not null)
        {
            foreach (var categoryId in categoryIds)
            {
                genre.AddCategory(categoryId);
            }
        }

        return genre;
    }

    public List<Category> GetExampleCategoriesList(int length = 10)
    {
        var categoryFixture = new CategoryRepositoriesTestFixture();
        return categoryFixture.GetExampleCategoriesList(length);
    }

    public List<Genre> GetExampleGenresList(int length = 10)
    {
        var list = Enumerable.Range(1, length).Select(_ => GetExampleGenre()).ToList();
        return list;
    }

    public List<Genre> GetExampleGenresListWithNames(List<string> names)
    {
        return names.Select(name =>
        {
            var genre = GetExampleGenre();
            genre.Update(name);
            return genre;
        }).ToList();
    }

    public List<Genre> CloneGenreListOrdered(List<Genre> genres, string orderBy, SearchOrder order)
    {
        var listClone = new List<Genre>(genres);
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