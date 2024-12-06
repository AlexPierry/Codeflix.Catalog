using UnitTests.Application.CastMember.Common;

namespace UnitTests.Application.CastMember;

[CollectionDefinition(nameof(DeleteCastMemberTestFixture))]
public class DeleteCastMemberTestFixtureCollection : ICollectionFixture<DeleteCastMemberTestFixture> { }

public class DeleteCastMemberTestFixture : CastMemberBaseFixture
{

}