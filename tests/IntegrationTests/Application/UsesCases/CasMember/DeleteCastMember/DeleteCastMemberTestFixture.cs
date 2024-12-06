using IntegrationTest.Application.UseCases.CastMember.Common;

namespace IntegrationTest.Application.UseCases.CastMember;

[CollectionDefinition(nameof(DeleteCastMemberTestFixture))]
public class DeleteCastMemberTestFixtureCollection : ICollectionFixture<DeleteCastMemberTestFixture> { }

public class DeleteCastMemberTestFixture : CastMemberUseCaseBaseFixture
{

}