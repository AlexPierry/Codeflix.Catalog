using Domain.SeedWork.SearchableRepository;
using EndToEndTests.Api.Category.Common;
using Entities = Domain.Entity;

namespace EndToEndTests.Api.Category.ListCategories;

[CollectionDefinition(nameof(ListCategoriesApiTestFixture))]
public class ListCategoriesApiTestFixtureCollection : ICollectionFixture<ListCategoriesApiTestFixture> { }

public class ListCategoriesApiTestFixture : CategoryBaseFixture
{

    public List<Entities.Category> GetExampleCategoriesListWithNames(List<string> names)
    {
        return names.Select(name =>
        {
            var category = GetExampleCategory();
            category.Update(name);
            return category;
        }).ToList();
    }

    public List<Entities.Category> CloneCategoryListOrdered(List<Entities.Category> categories, string orderBy, SearchOrder order)
    {
        var listClone = new List<Entities.Category>(categories);
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