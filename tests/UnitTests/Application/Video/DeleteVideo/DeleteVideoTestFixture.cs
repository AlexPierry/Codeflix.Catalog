using System;
using UnitTests.Common.Fixtures;

namespace UnitTests.Application.Video;
[CollectionDefinition(nameof(DeleteVideoTestFixture))]
public class DeleteVideoTestFixtureCollection : ICollectionFixture<DeleteVideoTestFixture>
{
    // This class is used to define a collection fixture for the DeleteVideoTestFixture.
    // It allows us to share the same instance of DeleteVideoTestFixture across multiple test classes.
}

public class DeleteVideoTestFixture: VideoTestFixtureBase
{

}
