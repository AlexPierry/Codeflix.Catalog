using UnitTests.Application.CastMember.Common;

namespace UnitTests.Application.CastMember;

[CollectionDefinition(nameof(GetCastMemberTestFixture))]
public class GetCastMemberTestFixtureCollection : ICollectionFixture<GetCastMemberTestFixture> { }

public class GetCastMemberTestFixture : CastMemberBaseFixture
{
}