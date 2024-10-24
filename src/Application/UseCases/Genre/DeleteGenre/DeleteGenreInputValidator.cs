using FluentValidation;

namespace Application.UseCases.Genre.DeleteGenre;

public class DeleteGenreInputValidator : AbstractValidator<DeleteGenreInput>
{
    public DeleteGenreInputValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}