using IntegrationTest.Application.UseCases.Category.Common;

namespace IntegrationTest.Application.UseCases.Category;

[CollectionDefinition(nameof(GetCategoryTestFixture))]
public class GetCategoryTestFixtureCollection : ICollectionFixture<GetCategoryTestFixture> { }

public class GetCategoryTestFixture : CategoryUseCasesBaseFixture
{

}