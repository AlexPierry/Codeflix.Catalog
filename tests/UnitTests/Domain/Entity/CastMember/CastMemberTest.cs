using Domain.Enum;
using FluentAssertions;
using Entities = Domain.Entity;

namespace UnitTests.Domain.Entity.CastMember;

[Collection(nameof(CastMemberTestFixture))]
public class CastMemberTest
{
    private readonly CastMemberTestFixture _fixture;

    public CastMemberTest(CastMemberTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(Instantiate))]
    [Trait("Domain", "CastMember - Aggregates")]
    public void Instantiate()
    {
        //Given
        var name = _fixture.GetValidName();;
        var castMemberType = _fixture.GetRandomCastMemberType();
        var dateTimeBefore = DateTime.Now.AddSeconds(-1);

        // When
        var castMember = new Entities.CastMember(name, castMemberType);

        // Then
        castMember.Id.Should().NotBe(default(Guid));
        castMember.Name.Should().Be(name);
        castMember.Type.Should().Be(castMemberType);
        (castMember.CreatedAt >= dateTimeBefore).Should().BeTrue();
    }
}