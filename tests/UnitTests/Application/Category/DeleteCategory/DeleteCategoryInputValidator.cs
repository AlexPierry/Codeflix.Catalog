using Application.UseCases.Category.DeleteCategory;
using FluentAssertions;

namespace UnitTests.Application.Category;

[Collection(nameof(DeleteCategoryTestFixture))]
public class DeleteCategoryInputValidatorTest
{
    private readonly DeleteCategoryTestFixture _fixture;

    public DeleteCategoryInputValidatorTest(DeleteCategoryTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(ValidationOk))]
    [Trait("Application", "DeleteCategory - Use Cases")]
    public void ValidationOk()
    {
        // Given
        var validInput = new DeleteCategoryInput(Guid.NewGuid());
        var validator = new DeleteCategoryInputValidator();

        // When
        var validationResult = validator.Validate(validInput);

        // Then
        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeTrue();
        validationResult.Errors.Should().HaveCount(0);
    }

    [Fact(DisplayName = nameof(InvalidWhenEmptyId))]
    [Trait("Application", "DeleteCategory - Use Cases")]
    public void InvalidWhenEmptyId()
    {
        // Given
        var invalidInput = new DeleteCategoryInput(Guid.Empty);
        var validator = new DeleteCategoryInputValidator();

        // When
        var validationResult = validator.Validate(invalidInput);

        // Then
        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().HaveCount(1);
        validationResult.Errors[0].ErrorMessage.Should().Be("'Id' must not be empty.");
    }
}