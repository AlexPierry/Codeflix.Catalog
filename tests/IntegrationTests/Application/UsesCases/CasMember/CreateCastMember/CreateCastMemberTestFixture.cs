using Application.UseCases.CastMember;
using IntegrationTest.Application.UseCases.CastMember.Common;

namespace IntegrationTest.Application.UseCases.CastMember;

[CollectionDefinition(nameof(CreateCastMemberTestFixture))]
public class CreateCastMemberTestFixtureCollection : ICollectionFixture<CreateCastMemberTestFixture> { }

public class CreateCastMemberTestFixture : CastMemberUseCaseBaseFixture
{
    public CreateCastMemberInput GetInput() => new(GetValidName(), GetRandomCastMemberType());
}