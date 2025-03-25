using System.Text;
using Application.UseCases.Video.Common;
using Domain.Entity;
using Domain.Enum;

namespace UnitTests.Common.Fixtures;

public abstract class VideoTestFixtureBase : BaseFixture
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

    internal string GetTooLongTitle()
    {
        return Faker.Lorem.Letter(400);
    }

    internal string GetTooLongDescription()
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

    internal Media GetValidMedia()
    {
        return new Media(GetValidMediaPath());
    }

    internal FileInput GetValidImageFileInput()
    {
        var exampleStream = new MemoryStream(Encoding.ASCII.GetBytes("teste"));
        var fileInput = new FileInput("jpg", exampleStream);
        return fileInput;
    }

    internal FileInput GetValidVideoFileInput()
    {
        var exampleStream = new MemoryStream(Encoding.ASCII.GetBytes("teste"));
        var fileInput = new FileInput("mp4", exampleStream);
        return fileInput;
    }
}
