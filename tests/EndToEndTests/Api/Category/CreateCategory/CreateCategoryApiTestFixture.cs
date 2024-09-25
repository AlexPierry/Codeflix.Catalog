using Application.UseCases.Category.CreateCategory;
using EndToEndTests.Api.Category.Common;

namespace EndToEndTests.Api.Category.CreateCategory;

[CollectionDefinition(nameof(CreateCategoryApiTestFixture))]
public class CreateCategoryApiTestFixtureCollection : ICollectionFixture<CreateCategoryApiTestFixture> { }

public class CreateCategoryApiTestFixture : CreateCategoryBaseFixture
{
    public CreateCategoryInput GetExampleInput()
    {
        return new(
            GetValidCategoryName(),
            GetValidCategoryDescription(),
            GetRandomBoolean()
        );
    }
}