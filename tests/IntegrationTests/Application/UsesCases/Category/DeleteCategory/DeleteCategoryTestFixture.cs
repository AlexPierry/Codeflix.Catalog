using IntegrationTest.Application.UseCases.Category.Common;

namespace IntegrationTest.Application.UseCases.Category;

[CollectionDefinition(nameof(DeleteCategoryTestFixture))]
public class DeleteCategoryTestFixtureCollection : ICollectionFixture<DeleteCategoryTestFixture> { }

public class DeleteCategoryTestFixture : CategoryUseCasesBaseFixture
{

}
