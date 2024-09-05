using Application.Interfaces;
using Application.UseCases.Category.CreateCategory;
using Domain.Repository;
using Moq;

namespace UnitTests.Application.CreateCategory;

[CollectionDefinition(nameof(CreateCategoryTestFixture))]
public class CreateCategoryTestFixtureCollection : ICollectionFixture<CreateCategoryTestFixture> { }

public class CreateCategoryTestFixture : BaseFixture
{

    public CreateCategoryTestFixture() : base()
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

    public bool GetRandomBoolean()
    {
        return new Random().NextDouble() < 0.5;
    }

    public CreateCategoryInput GetInput() => new(GetValidCategoryName(), GetValidCategoryDescription(), GetRandomBoolean());

    public Mock<ICategoryRepository> GetRepositoryMock() => new();
    public Mock<IUnitOfWork> GetUnitOfWorkMock() => new();

}