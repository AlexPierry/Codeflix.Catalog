using Application.UseCases.Genre.CreateGenre;

namespace IntegrationTest.Application.UseCases.Genre;

[CollectionDefinition(nameof(CreateGenreTestFixture))]
public class CreateGenreTestFixtureCollection : ICollectionFixture<CreateGenreTestFixture> { }

public class CreateGenreTestFixture : GenreUseCasesBaseFixture
{
    public CreateGenreInput GetInput(List<Guid>? categoriesIds = null) => new(GetValidGenreName(), GetRandomBoolean(), categoriesIds);

    public CreateGenreInput GetInvalidInputShortName()
    {
        var invalidInputShortName = GetInput();
        invalidInputShortName.Name = invalidInputShortName.Name.Substring(0, 2);
        return invalidInputShortName;
    }

    public CreateGenreInput GetInvalidInputLongName()
    {
        var invalidInputTooLongName = GetInput();
        while (invalidInputTooLongName.Name.Length <= 255)
            invalidInputTooLongName.Name += " " + Faker.Commerce.ProductName();

        return invalidInputTooLongName;
    }

    public CreateGenreInput GetInvalidInputNameIsNull()
    {
        var invalidInputNameIsNull = GetInput();
        invalidInputNameIsNull.Name = null!;
        return invalidInputNameIsNull;
    }
}