using Application.UseCases.CastMember.Common;
using Domain.Enum;
using MediatR;

namespace Application.UseCases.CastMember;

public class CreateCastMemberInput : IRequest<CastMemberModelOutput>
{
    public string Name { get; set; }
    public CastMemberType Type { get; set; }

    public CreateCastMemberInput(string name, CastMemberType type)
    {
        Name = name;
        Type = type;
    }
}