using Bogus;
using Domain.Exceptions;
using Domain.Validation;
using FluentAssertions;

namespace UnitTests.Domain.Validation;

public class DomainValidationTest
{
    private Faker Faker { get; set; } = new Faker();

    [Fact(DisplayName = nameof(NotNullOk))]
    [Trait("Domain", "DomainValidation - Validation")]
    public void NotNullOk()
    {
        var value = Faker.Commerce.ProductName();
        string fieldName = Faker.Commerce.ProductName().Replace(" ", "");

        Action action = () => DomainValidation.NotNull(value, fieldName);

        action.Should().NotThrow();
    }

    [Fact(DisplayName = nameof(NotNullThrowWhenNull))]
    [Trait("Domain", "DomainValidation - Validation")]
    public void NotNullThrowWhenNull()
    {
        string? value = null;
        string fieldName = Faker.Commerce.ProductName().Replace(" ", "");

        Action action = () => DomainValidation.NotNull(value, fieldName);
        action.Should().Throw<EntityValidationException>().WithMessage($"{fieldName} should not be null.");
    }

    [Theory(DisplayName = nameof(NotNullOrEmptyThrowWhenEmpty))]
    [Trait("Domain", "DomainValidation - Validation")]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void NotNullOrEmptyThrowWhenEmpty(string? target)
    {
        // Given
        string fieldName = Faker.Commerce.ProductName().Replace(" ", "");

        // When
        Action action = () => DomainValidation.NotNullOrEmpty(target, fieldName);

        // Then
        action.Should().Throw<EntityValidationException>().WithMessage($"{fieldName} should not be null or empty.");
    }

    [Fact(DisplayName = nameof(NotNullOrEmptyOk))]
    [Trait("Domain", "DomainValidation - Validation")]
    public void NotNullOrEmptyOk()
    {
        // Given
        var target = Faker.Commerce.ProductName();
        string fieldName = Faker.Commerce.ProductName().Replace(" ", "");

        // When
        Action action = () => DomainValidation.NotNullOrEmpty(target, fieldName);

        // Then
        action.Should().NotThrow();
    }

    [Theory(DisplayName = nameof(MinLengthThrowWhenLess))]
    [Trait("Domain", "DomainValidation - Validation")]
    [MemberData(nameof(GetValuesSmallerThanMin), 10)]
    public void MinLengthThrowWhenLess(string target, int minLength)
    {
        // Given
        string fieldName = Faker.Commerce.ProductName().Replace(" ", "");

        // When
        Action action = () => DomainValidation.MinLength(target, minLength, fieldName);

        // Then
        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage($"{fieldName} should not be less than {minLength} characters long.");
    }

    public static IEnumerable<object[]> GetValuesSmallerThanMin(int numTests)
    {
        yield return new object[] { "123456", 10 };
        var faker = new Faker();
        for (int i = 0; i < numTests - 1; i++)
        {
            var example = faker.Commerce.ProductName();
            var minLength = example.Length + new Random().Next(1, 20);
            yield return new object[] { example, minLength };
        }
    }

    [Theory(DisplayName = nameof(MinLengthOk))]
    [Trait("Domain", "DomainValidation - Validation")]
    [MemberData(nameof(GetValuesGraterThanMin), 10)]
    public void MinLengthOk(string target, int minLength)
    {
        // Given
        string fieldName = Faker.Commerce.ProductName().Replace(" ", "");

        // When
        Action action = () => DomainValidation.MinLength(target, minLength, fieldName);

        // Then
        action.Should().NotThrow();
    }

    public static IEnumerable<object[]> GetValuesGraterThanMin(int numTests)
    {
        yield return new object[] { "123456", 6 };
        var faker = new Faker();
        for (int i = 0; i < numTests - 1; i++)
        {
            var example = faker.Commerce.ProductName();
            var minLength = example.Length - new Random().Next(1, 5);
            yield return new object[] { example, minLength };
        }
    }

    [Theory(DisplayName = nameof(MaxLengthThrowWhenGreater))]
    [Trait("Domain", "DomainValidation - Validation")]
    [MemberData(nameof(GetValuesGraterThanMax), 10)]
    public void MaxLengthThrowWhenGreater(string target, int maxLength)
    {
        string fieldName = Faker.Commerce.ProductName().Replace(" ", "");

        Action action = () => DomainValidation.MaxLength(target, maxLength, fieldName);

        action.Should().Throw<EntityValidationException>().WithMessage($"{fieldName} should not be greater than {maxLength} characters long.");
    }

    public static IEnumerable<object[]> GetValuesGraterThanMax(int numTests)
    {
        yield return new object[] { "123456", 5 };
        var faker = new Faker();
        for (int i = 0; i < numTests - 1; i++)
        {
            var example = faker.Commerce.ProductName();
            var maxLength = example.Length - new Random().Next(1, 5);
            yield return new object[] { example, maxLength };
        }
    }

    [Theory(DisplayName = nameof(MaxLengthOk))]
    [Trait("Domain", "DomainValidation - Validation")]
    [MemberData(nameof(GetValuesLessThanMax), 10)]
    public void MaxLengthOk(string target, int maxLength)
    {
        string fieldName = Faker.Commerce.ProductName().Replace(" ", "");

        Action action = () => DomainValidation.MaxLength(target, maxLength, fieldName);

        action.Should().NotThrow();
    }

    public static IEnumerable<object[]> GetValuesLessThanMax(int numTests)
    {
        yield return new object[] { "123456", 6 };
        var faker = new Faker();
        for (int i = 0; i < numTests - 1; i++)
        {
            var example = faker.Commerce.ProductName();
            var maxLength = example.Length + new Random().Next(0, 5);
            yield return new object[] { example, maxLength };
        }
    }
}