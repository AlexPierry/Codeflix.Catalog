using System.Runtime.ConstrainedExecution;
using Application.UseCases.Video.ListVideos;
using Domain.Enum;
using Domain.SeedWork.SearchableRepository;
using UnitTests.Common.Fixtures;
using Entities = Domain.Entity;

namespace UnitTests.Application.Video;
[CollectionDefinition(nameof(ListVideosTestFixture))]
public class ListVideosTestFixtureCollection : ICollectionFixture<ListVideosTestFixture>
{
}

public class ListVideosTestFixture : VideoTestFixtureBase
{
    internal (
        List<Entities.Video> videos,
        List<Entities.Category> categories,
        List<Entities.Genre> genres,
        List<Entities.CastMember> castMembers)
        GetValidVideoListAndAggregates(int numberOfIOtems = 5)
    {
        var videos = Enumerable.Range(1, numberOfIOtems).Select(x => GetValidVideoWithAllProperties()).ToList();
        var categories = new List<Entities.Category>();
        var genres = new List<Entities.Genre>();
        var castMembers = new List<Entities.CastMember>();

        foreach (var video in videos)
        {
            video.RemoveAllCategories();
            var qtdCategories = new Random().Next(1, 5);
            for (var i = 0; i < qtdCategories; i++)
            {
                var category = GetExampleCategory();
                categories.Add(category);
                video.AddCategory(category.Id);
            }

            video.RemoveAllGenres();
            var qtdGenres = new Random().Next(1, 5);
            for (var i = 0; i < qtdGenres; i++)
            {
                var genre = GetExampleGenre();
                genres.Add(genre);
                video.AddGenre(genre.Id);
            }

            video.RemoveAllCastMembers();
            var qtdCastMembers = new Random().Next(1, 5);
            for (var i = 0; i < qtdCastMembers; i++)
            {
                var castMember = GetExampleCastMember();
                castMembers.Add(castMember);
                video.AddCastMember(castMember.Id);
            }
        }

        return (videos, categories, genres, castMembers);
    }

    public ListVideosInput GetExampleInput()
    {
        var random = new Random();
        return new ListVideosInput(
            page: random.Next(1, 10),
            perPage: random.Next(15, 100),
            search: Faker.Commerce.ProductName(),
            sort: Faker.Commerce.ProductName(),
            dir: random.Next(0, 10) > 5
                ? SearchOrder.Asc
                : SearchOrder.Desc
        );
    }

    public Entities.Category GetExampleCategory()
        => new Entities.Category(GetValidCategoryName(), GetValidCategoryDescription(), GetRandomBoolean());

    private string GetValidCategoryName()
    {
        var categoryName = "";
        while (categoryName.Length < 3)
            categoryName = Faker.Commerce.Categories(1)[0];

        if (categoryName.Length > 255)
            categoryName = categoryName[..255];

        return categoryName;
    }

    private string GetValidCategoryDescription()
    {
        var description = Faker.Commerce.ProductDescription();
        if (description.Length > 10_000)
            description = description[..10_000];

        return description;
    }

    private bool GetRandomBoolean()
    {
        return new Random().NextDouble() < 0.5;
    }

    private string GetValidGenreName()
    {
        return Faker.Commerce.Categories(1)[0];
    }


    public Entities.Genre GetExampleGenre(bool? isActive = null)
    {
        return new Entities.Genre(GetValidGenreName(), isActive ?? GetRandomBoolean());
    }

    private string GetValidName()
    {
        return Faker.Name.FullName();
    }

    private CastMemberType GetRandomCastMemberType()
    {
        return (CastMemberType)new Random().Next(1, 2);
    }

    public Entities.CastMember GetExampleCastMember()
        => new Entities.CastMember(GetValidName(), GetRandomCastMemberType());

    internal List<Entities.Video> GetValidVideoListWithoutRelations(int numberOfItems)
    {
        return Enumerable.Range(1, numberOfItems).Select(x => GetValidVideo()).ToList();
    }
}
