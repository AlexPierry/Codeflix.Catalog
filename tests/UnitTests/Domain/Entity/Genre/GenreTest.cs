using Domain.Exceptions;
using FluentAssertions;
using Entities = Domain.Entity;

namespace UnitTests.Domain.Entity.Genre;

[Collection(nameof(GenreTestFixture))]
public class GenreTest
{
    private readonly GenreTestFixture _fixture;

    public GenreTest(GenreTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(InstantiateOk))]
    [Trait("Domain", "Genre - Aggregates")]
    public void InstantiateOk()
    {
        // Given
        var genreName = _fixture.GetValidGenreName();
        var now = DateTime.Now;

        // When
        var genre = new Entities.Genre(genreName);

        // Then
        genre.Should().NotBeNull();
        genre.Name.Should().Be(genreName);
        genre.IsActive.Should().Be(true);
        genre.CreatedAt.Should().BeSameDateAs(now);
    }

    [Theory(DisplayName = nameof(InstantiateWithIsActive))]
    [Trait("Domain", "Genre - Aggregates")]
    [InlineData(true)]
    [InlineData(false)]
    public void InstantiateWithIsActive(bool isActive)
    {
        // Given
        var genreName = _fixture.GetValidGenreName();
        var now = DateTime.Now;

        // When
        var genre = new Entities.Genre(genreName, isActive);

        // Then
        genre.Should().NotBeNull();
        genre.Name.Should().Be(genreName);
        genre.IsActive.Should().Be(isActive);
        genre.CreatedAt.Should().BeSameDateAs(now);
    }

    [Theory(DisplayName = nameof(Activate))]
    [Trait("Domain", "Genre - Aggregates")]
    [InlineData(true)]
    [InlineData(false)]
    public void Activate(bool isActive)
    {
        // Given
        var exampleGenre = _fixture.GetExampleGenre(isActive);

        // When        
        exampleGenre.Activate();

        // Then
        exampleGenre.Should().NotBeNull();
        exampleGenre.IsActive.Should().BeTrue();
        exampleGenre.CreatedAt.Should().NotBe(default);
    }

    [Theory(DisplayName = nameof(Deactivate))]
    [Trait("Domain", "Genre - Aggregates")]
    [InlineData(false)]
    [InlineData(true)]
    public void Deactivate(bool isActive)
    {
        // Given
        var exampleGenre = _fixture.GetExampleGenre(isActive);

        // When
        exampleGenre.Deactivate();

        // Then
        exampleGenre.Should().NotBeNull();
        exampleGenre.IsActive.Should().BeFalse();
        exampleGenre.CreatedAt.Should().NotBe(default);
    }

    [Theory(DisplayName = nameof(Deactivate))]
    [Trait("Domain", "Genre - Aggregates")]
    [InlineData(false)]
    [InlineData(true)]
    public void Update(bool isActive)
    {
        // Given
        var exampleGenre = _fixture.GetExampleGenre(isActive);
        var newName = _fixture.GetValidGenreName();
        var oldIsActive = exampleGenre.IsActive;

        // When
        exampleGenre.Update(newName);

        // Then
        exampleGenre.Should().NotBeNull();
        exampleGenre.Name.Should().Be(newName);
        exampleGenre.IsActive.Should().Be(oldIsActive);
        exampleGenre.CreatedAt.Should().NotBe(default);
    }

    [Theory(DisplayName = nameof(ThrowsErrorWhenNameIsEmptyOrNull))]
    [Trait("Domain", "Genre - Aggregates")]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData(null)]
    public void ThrowsErrorWhenNameIsEmptyOrNull(string? invalidName)
    {
        // Given        

        // When
        var action = () => new Entities.Genre(invalidName!);

        // Then
        action.Should().Throw<EntityValidationException>().WithMessage("Name should not be null or empty.");
    }

    [Theory(DisplayName = nameof(UpdateThrowsErrorWhenNameIsEmptyOrNull))]
    [Trait("Domain", "Genre - Aggregates")]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData(null)]
    public void UpdateThrowsErrorWhenNameIsEmptyOrNull(string? invalidName)
    {
        // Given        
        var exampleGenre = _fixture.GetExampleGenre();

        // When
        var action = () => exampleGenre.Update(invalidName!);

        // Then
        action.Should().Throw<EntityValidationException>().WithMessage("Name should not be null or empty.");
    }

    [Fact(DisplayName = nameof(AddCategory))]
    [Trait("Domain", "Genre - Aggregates")]
    public void AddCategory()
    {
        // Given
        var exampleGenre = _fixture.GetExampleGenre();
        var categoryGuid = Guid.NewGuid();

        // When
        exampleGenre.AddCategory(categoryGuid);

        // Then
        exampleGenre.Categories.Should().HaveCount(1);
        exampleGenre.Categories.Should().Contain(categoryGuid);
    }

    [Fact(DisplayName = nameof(AddTwoCategory))]
    [Trait("Domain", "Genre - Aggregates")]
    public void AddTwoCategory()
    {
        // Given
        var exampleGenre = _fixture.GetExampleGenre();
        var categoryGuid = Guid.NewGuid();
        var categoryGuid2 = Guid.NewGuid();

        // When
        exampleGenre.AddCategory(categoryGuid);
        exampleGenre.AddCategory(categoryGuid2);

        // Then
        exampleGenre.Categories.Should().HaveCount(2);
        exampleGenre.Categories.Should().Contain(categoryGuid);
        exampleGenre.Categories.Should().Contain(categoryGuid2);
    }

    [Fact(DisplayName = nameof(RemoveCategory))]
    [Trait("Domain", "Genre - Aggregates")]
    public void RemoveCategory()
    {
        // Given
        var exampleGuid = Guid.NewGuid();
        var exampleGenre = _fixture.GetExampleGenre(categoryIds: new List<Guid> { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), exampleGuid });

        // When
        exampleGenre.RemoveCategory(exampleGuid);

        // Then
        exampleGenre.Categories.Should().HaveCount(3);
        exampleGenre.Categories.Should().NotContain(exampleGuid);
    }

    [Fact(DisplayName = nameof(RemoveAllCategories))]
    [Trait("Domain", "Genre - Aggregates")]
    public void RemoveAllCategories()
    {
        // Given        
        var exampleGenre = _fixture.GetExampleGenre(categoryIds: new List<Guid> {
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid() });

        // When
        exampleGenre.RemoveAllCategories();

        // Then
        exampleGenre.Categories.Should().HaveCount(0);
    }
}