using FluentValidation;

namespace Application.UseCases.CastMember;

public class UpdateCastMemberInputValidator : AbstractValidator<UpdateCastMemberInput>
{
    public UpdateCastMemberInputValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}