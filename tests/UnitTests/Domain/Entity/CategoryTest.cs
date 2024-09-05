using Domain.Entity;
using Domain.Exceptions;
using FluentAssertions;

namespace UnitTests.Domain.Entity;

[Collection(nameof(CategoryTestFixture))]
public class CategoryTest
{
    private readonly CategoryTestFixture _categoryTestFixture;

    public CategoryTest(CategoryTestFixture categoryTestFixture)
    {
        _categoryTestFixture = categoryTestFixture;
    }

    [Fact(DisplayName = nameof(Instantiate))]
    [Trait("Domain", "Category - Aggregates")]
    public void Instantiate()
    {
        // Given
        var validCategory = _categoryTestFixture.GetValidCategory();
        var datetimeBefore = DateTime.Now;

        // When
        var category = new Category(validCategory.Name, validCategory.Description);
        var datetimeAfter = DateTime.Now;

        // Then
        category.Should().NotBeNull();
        category.Name.Should().Be(validCategory.Name);
        category.Description.Should().Be(validCategory.Description);
        category.Id.Should().NotBe(default(Guid));
        category.CreatedAt.Should().NotBe(default);
        (category.CreatedAt >= datetimeBefore).Should().BeTrue();
        (category.CreatedAt <= datetimeAfter).Should().BeTrue();
        category.IsActive.Should().BeTrue();
    }

    [Theory(DisplayName = nameof(InstantiateWithIsActive))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData(true)]
    [InlineData(false)]
    public void InstantiateWithIsActive(bool isActive)
    {
        // Given
        var validCategory = _categoryTestFixture.GetValidCategory();
        var datetimeBefore = DateTime.Now;

        // When
        var category = new Category(validCategory.Name, validCategory.Description, isActive);
        var datetimeAfter = DateTime.Now;

        // Then
        category.Should().NotBeNull();
        category.Name.Should().Be(validCategory.Name);
        category.Description.Should().Be(validCategory.Description);
        category.Id.Should().NotBe(default(Guid));
        category.CreatedAt.Should().NotBe(default);
        (category.CreatedAt >= datetimeBefore).Should().BeTrue();
        (category.CreatedAt <= datetimeAfter).Should().BeTrue();
        category.IsActive.Should().Be(isActive);
    }

    [Theory(DisplayName = nameof(InstantiateErrorWhenNameIsEmpty))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("  ")]
    public void InstantiateErrorWhenNameIsEmpty(string? name)
    {
        // Given
        var validCategory = _categoryTestFixture.GetValidCategory();

        // When
        Action action = () => new Category(name!, validCategory.Description);

        // Then
        action.Should().Throw<EntityValidationException>().WithMessage("Name should not be null or empty.");
    }

    [Fact(DisplayName = nameof(InstantiateErrorWhenDescriptionIsNull))]
    [Trait("Domain", "Category - Aggregates")]
    public void InstantiateErrorWhenDescriptionIsNull()
    {
        // Given
        var validCategory = _categoryTestFixture.GetValidCategory();

        // When
        Action action = () => new Category(validCategory.Name, null!);

        // Then
        action.Should().Throw<EntityValidationException>().WithMessage("Description should not be null.");
    }

    [Theory(DisplayName = nameof(InstantiateErrorWhenNameHasLessThan3Characters))]
    [Trait("Domain", "Category - Aggregates")]
    [MemberData(nameof(GetNamesWithLessThan3Characters), parameters: 10)]
    public void InstantiateErrorWhenNameHasLessThan3Characters(string invalidName)
    {
        // Given        
        var validCategory = _categoryTestFixture.GetValidCategory();

        // When
        Action action = () => new Category(invalidName, validCategory.Description);

        // Then
        action.Should().Throw<EntityValidationException>().WithMessage("Name should not be less than 3 characters long.");
    }

    [Fact(DisplayName = nameof(InstantiateErrorWhenNameHasMoreThan255Characters))]
    [Trait("Domain", "Category - Aggregates")]
    public void InstantiateErrorWhenNameHasMoreThan255Characters()
    {
        // Given
        var validCategory = _categoryTestFixture.GetValidCategory();
        var invalidName = string.Join(null, Enumerable.Range(1, 256).Select(_ => "a").ToArray());

        // When
        Action action = () => new Category(invalidName, validCategory.Description);

        // Then
        action.Should().Throw<EntityValidationException>().WithMessage("Name should not be greater than 255 characters long.");
    }

    [Fact(DisplayName = nameof(InstantiateErrorWhenDescriptionHasMoreThan10_000Characters))]
    [Trait("Domain", "Category - Aggregates")]
    public void InstantiateErrorWhenDescriptionHasMoreThan10_000Characters()
    {
        // Given
        var validCategory = _categoryTestFixture.GetValidCategory();
        var invalidDescription = string.Join(null, Enumerable.Range(1, 10001).Select(_ => "a").ToArray());

        // When
        Action action = () => new Category(validCategory.Name, invalidDescription);

        // Then
        action.Should().Throw<EntityValidationException>().WithMessage("Description should not be greater than 10000 characters long.");
    }

    [Fact(DisplayName = nameof(Activate))]
    [Trait("Domain", "Category - Aggregates")]
    public void Activate()
    {
        // Given
        var validCategory = _categoryTestFixture.GetValidCategory();

        // When
        var category = new Category(validCategory.Name, validCategory.Description, false);
        category.Activate();

        // Then
        category.IsActive.Should().BeTrue();
    }

    [Fact(DisplayName = nameof(Deactivate))]
    [Trait("Domain", "Category - Aggregates")]
    public void Deactivate()
    {
        // Given
        var validData = new
        {
            Name = "category name",
            Description = "category description"
        };

        // When
        var category = new Category(validData.Name, validData.Description, true);
        category.Deactivate();

        // Then
        category.IsActive.Should().BeFalse();
    }

    [Fact(DisplayName = nameof(Update))]
    [Trait("Domain", "Category - Aggregates")]
    public void Update()
    {
        // Given
        var category = _categoryTestFixture.GetValidCategory();

        // When
        var newValues = _categoryTestFixture.GetValidCategory();
        category.Update(newValues.Name, newValues.Description);

        // Then
        category.Name.Should().Be(newValues.Name);
        category.Description.Should().Be(newValues.Description);
    }

    [Fact(DisplayName = nameof(UpdateOnlyName))]
    [Trait("Domain", "Category - Aggregates")]
    public void UpdateOnlyName()
    {
        // Given
        var category = _categoryTestFixture.GetValidCategory();

        // When
        var newValues = _categoryTestFixture.GetValidCategory();
        var currentDescription = category.Description;
        category.Update(newValues.Name);

        // Then
        category.Name.Should().Be(newValues.Name);
        category.Description.Should().Be(currentDescription);
    }

    [Theory(DisplayName = nameof(UpdateErrorWhenNameIsEmpty))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("  ")]
    public void UpdateErrorWhenNameIsEmpty(string? name)
    {
        // Given
        var category = _categoryTestFixture.GetValidCategory();

        // When
        Action action = () => category.Update(name!);

        // Then
        action.Should().Throw<EntityValidationException>().WithMessage("Name should not be null or empty.");
    }

    [Theory(DisplayName = nameof(UpdateErrorWhenNameHasLessThan3Characters))]
    [Trait("Domain", "Category - Aggregates")]
    [MemberData(nameof(GetNamesWithLessThan3Characters), parameters: 10)]
    public void UpdateErrorWhenNameHasLessThan3Characters(string invalidName)
    {
        // Given
        var category = _categoryTestFixture.GetValidCategory();

        // When
        Action action = () => category.Update(invalidName!);

        // Then
        action.Should().Throw<EntityValidationException>().WithMessage("Name should not be less than 3 characters long.");
    }

    public static IEnumerable<object[]> GetNamesWithLessThan3Characters(int numberOfTests)
    {
        var fixture = new CategoryTestFixture();
        for (int i = 0; i < numberOfTests; i++)
        {
            var isOdd = i % 2 == 1;
            yield return new object[] {
                fixture.GetValidCategory().Name[..(isOdd ? 1: 2)]
            };
        }
    }

    [Fact(DisplayName = nameof(UpdateErrorWhenNameHasMoreThan255Characters))]
    [Trait("Domain", "Category - Aggregates")]
    public void UpdateErrorWhenNameHasMoreThan255Characters()
    {
        // Given
        var invalidName = _categoryTestFixture.Faker.Lorem.Letter(256);
        var category = _categoryTestFixture.GetValidCategory();

        // When
        Action action = () => category.Update(invalidName!);

        // Then
        action.Should().Throw<EntityValidationException>().WithMessage("Name should not be greater than 255 characters long.");
    }

    [Fact(DisplayName = nameof(UpdateErrorWhenDescriptionHasMoreThan10_000Characters))]
    [Trait("Domain", "Category - Aggregates")]
    public void UpdateErrorWhenDescriptionHasMoreThan10_000Characters()
    {
        // Given
        var category = _categoryTestFixture.GetValidCategory();
        var invalidDescription = _categoryTestFixture.Faker.Commerce.ProductDescription();
        while (invalidDescription.Length <= 10_000)
            invalidDescription += " " + _categoryTestFixture.Faker.Commerce.ProductDescription();

        // When
        Action action = () => category.Update(category.Name, invalidDescription);

        // Then
        action.Should().Throw<EntityValidationException>().WithMessage("Description should not be greater than 10000 characters long.");
    }
}