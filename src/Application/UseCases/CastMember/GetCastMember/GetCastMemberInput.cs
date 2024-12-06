using Application.UseCases.CastMember.Common;
using MediatR;

namespace Application.UseCases.CastMember;

public class GetCastMemberInput : IRequest<CastMemberModelOutput>
{
    public Guid Id { get; set; }

    public GetCastMemberInput(Guid id)
    {
        Id = id;
    }
}