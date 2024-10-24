using Application.UseCases.Category.UpdateCategory;
using UnitTests.Application.Common;

namespace UnitTests.Application.Category;

[CollectionDefinition(nameof(UpdateCategoryTestFixture))]
public class UpdateCategoryTestFixtureCollection : ICollectionFixture<UpdateCategoryTestFixture> { }

public class UpdateCategoryTestFixture : CategoryBaseFixture
{
    public UpdateCategoryInput GetValidInput(Guid? id = null)
    {
        return new(
            id ?? Guid.NewGuid(),
            GetValidCategoryName(),
            GetValidCategoryDescription(),
            GetRandomBoolean()
        );
    }

    public UpdateCategoryInput GetInvalidInputShortName()
    {
        var invalidInputShortName = GetValidInput();
        invalidInputShortName.Name = invalidInputShortName.Name.Substring(0, 2);
        return invalidInputShortName;
    }

    public UpdateCategoryInput GetInvalidInputLongName()
    {
        var invalidInputTooLongName = GetValidInput();
        while (invalidInputTooLongName.Name.Length <= 255)
            invalidInputTooLongName.Name += " " + Faker.Commerce.ProductName();

        return invalidInputTooLongName;
    }

    public UpdateCategoryInput GetInvalidInputLongDescription()
    {
        var invalidInputTooLongDescription = GetValidInput();
        while (invalidInputTooLongDescription.Description?.Length <= 10_000)
            invalidInputTooLongDescription.Description += " " + Faker.Commerce.ProductDescription();

        return invalidInputTooLongDescription;
    }
}