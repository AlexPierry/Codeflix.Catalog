using MediatR;

namespace Application.UseCases.CastMember;

public interface IListCastMembers : IRequestHandler<ListCastMembersInput, ListCastMembersOutput>
{
}