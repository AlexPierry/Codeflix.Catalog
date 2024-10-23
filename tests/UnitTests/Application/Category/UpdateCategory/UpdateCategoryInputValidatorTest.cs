using Application.UseCases.Category.UpdateCategory;
using FluentAssertions;

namespace UnitTests.Application.UpdateCategory;

[Collection(nameof(UpdateCategoryTestFixture))]
public class UpdateCategoryInputValidatorTest
{
    private readonly UpdateCategoryTestFixture _fixture;

    public UpdateCategoryInputValidatorTest(UpdateCategoryTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(ValidationOk))]
    [Trait("Application", "UpdateCategory - Use Cases")]
    public void ValidationOk()
    {
        // Given
        var validInput = _fixture.GetValidInput();
        var validator = new UpdateCategoryInputValidator();

        // When
        var validationResult = validator.Validate(validInput);

        // Then
        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeTrue();
        validationResult.Errors.Should().HaveCount(0);
    }

    [Fact(DisplayName = nameof(InvalidWhenEmptyId))]
    [Trait("Application", "UpdateCategory - Use Cases")]
    public void InvalidWhenEmptyId()
    {
        // Given
        var invalidInput = _fixture.GetValidInput(Guid.Empty);
        var validator = new UpdateCategoryInputValidator();

        // When
        var validationResult = validator.Validate(invalidInput);

        // Then
        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().HaveCount(1);
        validationResult.Errors[0].ErrorMessage.Should().Be("'Id' must not be empty.");
    }
}