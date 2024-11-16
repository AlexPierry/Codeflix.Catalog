using Application.UseCases.Genre.DeleteGenre;
namespace IntegrationTest.Application.UseCases.Genre;

[CollectionDefinition(nameof(DeleteGenreTestFixture))]
public class DeleteGenreTestFixtureCollection : ICollectionFixture<DeleteGenreTestFixture> { }

public class DeleteGenreTestFixture : GenreUseCasesBaseFixture
{
    public DeleteGenreInput GetInput(Guid genreId)
        => new(genreId);
}