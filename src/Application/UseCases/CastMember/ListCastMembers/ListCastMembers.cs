using Application.UseCases.CastMember.Common;
using Domain.Repository;
using Domain.SeedWork.SearchableRepository;

namespace Application.UseCases.CastMember;

public class ListCastMembers : IListCastMembers
{
    private readonly ICastMemberRepository _castMemberRepository;

    public ListCastMembers(ICastMemberRepository castMemberRepository)
    {
        _castMemberRepository = castMemberRepository;
    }

    public async Task<ListCastMembersOutput> Handle(ListCastMembersInput request, CancellationToken cancellationToken)
    {
        var searchOutput = await _castMemberRepository.Search(request.ToSearchInput(), cancellationToken);

        return ListCastMembersOutput.FromSearchOutput(searchOutput);
    }
}