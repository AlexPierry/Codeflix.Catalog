using Domain.Entity;
using Domain.Enum;
using Domain.SeedWork.SearchableRepository;
using IntegrationTest.Base;

namespace IntegrationTests.Infra.Data.EF.Repositories;

[CollectionDefinition(nameof(VideoRepositoryTestFixture))]
public class VideoRepositoryTestFixtureCollection : ICollectionFixture<VideoRepositoryTestFixture>
{
}

public class VideoRepositoryTestFixture : BaseFixture
{
    internal string GetValidDescription()
    {
        return Faker.Commerce.ProductDescription();
    }

    internal int GetValidDuration()
    {
        return Faker.Random.Int(1, 300);
    }

    internal bool GetValidOpened()
    {
        return GetRandomBool();
    }

    internal bool GetValidPublished()
    {
        return GetRandomBool();
    }

    internal string GetValidTitle()
    {
        return Faker.Lorem.Letter(100);
    }

    internal int GetValidYear()
    {
        return Faker.Random.Int(1900, 2024);
    }

    internal bool GetRandomBool()
    {
        return Faker.Random.Bool();
    }

    internal Video GetValidVideo()
    {
        var title = GetValidTitle();
        var description = GetValidDescription();
        var opened = GetValidOpened();
        var published = GetValidPublished();
        var year = GetValidYear();
        var duration = GetValidDuration();
        var rating = GetRandomMovieRating();

        return new Video(title, description, opened, published, year, duration, rating);
    }

    internal List<Video> GetVideoList(int count)
    {
        return Enumerable.Range(1, count).Select(_ => GetValidVideo()).ToList();
    }

    internal Video GetValidVideoWithAllProperties()
    {
        var video = GetValidVideo();
        video.UpdateBanner(GetValidImagePath());
        video.UpdateThumb(GetValidImagePath());
        video.UpdateThumbHalf(GetValidImagePath());
        video.UpdateTrailer(GetValidMediaPath());
        video.UpdateMedia(GetValidMediaPath());
        video.UpdateAsEncoded(GetValidMediaPath());

        var random = new Random();
        Enumerable.Range(1, random.Next(2, 5)).ToList().ForEach(_ => video.AddCategory(Guid.NewGuid()));
        Enumerable.Range(1, random.Next(2, 5)).ToList().ForEach(_ => video.AddGenre(Guid.NewGuid()));
        Enumerable.Range(1, random.Next(2, 5)).ToList().ForEach(_ => video.AddCastMember(Guid.NewGuid()));

        return video;
    }

    internal MovieRating GetRandomMovieRating()
    {
        return (MovieRating)Faker.Random.Int(0, Enum.GetValues<MovieRating>().Length - 1);
    }

    internal string GetValidImagePath()
    {
        return Faker.Image.PlaceImgUrl();
    }

    internal string GetValidMediaPath()
    {
        var exampleMedias = new string[]{
            "https://www.youtube.com/file-example.mp4",
            "https://www.vimeo.com/file-example4.mp4",
            "https://www.netflix.com/video.mp4",
            "https://www.primevideo.com/example.mp4",
            "https://www.disneyplus.com/file.mp4"
        };

        return exampleMedias[Faker.Random.Int(0, exampleMedias.Length - 1)];
    }

    public string GetValidCastMemberName()
    {
        return Faker.Name.FullName();
    }

    public CastMemberType GetRandomCastMemberType()
    {
        return (CastMemberType)new Random().Next(1, 2);
    }

    public CastMember GetValidCastMember()
    {
        return new CastMember(GetValidCastMemberName(), GetRandomCastMemberType());
    }

    public List<CastMember> GetRandomCastMemberList()
    {
        return Enumerable.Range(1, Random.Shared.Next(1, 5)).Select(_ => GetValidCastMember()).ToList();
    }

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

    public Category GetValidCategory() => new Category(GetValidCategoryName(), GetValidCategoryDescription());

    public List<Category> GetRandomCategoryList()
    {
        return Enumerable.Range(1, Random.Shared.Next(1, 5)).Select(_ => GetValidCategory()).ToList();
    }

    public string GetValidGenreName()
    {
        return Faker.Commerce.Categories(1)[0];
    }

    public bool GetRandomBoolean()
    {
        var randomNumber = new Random().Next(10);
        return randomNumber % 2 == 0;
    }

    public Genre GetExampleGenre()
    {
        return new Genre(GetValidGenreName(), GetRandomBoolean());
    }

    public List<Genre> GetRandomGenreList()
    {
        return Enumerable.Range(1, Random.Shared.Next(1, 5)).Select(_ => GetExampleGenre()).ToList();
    }

    internal List<Video> GetExampleVideoListWithNames(List<string> titles)
    {
        return titles.Select(title =>
        {
            var video = GetValidVideo();
            video.Update(title, video.Description, video.Opened, video.Published, video.YearLaunched, video.Duration, video.MovieRating);
            return video;
        }).ToList();
    }

    internal List<Video> CloneVideoListOrdered(List<Video> videos, string orderBy, SearchOrder searchOrder)
    {
        var listClone = new List<Video>(videos);
        var orderedEnumarable = (orderBy.ToLower(), searchOrder) switch
        {
            ("title", SearchOrder.Asc) => listClone.OrderBy(x => x.Title),
            ("title", SearchOrder.Desc) => listClone.OrderByDescending(x => x.Title),
            ("id", SearchOrder.Asc) => listClone.OrderBy(x => x.Id),
            ("id", SearchOrder.Desc) => listClone.OrderByDescending(x => x.Id),
            ("createdat", SearchOrder.Asc) => listClone.OrderBy(x => x.CreatedAt),
            ("createdat", SearchOrder.Desc) => listClone.OrderByDescending(x => x.CreatedAt),
            _ => listClone.OrderBy(x => x.Title)
        };

        return orderedEnumarable.ToList();
    }
}
