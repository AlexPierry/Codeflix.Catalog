using Application.UseCases.Video.CreateVideo;
using UnitTests.Common.Fixtures;

namespace UnitTests.Application.Video;

[CollectionDefinition(nameof(CreateVideoTestFixture))]
public class CreateVideoTestFixtureCollection : ICollectionFixture<CreateVideoTestFixture>
{

}

public class CreateVideoTestFixture : VideoTestFixtureBase
{

    public CreateVideoInput GetValidCreateVideoInput(List<Guid>? CategoriesIds = null)
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
            CategoriesIds
        );
    }
}
