using UnitTests.Application.Genre.Common;

namespace UnitTests.Application.Genre;

[CollectionDefinition(nameof(DeleteGenreTestFixture))]
public class DeleteGenreTestFixtureCollection : ICollectionFixture<DeleteGenreTestFixture> { }

public class DeleteGenreTestFixture : GenreUseCasesBaseFixture
{

}
