using Application.UseCases.Genre.GetGenre;
using UnitTests.Application.Genre.Common;

namespace UnitTests.Application.Genre;

[CollectionDefinition(nameof(GetGenreTestFixture))]
public class GetGenreTestFixtureCollection : ICollectionFixture<GetGenreTestFixture> { }

public class GetGenreTestFixture : GenreUseCasesBaseFixture
{

    public GetGenreInput GetExampleInput(Guid id)
    {
        return new GetGenreInput(id);
    }
}