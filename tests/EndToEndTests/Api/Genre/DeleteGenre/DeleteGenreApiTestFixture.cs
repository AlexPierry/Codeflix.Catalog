using EndToEndTests.Api.Genre.Common;

namespace EndToEndTests.Api.Genre;

[CollectionDefinition(nameof(DeleteGenreApiTestFixture))]
public class DeleteGenreApiTestFixtureCollection : ICollectionFixture<DeleteGenreApiTestFixture> { }

public class DeleteGenreApiTestFixture : GenreBaseFixture
{

}