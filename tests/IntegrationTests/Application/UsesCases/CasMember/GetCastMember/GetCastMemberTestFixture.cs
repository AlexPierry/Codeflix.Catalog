using IntegrationTest.Application.UseCases.CastMember.Common;

namespace IntegrationTest.Application.UseCases.CastMember;

[CollectionDefinition(nameof(GetCastMemberTestFixture))]
public class GetCastMemberTestFixtureCollection : ICollectionFixture<GetCastMemberTestFixture> { }

public class GetCastMemberTestFixture : CastMemberUseCaseBaseFixture
{

}