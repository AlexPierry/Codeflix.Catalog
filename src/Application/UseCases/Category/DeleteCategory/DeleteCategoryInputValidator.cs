using FluentValidation;

namespace Application.UseCases.Category.DeleteCategory;

public class DeleteCategoryInputValidator : AbstractValidator<DeleteCategoryInput>
{
    public DeleteCategoryInputValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}