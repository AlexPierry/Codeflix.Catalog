using EndToEndTests.Base;
using Entities = Domain.Entity;

namespace EndToEndTests.Api.Category.Common;

public class CreateCategoryBaseFixture : BaseFixture
{
    public CategoryPersistence Persistence { get; private set; }

    public CreateCategoryBaseFixture() : base()
    {
        Persistence = new CategoryPersistence(CreateDbContext());
    }

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


    public string GetInvalidNameTooShort()
    {
        var invalidNameTooShort = GetValidCategoryName();
        invalidNameTooShort = invalidNameTooShort.Substring(0, 2);
        return invalidNameTooShort;
    }

    public string GetInvalidNameTooLong()
    {
        var invalidNameTooLong = GetValidCategoryName();
        while (invalidNameTooLong.Length <= 255)
            invalidNameTooLong += " " + Faker.Commerce.ProductName();

        return invalidNameTooLong;
    }

    public string GetInvalidDescriptionTooLong()
    {
        var invalidDescriptionTooLong = GetValidCategoryDescription();
        while (invalidDescriptionTooLong.Length <= 10_000)
            invalidDescriptionTooLong += " " + Faker.Commerce.ProductDescription();

        return invalidDescriptionTooLong;
    }

    public Entities.Category GetExampleCategory() => new Entities.Category(GetValidCategoryName(), GetValidCategoryDescription(), GetRandomBoolean());

    public List<Entities.Category> GetExampleCategoriesList(int length = 10)
    {
        return Enumerable.Range(1, length).Select(_ => GetExampleCategory()).ToList();
    }
}