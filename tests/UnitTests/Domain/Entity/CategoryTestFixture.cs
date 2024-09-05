using Domain.Entity;

namespace UnitTests.Domain.Entity;

public class CategoryTestFixture : BaseFixture
{
    public CategoryTestFixture() : base()
    {
    }

    private string GetValidCategoryName()
    {
        var categoryName = "";
        while (categoryName.Length < 3)
            categoryName = Faker.Commerce.Categories(1)[0];

        if (categoryName.Length > 255)
            categoryName = categoryName[..255];

        return categoryName;
    }

    private string GetValidCategoryDescription()
    {
        var description = Faker.Commerce.ProductDescription();
        if (description.Length > 10_000)
            description = description[..10_000];

        return description;
    }

    public Category GetValidCategory() => new Category(GetValidCategoryName(), GetValidCategoryDescription());
}

[CollectionDefinition(nameof(CategoryTestFixture))]
public class CategoryTestFixtureCollection : ICollectionFixture<CategoryTestFixture> { }