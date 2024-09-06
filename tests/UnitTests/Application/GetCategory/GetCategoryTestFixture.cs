using Domain.Entity;
using UnitTests.Application.Common;

namespace UnitTests.Application.GetCategory;

[CollectionDefinition(nameof(GetCategoryTestFixture))]
public class GetCategoryTestFixtureCollection : ICollectionFixture<GetCategoryTestFixture> { }

public class GetCategoryTestFixture : CategoryBaseFixture
{
}