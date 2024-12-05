using Api.Models.Category;
using EndToEndTests.Api.Category.Common;

namespace EndToEndTests.Api.Category;

[CollectionDefinition(nameof(UpdateCategoryApiTestFixture))]
public class UpdateCategoryApiTestFixtureCollection : ICollectionFixture<UpdateCategoryApiTestFixture> { }

public class UpdateCategoryApiTestFixture : CategoryBaseFixture
{

    public UpdateCategoryApiInput GetExampleInput()
    {
        return new UpdateCategoryApiInput(
            GetValidCategoryName(),
            GetValidCategoryDescription(),
            GetRandomBoolean());
    }
}