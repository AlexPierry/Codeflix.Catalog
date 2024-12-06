using Application.UseCases.CastMember;
using Domain.SeedWork.SearchableRepository;
using UnitTests.Application.CastMember.Common;

namespace UnitTests.Application.CastMember;
[CollectionDefinition(nameof(ListCastMembersTestFixture))]
public class ListCastMembersTestFixtureCollection : ICollectionFixture<ListCastMembersTestFixture> { }

public class ListCastMembersTestFixture : CastMemberBaseFixture
{
    public ListCastMembersInput GetExampleInput()
    {
        var random = new Random();
        return new ListCastMembersInput(
            page: random.Next(1, 10),
            perPage: random.Next(15, 100),
            search: Faker.Commerce.ProductName(),
            sort: Faker.Commerce.ProductName(),
            dir: random.Next(0, 10) > 5
                ? SearchOrder.Asc
                : SearchOrder.Desc
        );
    }
}