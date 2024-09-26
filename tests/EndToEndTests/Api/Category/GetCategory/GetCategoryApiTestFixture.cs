using EndToEndTests.Api.Category.Common;

namespace EndToEndTests.Api.Category.GetCategory;

[CollectionDefinition(nameof(GetCategoryApiTestFixture))]
public class GetCategoryApiTestFixtureCollection : ICollectionFixture<GetCategoryApiTestFixture> { }

public class GetCategoryApiTestFixture : CreateCategoryBaseFixture
{

}