using Application.UseCases.Genre.CreateGenre;

namespace IntegrationTest.Application.UseCases.Genre;

[CollectionDefinition(nameof(CreateGenreTestFixture))]
public class CreateGenreTestFixtureCollection : ICollectionFixture<CreateGenre> { }

public class CreateGenreTestFixture : GenreUseCasesBaseFixture
{

}