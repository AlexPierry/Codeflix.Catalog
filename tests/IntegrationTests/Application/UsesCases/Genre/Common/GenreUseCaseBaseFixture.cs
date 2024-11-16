using IntegrationTest.Base;
using IntegrationTest.Infra.Data.EF.Repositories;
using Entities = Domain.Entity;

namespace IntegrationTest.Application.UseCases.Genre;

public class GenreUseCasesBaseFixture : BaseFixture
{
    public string GetValidGenreName()
    {
        var genreName = "";
        while (genreName.Length < 3)
            genreName = Faker.Commerce.Categories(1)[0];

        if (genreName.Length > 255)
            genreName = genreName[..255];

        return genreName;
    }

    public bool GetRandomBoolean()
    {
        return new Random().NextDouble() < 0.5;
    }

    public Entities.Genre GetExampleGenre() => new Entities.Genre(GetValidGenreName(), GetRandomBoolean());

    public List<Entities.Genre> GetExampleGenresList(int length = 10)
    {
        return Enumerable.Range(1, length).Select(_ => GetExampleGenre()).ToList();
    }

    public List<Entities.Category> GetExampleCategoriesList(int length = 10)
    {
        var categoryFixture = new CategoryRepositoriesTestFixture();
        return categoryFixture.GetExampleCategoriesList(length);
    }    
}