using Domain.Enum;
using Domain.SeedWork.SearchableRepository;
using EndToEndTests.Base;
using Entities = Domain.Entity;

namespace EndToEndTests.Api.CastMember;

[CollectionDefinition(nameof(CastMemberBaseFixture))]
public class CastMemberBaseFixtureCollection : ICollectionFixture<CastMemberBaseFixture> { }

public class CastMemberBaseFixture : BaseFixture
{
    public CastMemberPersistence Persistence { get; private set; }

    public CastMemberBaseFixture() : base()
    {
        var dbContext = CreateDbContext();
        Persistence = new CastMemberPersistence(dbContext);
    }

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
        return Enumerable.Range(1, length).Select(_ => GetExampleCastMember()).ToList();
    }

    public List<Entities.CastMember> GetExampleCastMembersListWithNames(List<string> names)
    {
        return names.Select(name =>
        {
            var castMember = GetExampleCastMember();
            castMember.Update(name, castMember.Type);
            return castMember;
        }).ToList();
    }

    public List<Entities.CastMember> CloneCastMembersListOrdered(List<Entities.CastMember> castMembers, string orderBy, SearchOrder order)
    {
        var listClone = new List<Entities.CastMember>(castMembers);
        var orderedEnumarable = (orderBy.ToLower(), order) switch
        {
            ("name", SearchOrder.Asc) => listClone.OrderBy(x => x.Name),
            ("name", SearchOrder.Desc) => listClone.OrderByDescending(x => x.Name),
            ("id", SearchOrder.Asc) => listClone.OrderBy(x => x.Id),
            ("id", SearchOrder.Desc) => listClone.OrderByDescending(x => x.Id),
            ("createdat", SearchOrder.Asc) => listClone.OrderBy(x => x.CreatedAt),
            ("createdat", SearchOrder.Desc) => listClone.OrderByDescending(x => x.CreatedAt),
            _ => listClone.OrderBy(x => x.Name)
        };

        return orderedEnumarable.ToList();
    }
}