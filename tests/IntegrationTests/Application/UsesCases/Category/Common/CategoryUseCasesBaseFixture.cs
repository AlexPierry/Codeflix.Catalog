using IntegrationTest.Base;
using Entities = Domain.Entity;
namespace IntegrationTest.Application.UseCases.Category.Common;

public class CategoryUseCasesBaseFixture : BaseFixture
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

    public Entities.Category GetExampleCategory() => new Entities.Category(GetValidCategoryName(), GetValidCategoryDescription(), GetRandomBoolean());

    public List<Entities.Category> GetExampleCategoriesList(int length = 10)
    {
        return Enumerable.Range(1, length).Select(_ => GetExampleCategory()).ToList();
    }
    
}