using Application.UseCases.CastMember;
using UnitTests.Application.CastMember.Common;

namespace UnitTests.Application.CastMember;

[CollectionDefinition(nameof(CreateCastMemberTestFixture))]
public class CreateCastMemberTestFixtureCollection : ICollectionFixture<CreateCastMemberTestFixture> { }

public class CreateCastMemberTestFixture : CastMemberBaseFixture
{
    public CreateCastMemberInput GetInput() => new(GetValidName(), GetRandomCastMemberType());
}