using Application.UseCases.CastMember;
using IntegrationTest.Application.UseCases.CastMember.Common;
using Entities = Domain.Entity;

namespace IntegrationTest.Application.UseCases.CastMember;

[CollectionDefinition(nameof(UpdateCastMemberTestFixture))]
public class UpdateCastMemberTestFixtureCollection : ICollectionFixture<UpdateCastMemberTestFixture> { }

public class UpdateCastMemberTestFixture : CastMemberUseCaseBaseFixture
{
    public UpdateCastMemberInput GetInput(Entities.CastMember castMember)
        => new(castMember.Id, castMember.Name, castMember.Type);
}