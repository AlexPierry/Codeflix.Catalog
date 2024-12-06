using Application.UseCases.CastMember.Common;
using MediatR;

namespace Application.UseCases.CastMember;

public interface IUpdateCastMember : IRequestHandler<UpdateCastMemberInput, CastMemberModelOutput>
{

}