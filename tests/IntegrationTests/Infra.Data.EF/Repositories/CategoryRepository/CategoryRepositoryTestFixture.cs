using Domain.Entity;
using Infra.Data.EF;
using IntegrationTest.Base;
using Microsoft.EntityFrameworkCore;

namespace IntegrationTest.Infra.Data.EF.Repositories.CategoryRepositories;

[CollectionDefinition(nameof(CategoryRepositoriesTestFixture))]
public class CategoryRepositoryTestFixtureCollection : ICollectionFixture<CategoryRepositoriesTestFixture> { }

public class CategoryRepositoriesTestFixture : BaseFixture
{
    public string GetValidCategoryName()
    {
        var categoryName = "";
        while (categoryName.Length < 3)
            categoryName = Faker.Commerce.Categories(1)[0];

        if (categoryName.Length > 255)
            categoryName = categoryName[..255];

        return categoryName;
    }

    public string GetValidCategoryDescription()
    {
        var description = Faker.Commerce.ProductDescription();
        if (description.Length > 10_000)
            description = description[..10_000];

        return description;
    }

    public bool GetRandomBoolean()
    {
        return new Random().NextDouble() < 0.5;
    }

    public Category GetExampleCategory() => new Category(GetValidCategoryName(), GetValidCategoryDescription(), GetRandomBoolean());

    public List<Category> GetExampleCategoriesList(int length = 10)
    {
        return Enumerable.Range(1, length).Select(_ => GetExampleCategory()).ToList();
    }

    public CodeflixCatalogDbContext CreateDbContext()
    {
        return new CodeflixCatalogDbContext(
            new DbContextOptionsBuilder<CodeflixCatalogDbContext>()
            .UseInMemoryDatabase("integration-tests-db")
            .Options
        );
    }
}