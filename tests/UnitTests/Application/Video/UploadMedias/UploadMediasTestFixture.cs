using Application.UseCases.Video.UploadMedias;
using UnitTests.Common.Fixtures;

namespace UnitTests.Application.Video;

[CollectionDefinition(nameof(UploadMediasTestFixture))]
public class UploadMediasTestFixtureCollection : ICollectionFixture<UploadMediasTestFixture>
{

}

public class UploadMediasTestFixture : VideoTestFixtureBase
{
    public UploadMediasInput GetValidUploadMediasInput(Guid? videoId = null, bool withTrailer = true)
    {
        return new UploadMediasInput(
            videoId ?? Guid.NewGuid(),
            GetValidImageFileInput(),
            withTrailer ? GetValidImageFileInput() : null
        );
    }
}
