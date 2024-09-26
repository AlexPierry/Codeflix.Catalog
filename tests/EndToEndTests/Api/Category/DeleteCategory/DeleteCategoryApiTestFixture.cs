using EndToEndTests.Api.Category.Common;

namespace EndToEndTests.Api.Category.DeleteCategory;

[CollectionDefinition(nameof(DeleteCategoryApiTestFixture))]
public class DeleteCategoryApiTestFixtureCollection : ICollectionFixture<DeleteCategoryApiTestFixture>
{

}

public class DeleteCategoryApiTestFixture : CategoryBaseFixture
{

}