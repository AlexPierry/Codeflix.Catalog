using EndToEndTests.Api.Genre.Common;

namespace EndToEndTests.Api.Genre;

[CollectionDefinition(nameof(GetGenreApiTestFixture))]
public class GetGenreApiTestFixtureCollection : ICollectionFixture<GetGenreApiTestFixture> { }

public class GetGenreApiTestFixture : GenreBaseFixture
{

}