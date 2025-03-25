
using UnitTests.Common.Fixtures;

namespace UnitTests.Application.Video;

[CollectionDefinition(nameof(GetVideoTestFixture))]
public class GetVideoTestFixtureCollection : ICollectionFixture<GetVideoTestFixture>
{
}

public class GetVideoTestFixture : VideoTestFixtureBase
{
    
}
