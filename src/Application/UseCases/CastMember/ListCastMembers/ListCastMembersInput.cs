using Application.Common;
using Domain.SeedWork.SearchableRepository;
using MediatR;

namespace Application.UseCases.CastMember;

public class ListCastMembersInput : PaginatedListInput, IRequest<ListCastMembersOutput>
{
    public ListCastMembersInput(int page = 1, int perPage = 15, string search = "", string sort = "", SearchOrder dir = SearchOrder.Asc)
        : base(page, perPage, search, sort, dir)
    {
    }

    public ListCastMembersInput() : base(1, 15, "", "", SearchOrder.Asc)
    {
    }
}