using Application.UseCases.CastMember.Common;
using MediatR;

namespace Application.UseCases.CastMember;

public interface IGetCastMember : IRequestHandler<GetCastMemberInput, CastMemberModelOutput>
{

}