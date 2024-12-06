using Domain.Enum;
using Entities = Domain.Entity;

namespace UnitTests.Domain.Entity.CastMember;

[CollectionDefinition(nameof(CastMemberTestFixture))]
public class CastMemberTestFixtureCollection : ICollectionFixture<CastMemberTestFixture> { }

public class CastMemberTestFixture : BaseFixture
{
    public string GetValidName()
    {
        return Faker.Name.FullName();
    }

    public CastMemberType GetRandomCastMemberType()
    {
        return (CastMemberType)new Random().Next(1, 2);
    }

    public Entities.CastMember GetValidCastMember()
    {
        return new Entities.CastMember(GetValidName(), GetRandomCastMemberType());
    }
}