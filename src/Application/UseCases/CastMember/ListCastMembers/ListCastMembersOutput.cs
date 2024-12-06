using Application.Common;
using Application.UseCases.CastMember.Common;
using Domain.SeedWork.SearchableRepository;
using Entities = Domain.Entity;
using MediatR;

namespace Application.UseCases.CastMember;

public class ListCastMembersOutput : PaginatedListOutput<CastMemberModelOutput>, IRequest<ListCastMembersOutput>
{
    public ListCastMembersOutput(int page, int perPage, int total, IReadOnlyList<CastMemberModelOutput> items)
        : base(page, perPage, total, items)
    {
    }

    public static ListCastMembersOutput FromSearchOutput(SearchOutput<Entities.CastMember> searchOutput)
    {
        return new ListCastMembersOutput(
            searchOutput.CurrentPage,
            searchOutput.PerPage,
            searchOutput.Total,
            searchOutput.Items
                .Select(CastMemberModelOutput.FromCastMember)
                .ToList()
                .AsReadOnly()
        );
    }
}