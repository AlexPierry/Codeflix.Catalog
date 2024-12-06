using Application.Interfaces;
using Domain.Enum;
using Domain.Repository;
using Moq;
using Entities = Domain.Entity;

namespace UnitTests.Application.CastMember.Common;

public abstract class CastMemberBaseFixture : BaseFixture
{
    public Mock<ICastMemberRepository> GetRepositoryMock() => new();

    public Mock<IUnitOfWork> GetUnitOfWorkMock() => new();

    public string GetValidName()
    {
        return Faker.Name.FullName();
    }

    public CastMemberType GetRandomCastMemberType()
    {
        return (CastMemberType)new Random().Next(1, 2);
    }

    public Entities.CastMember GetExampleCastMember() => new Entities.CastMember(GetValidName(), GetRandomCastMemberType());

    public List<Entities.CastMember> GetExampleCastMembersList(int length = 10)
    {
        var list = new List<Entities.CastMember>();
        for (int index = 0; index < length; index++)
        {
            list.Add(GetExampleCastMember());
        }

        return list;
    }
}