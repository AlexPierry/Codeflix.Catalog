using Application.UseCases.Genre.CreateGenre;
using UnitTests.Application.Genre.Common;

namespace UnitTests.Application.Genre;

[CollectionDefinition(nameof(CreateGenreTestFixture))]
public class CreateGenreTestFixtureCollection : ICollectionFixture<CreateGenreTestFixture> { }

public class CreateGenreTestFixture : GenreUseCasesBaseFixture
{
    public CreateGenreInput GetExampleInput() => new(GetValidGenreName(), GetRandomBoolean());

    public CreateGenreInput GetExampleInputWithCategories()
    {
        var numberOfCategories = new Random().Next(1, 10);
        var categoryIds = Enumerable.Range(1, numberOfCategories).Select(_ => Guid.NewGuid());
        var genre = new CreateGenreInput(GetValidGenreName(), GetRandomBoolean(), categoryIds.ToList());

        return genre;
    }
}