using UnitTests.Application.Common;

namespace UnitTests.Application.Category;

[CollectionDefinition(nameof(GetCategoryTestFixture))]
public class GetCategoryTestFixtureCollection : ICollectionFixture<GetCategoryTestFixture> { }

public class GetCategoryTestFixture : CategoryBaseFixture
{
}