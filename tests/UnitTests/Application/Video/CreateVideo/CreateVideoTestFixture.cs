using Application.UseCases.Video.Common;
using Application.UseCases.Video.CreateVideo;
using UnitTests.Common.Fixtures;

namespace UnitTests.Application.Video;

[CollectionDefinition(nameof(CreateVideoTestFixture))]
public class CreateVideoTestFixtureCollection : ICollectionFixture<CreateVideoTestFixture>
{

}

public class CreateVideoTestFixture : VideoTestFixtureBase
{
    public CreateVideoInput GetValidCreateVideoInput(
        List<Guid>? categoriesIds = null,
        List<Guid>? genresIds = null,
        List<Guid>? castMembersIds = null,
        FileInput? thumb = null,
        FileInput? banner = null,
        FileInput? thumbHalf = null,
        FileInput? media = null,
        FileInput? trailer = null
    )
    {
        return new CreateVideoInput
        (
            GetValidTitle(),
            GetValidDescription(),
            GetValidYear(),
            GetValidOpened(),
            GetValidPublished(),
            GetValidDuration(),
            GetRandomMovieRating(),
            categoriesIds,
            genresIds,
            castMembersIds,
            thumb,
            banner,
            thumbHalf,
            media,
            trailer
        );
    }

    public CreateVideoInput GetCreateVideoInputWithAllImages()
    {
        return new CreateVideoInput
        (
            GetValidTitle(),
            GetValidDescription(),
            GetValidYear(),
            GetValidOpened(),
            GetValidPublished(),
            GetValidDuration(),
            GetRandomMovieRating(),
            null,
            null,
            null,
            GetValidImageFileInput(),
            GetValidImageFileInput(),
            GetValidImageFileInput()
        );
    }

    public CreateVideoInput GetCreateVideoInputWithAllMedias()
    {
        return new CreateVideoInput
        (
            GetValidTitle(),
            GetValidDescription(),
            GetValidYear(),
            GetValidOpened(),
            GetValidPublished(),
            GetValidDuration(),
            GetRandomMovieRating(),
            null,
            null,
            null,
            null,
            null,
            null,
            GetValidMediaFileInput(),
            GetValidMediaFileInput()
        );
    }
}
