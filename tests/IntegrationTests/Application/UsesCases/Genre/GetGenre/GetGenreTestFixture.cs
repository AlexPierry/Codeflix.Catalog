using IntegrationTest.Infra.Data.EF.Repositories;
using Entities = Domain.Entity;
namespace IntegrationTest.Application.UseCases.Genre;

[CollectionDefinition(nameof(GetGenreTestFixture))]
public class GetGenreTestFixtureCollection : ICollectionFixture<GetGenreTestFixture>
{

}

public class GetGenreTestFixture : GenreUseCasesBaseFixture
{
}