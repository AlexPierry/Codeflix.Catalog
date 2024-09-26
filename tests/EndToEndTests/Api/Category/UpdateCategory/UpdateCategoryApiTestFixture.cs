using Application.UseCases.Category.UpdateCategory;
using EndToEndTests.Api.Category.Common;

namespace EndToEndTests.Api.Category.UpdateCategory;

[CollectionDefinition(nameof(UpdateCategoryApiTestFixture))]
public class UpdateCategoryApiTestFixtureCollection : ICollectionFixture<UpdateCategoryApiTestFixture> { }

public class UpdateCategoryApiTestFixture : CategoryBaseFixture
{

    public UpdateCategoryInput GetExampleInput(Guid? id = null)
    {
        return new UpdateCategoryInput(
            id ?? Guid.NewGuid(),
            GetValidCategoryName(),
            GetValidCategoryDescription(),
            GetRandomBoolean());
    }
}