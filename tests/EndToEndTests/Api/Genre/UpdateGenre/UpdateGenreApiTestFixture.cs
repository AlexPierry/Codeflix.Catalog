using Application.UseCases.Genre.UpdateGenre;
using EndToEndTests.Api.Genre.Common;

namespace EndToEndTests.Api.Genre;

[CollectionDefinition(nameof(UpdateGenreApiTestFixture))]
public class UpdateGenreApiTestFixtureCollection : ICollectionFixture<UpdateGenreApiTestFixture> { }

public class UpdateGenreApiTestFixture : GenreBaseFixture
{
    public UpdateGenreInput GetExampleInput(Guid genreId, bool isActive, List<Guid>? categoriesIds = null)
    {
        return new(
            genreId,
            GetValidName(),
            !isActive,
            categoriesIds
        );
    }
}