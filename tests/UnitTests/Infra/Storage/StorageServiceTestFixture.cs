using System;

namespace UnitTests.Infra.Storage;
[CollectionDefinition(nameof(StorageServiceTestFixture))]
public class StorageServiceTestFixtureCollection : ICollectionFixture<StorageServiceTestFixture>
{
    // This class is intentionally left empty. Its purpose is to be a collection fixture.
}

public class StorageServiceTestFixture : BaseFixture
{
    public string GetBucketName()
    {
        return "code-flix-medias";
    }

    public string GetFileName()
    {
        return Faker.System.CommonFileName();
    }

    public string GetContentType()
    {
        return "video/mp4";
    }

    public string GetFileContent()
    {
        return Faker.Lorem.Paragraph();
    }
}
