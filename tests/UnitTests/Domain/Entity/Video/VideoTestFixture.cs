using Domain.Enum;
using Entities = Domain.Entity;

namespace UnitTests.Domain.Entity.Video;

[CollectionDefinition(nameof(VideoTestFixture))]
public class VideoTestFixtureCollection : ICollectionFixture<VideoTestFixture> { }

public class VideoTestFixture : BaseFixture
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

    internal Entities.Video GetValidVideo()
    {
        var title = GetValidTitle();
        var description = GetValidDescription();
        var opened = GetValidOpened();
        var published = GetValidPublished();
        var year = GetValidYear();
        var duration = GetValidDuration();
        var rating = GetRandomMovieRating();

        return new Entities.Video(title, description, opened, published, year, duration, rating);
    }

    // Invalid stuff
    internal string GetTooLongTitle()
    {
        return Faker.Lorem.Letter(400);
    }

    internal string GetDescriptionTooLong()
    {
        var invalidDescription = Faker.Commerce.ProductDescription();
        while (invalidDescription.Length < 4000)
        {
            invalidDescription += Faker.Commerce.ProductDescription();
        }
        return invalidDescription;
    }

    internal MovieRating GetRandomMovieRating()
    {
        return (MovieRating)Faker.Random.Int(0, Enum.GetValues<MovieRating>().Length - 1);
    }
}