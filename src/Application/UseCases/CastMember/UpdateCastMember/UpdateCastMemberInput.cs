using Application.UseCases.CastMember.Common;
using Domain.Enum;
using MediatR;

namespace Application.UseCases.CastMember;

public class UpdateCastMemberInput : IRequest<CastMemberModelOutput>
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public CastMemberType? Type { get; set; }

    public UpdateCastMemberInput(Guid id, string name, CastMemberType? type = null)
    {
        Id = id;
        Name = name;
        Type = type;
    }
}