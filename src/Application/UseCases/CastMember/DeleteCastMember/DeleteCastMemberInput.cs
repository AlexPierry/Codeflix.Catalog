using MediatR;

namespace Application.UseCases.CastMember;

public class DeleteCastMemberInput : IRequest
{
    public Guid Id { get; set; }

    public DeleteCastMemberInput(Guid id)
    {
        Id = id;
    }
}