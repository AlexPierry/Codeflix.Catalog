using Application.UseCases.Video.Common;
using Application.UseCases.Video.UpdateVideo;
using UnitTests.Common.Fixtures;

namespace UnitTests.Application.Video;

[CollectionDefinition(nameof(UpdateVideoTestFixture))]
public class UpdateVideoTestFixtureCollection : ICollectionFixture<UpdateVideoTestFixture>
{
}

public class UpdateVideoTestFixture : VideoTestFixtureBase
{
    internal UpdateVideoInput GetUpdateVideoInput(Guid id, bool updateMovieRating,
        List<Guid>? genreIds = null,
        List<Guid>? categoryIds = null,
        List<Guid>? castMemberIds = null,
        FileInput? banner = null,
        FileInput? thumb = null,
        FileInput? thumbHalf = null)
    {
        return new UpdateVideoInput(
            Id: id,
            Title: GetValidTitle(),
            Description: GetValidDescription(),
            YearLaunched: GetValidYear(),
            Opened: GetValidOpened(),
            Published: GetValidPublished(),
            Duration: GetValidDuration(),
            MovieRating: updateMovieRating ? GetRandomMovieRating() : null,
            GenreIds: genreIds,
            CategoryIds: categoryIds,
            CastMemberIds: castMemberIds,
            Banner: banner,
            Thumb: thumb,
            ThumbHalf: thumbHalf
        );
    }
}
