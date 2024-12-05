using Application.UseCases.Genre.CreateGenre;
using EndToEndTests.Api.Genre.Common;

namespace EndToEndTests.Api.Genre;

[CollectionDefinition(nameof(CreateGenreApiTestFixture))]
public class CreateGenreApiTestFixtureCollection : ICollectionFixture<CreateGenreApiTestFixture> { }

public class CreateGenreApiTestFixture : GenreBaseFixture
{
    public CreateGenreInput GetExampleInput()
    {
        return new(
            GetValidName(),
            GetRandomBoolean()
        );
    }

    public CreateGenreInput GetExampleInputWithCategories(List<Guid> categoriesIds)
    {
        return new(
            GetValidName(),
            GetRandomBoolean(),
            categoriesIds
        );
    }
}