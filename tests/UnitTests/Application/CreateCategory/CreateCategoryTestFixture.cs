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

    public CreateCategoryInput GetInput() => new(GetValidCategoryName(), GetValidCategoryDescription(), GetRandomBoolean());

    public CreateCategoryInput GetInvalidInputShortName()
    {
        var invalidInputShortName = GetInput();
        invalidInputShortName.Name = invalidInputShortName.Name.Substring(0, 2);
        return invalidInputShortName;
    }

    public CreateCategoryInput GetInvalidInputLongName()
    {
        var invalidInputTooLongName = GetInput();
        while (invalidInputTooLongName.Name.Length <= 255)
            invalidInputTooLongName.Name += " " + Faker.Commerce.ProductName();

        return invalidInputTooLongName;
    }

    public CreateCategoryInput GetInvalidInputNameIsNull()
    {
        var invalidInputNameIsNull = GetInput();
        invalidInputNameIsNull.Name = null!;
        return invalidInputNameIsNull;
    }

    public CreateCategoryInput GetInvalidInputDescriptionIsNull()
    {
        var invalidInputDescriptionIsNull = GetInput();
        invalidInputDescriptionIsNull.Description = null!;
        return invalidInputDescriptionIsNull;
    }

    public CreateCategoryInput GetInvalidInputLongDescription()
    {
        var invalidInputTooLongDescription = GetInput();
        while (invalidInputTooLongDescription.Description.Length <= 10_000)
            invalidInputTooLongDescription.Description += " " + Faker.Commerce.ProductDescription();

        return invalidInputTooLongDescription;
    }

    public Mock<ICategoryRepository> GetRepositoryMock() => new();
    public Mock<IUnitOfWork> GetUnitOfWorkMock() => new();

}