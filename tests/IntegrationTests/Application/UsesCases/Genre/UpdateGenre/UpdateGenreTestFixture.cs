using Application.UseCases.Genre.UpdateGenre;
using Entities = Domain.Entity;
namespace IntegrationTest.Application.UseCases.Genre;

[CollectionDefinition(nameof(UpdateGenreTestFixture))]
public class UpdateGenreTestFixtureCollection : ICollectionFixture<UpdateGenreTestFixture> { }

public class UpdateGenreTestFixture : GenreUseCasesBaseFixture
{
    public UpdateGenreInput GetInput(Entities.Genre genre)
        => new(genre.Id, genre.Name, genre.IsActive, genre.Categories.ToList());

    public UpdateGenreInput GetInputWithoutCategories(Entities.Genre genre)
        => new(genre.Id, genre.Name, genre.IsActive);

}