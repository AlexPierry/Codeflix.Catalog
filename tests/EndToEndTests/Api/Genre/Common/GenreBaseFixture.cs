using EndToEndTests.Api.Category.Common;
using EndToEndTests.Base;
using Entities = Domain.Entity;

namespace EndToEndTests.Api.Genre.Common;

public class GenreBaseFixture : BaseFixture
{
    public GenrePersistence Persistence { get; private set; }
    public CategoryPersistence CategoryPersistence { get; private set; }

    public GenresCategoriesPersistence GenresCategoriesPersistence { get; private set; }

    public GenreBaseFixture() : base()
    {
        var dbContext = CreateDbContext();
        Persistence = new GenrePersistence(dbContext);
        CategoryPersistence = new CategoryPersistence(dbContext);
        GenresCategoriesPersistence = new GenresCategoriesPersistence(dbContext);
    }

    public string GetValidName()
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

    public string GetValidCategoryDescription()
    {
        var description = Faker.Commerce.ProductDescription();
        if (description.Length > 10_000)
            description = description[..10_000];

        return description;
    }

    public Entities.Genre GetExampleGenre() => new Entities.Genre(GetValidName(), GetRandomBoolean());

    public List<Entities.Genre> GetExampleGenresList(int length = 10)
    {
        return Enumerable.Range(1, length).Select(_ => GetExampleGenre()).ToList();
    }

    public Entities.Category GetExampleCategory() => new Entities.Category(GetValidName(), GetValidCategoryDescription(), GetRandomBoolean());

    public List<Entities.Category> GetExampleCategoriesList(int length = 10)
    {
        return Enumerable.Range(1, length).Select(_ => GetExampleCategory()).ToList();
    }
}