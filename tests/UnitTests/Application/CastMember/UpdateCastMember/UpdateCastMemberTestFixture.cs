using Application.UseCases.CastMember;
using UnitTests.Application.CastMember.Common;

namespace UnitTests.Application.CastMember;

[CollectionDefinition(nameof(UpdateCastMemberTestFixture))]
public class UpdateCastMemberTestFixtureCollection : ICollectionFixture<UpdateCastMemberTestFixture> { }

public class UpdateCastMemberTestFixture : CastMemberBaseFixture
{
    public UpdateCastMemberInput GetValidInput(Guid? id = null)
    {
        return new(
            id ?? Guid.NewGuid(),
            GetValidName(),
            GetRandomCastMemberType()
        );
    }
}