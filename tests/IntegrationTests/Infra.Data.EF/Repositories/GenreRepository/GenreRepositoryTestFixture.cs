using IntegrationTest.Base;
using Entities = Domain.Entity;

namespace IntegrationTest.Infra.Data.EF.Repositories;

[CollectionDefinition(nameof(GenreRepositoryTestFixture))]
public class GenreRepositoryTestFixtureCollection : ICollectionFixture<GenreRepositoryTestFixture> { }

public class GenreRepositoryTestFixture : BaseFixture
{
    public string GetValidGenreName()
    {
        return Faker.Commerce.Categories(1)[0];
    }

    public bool GetRandomBoolean()
    {
        var randomNumber = new Random().Next(10);
        return randomNumber % 2 == 0;
    }

    public Entities.Genre GetExampleGenre(bool? isActive = null, List<Guid>? categoryIds = null)
    {
        var genre = new Entities.Genre(GetValidGenreName(), isActive ?? GetRandomBoolean());
        if (categoryIds is not null)
        {
            foreach (var categoryId in categoryIds)
            {
                genre.AddCategory(categoryId);
            }
        }

        return genre;
    }

    public List<Entities.Category> GetExampleCategoriesList(int length = 10)
    {
        var categoryFixture = new CategoryRepositoriesTestFixture();
        return categoryFixture.GetExampleCategoriesList(length);
    }
}