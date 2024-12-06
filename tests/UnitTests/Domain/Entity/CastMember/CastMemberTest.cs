using Domain.Enum;
using Domain.Exceptions;
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
        var name = _fixture.GetValidName(); ;
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

    [Theory(DisplayName = nameof(InstantiateErrorWhenNameIsEmpty))]
    [Trait("Domain", "CastMember - Aggregates")]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("  ")]
    public void InstantiateErrorWhenNameIsEmpty(string? name)
    {
        // Given
        var type = _fixture.GetRandomCastMemberType();

        // When
        Action action = () => new Entities.CastMember(name!, type);

        // Then
        action.Should().Throw<EntityValidationException>().WithMessage("Name should not be null or empty.");
    }

    [Fact(DisplayName = nameof(Update))]
    [Trait("Domain", "CastMember - Aggregates")]
    public void Update()
    {
        //Given
        var dateTimeBefore = DateTime.Now.AddSeconds(-1);
        var name = _fixture.GetValidName();
        var castMemberType = _fixture.GetRandomCastMemberType();
        var castMember = new Entities.CastMember(name, castMemberType);

        var newName = _fixture.GetValidName();
        var newType = _fixture.GetRandomCastMemberType();

        // When
        castMember.Update(newName, newType);

        // Then
        castMember.Id.Should().NotBe(default(Guid));
        castMember.Name.Should().Be(newName);
        castMember.Type.Should().Be(newType);
        (castMember.CreatedAt >= dateTimeBefore).Should().BeTrue();
    }

    [Theory(DisplayName = nameof(UpdateErrorWhenNameIsEmpty))]
    [Trait("Domain", "CastMember - Aggregates")]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("  ")]
    public void UpdateErrorWhenNameIsEmpty(string? newName)
    {
        // Given
        var castMember = _fixture.GetValidCastMember();

        // When
        Action action = () => castMember.Update(newName!, castMember.Type);

        // Then
        action.Should().Throw<EntityValidationException>().WithMessage("Name should not be null or empty.");
    }
}