using System.Collections;
using Application.UseCases.Video.UpdateVideo;

namespace UnitTests.Application.Video;

public class UpdateVideoTestDataGenerator : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        var fixture = new UpdateVideoTestFixture();

        var invalidInputList = new List<object[]>();

        const int totalInvalidTests = 4;

        for (int index = 0; index < totalInvalidTests * 2; index++)
        {
            var exampleId = Guid.NewGuid();
            switch (index % totalInvalidTests)
            {
                case 0:
                    invalidInputList.Add([new UpdateVideoInput
                        (
                            exampleId,
                            "",
                            fixture.GetValidDescription(),
                            fixture.GetValidYear(),
                            fixture.GetValidOpened(),
                            fixture.GetValidPublished(),
                            fixture.GetValidDuration(),
                            fixture.GetRandomMovieRating()
                        ), "'Title' is required"]);
                    break;
                case 1:
                    invalidInputList.Add([new UpdateVideoInput
                        (
                            exampleId,
                            fixture.GetValidTitle(),
                            "",
                            fixture.GetValidYear(),
                            fixture.GetValidOpened(),
                            fixture.GetValidPublished(),
                            fixture.GetValidDuration(),
                            fixture.GetRandomMovieRating()
                        ), "'Description' is required"]);
                    break;
                case 2:
                    invalidInputList.Add([new UpdateVideoInput
                        (
                            exampleId,
                            fixture.GetTooLongTitle(),
                            fixture.GetValidDescription(),
                            fixture.GetValidYear(),
                            fixture.GetValidOpened(),
                            fixture.GetValidPublished(),
                            fixture.GetValidDuration(),
                            fixture.GetRandomMovieRating()
                        ), "'Title' should be less or equal 255 characters"]);
                    break;
                case 3:
                    invalidInputList.Add([new UpdateVideoInput
                        (
                            exampleId,
                            fixture.GetValidTitle(),
                            fixture.GetTooLongDescription(),
                            fixture.GetValidYear(),
                            fixture.GetValidOpened(),
                            fixture.GetValidPublished(),
                            fixture.GetValidDuration(),
                            fixture.GetRandomMovieRating()
                        ), "'Description' should be less or equal 4000 characters"]);
                    break;
                default:
                    break;
            }
        }

        return invalidInputList.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
