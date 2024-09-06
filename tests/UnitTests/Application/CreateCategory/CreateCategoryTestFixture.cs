using Application.UseCases.Category.CreateCategory;
using Bogus;
using UnitTests.Application.Common;

namespace UnitTests.Application.CreateCategory;

[CollectionDefinition(nameof(CreateCategoryTestFixture))]
public class CreateCategoryTestFixtureCollection : ICollectionFixture<CreateCategoryTestFixture> { }

public class CreateCategoryTestFixture : CategoryBaseFixture
{
    public CreateCategoryTestFixture() : base()
    {
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
}