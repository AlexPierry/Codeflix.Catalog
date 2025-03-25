using Application.Common;
using FluentAssertions;

namespace UnitTests.Application.Common;

public class StorageFileNameTest
{

    [Fact]
    [Trait("Application", "StorageFileName - Common")]
    public void CreateStorageFileName()
    {
        // Arrange
        var exampleId = Guid.NewGuid();
        var exampleExtension = "mp4";
        var propertyName = "videoFile";

        // Act
        var name = StorageFileName.Create(exampleId, propertyName, exampleExtension);

        // Assert
        name.Should().NotBeNullOrEmpty();
        name.Should().Be($"{exampleId}-{propertyName.ToLower()}.{exampleExtension.Replace(".", "")}");
    }
}
