using EndToEndTests.Api.Genre.Common;

namespace EndToEndTests.Api.Genre;

[CollectionDefinition(nameof(CreateGenreApiTestFixture))]
public class CreateGenreApiTestFixtureCollection : ICollectionFixture<CreateGenreApiTestFixture> { }

public class CreateGenreApiTestFixture : GenreBaseFixture
{

}