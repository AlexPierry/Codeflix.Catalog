using Application.UseCases.Genre.UpdateGenre;
using UnitTests.Application.Genre.Common;

namespace UnitTests.Application.Genre;

[CollectionDefinition(nameof(UpdateGenreTestFixture))]
public class UpdateGenreTestFixtureCollection : ICollectionFixture<UpdateGenreTestFixture> { }

public class UpdateGenreTestFixture : GenreUseCasesBaseFixture
{
    public UpdateGenreInput GetExampleInput(Guid id) => new(id, GetValidGenreName(), GetRandomBoolean());

    public UpdateGenreInput GetExampleInputWithCategories(Guid id)
    {
        var numberOfCategories = new Random().Next(1, 10);
        var categoryIds = Enumerable.Range(1, numberOfCategories).Select(_ => Guid.NewGuid());
        var genre = new UpdateGenreInput(id, GetValidGenreName(), GetRandomBoolean(), categoryIds.ToList());

        return genre;
    }

    internal UpdateGenreInput GetExampleInputWithEmptyCategories(Guid id)
    {
        return new UpdateGenreInput(id, GetValidGenreName(), GetRandomBoolean(), new List<Guid>());

    }
}