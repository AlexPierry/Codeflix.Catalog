using Application.Interfaces;
using Entities = Domain.Entity;
using Domain.Repository;
using Moq;

namespace UnitTests.Application.Common;
public abstract class CategoryBaseFixture : BaseFixture
{
    public Mock<ICategoryRepository> GetRepositoryMock() => new();
    public Mock<IUnitOfWork> GetUnitOfWorkMock() => new();

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
}