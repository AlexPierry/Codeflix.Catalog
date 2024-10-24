using Application.UseCases.Category.ListCategories;
using Domain.SeedWork.SearchableRepository;
using UnitTests.Application.Common;
using Entities = Domain.Entity;

namespace UnitTests.Application.Category;

[CollectionDefinition(nameof(ListCategoriesTestFixture))]
public class ListCategoryTestFixtureCollection : ICollectionFixture<ListCategoriesTestFixture> { }

public class ListCategoriesTestFixture : CategoryBaseFixture
{
    public List<Entities.Category> GetExampleCategoriesList(int length = 10)
    {
        var list = new List<Entities.Category>();
        for (int index = 0; index < length; index++)
        {
            list.Add(GetExampleCategory());
        }

        return list;
    }

    public ListCategoriesInput GetExampleInput()
    {
        var random = new Random();
        return new ListCategoriesInput(
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
