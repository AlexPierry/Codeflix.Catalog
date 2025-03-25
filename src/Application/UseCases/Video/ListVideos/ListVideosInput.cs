using Application.Common;
using Domain.SeedWork.SearchableRepository;
using MediatR;

namespace Application.UseCases.Video.ListVideos;

public class ListVideosInput : PaginatedListInput, IRequest<ListVideosOutput>
{
    public ListVideosInput(int page = 1, int perPage = 15, string search = "", string sort = "", SearchOrder dir = SearchOrder.Asc)
        : base(page, perPage, search, sort, dir)
    {
    }

    public ListVideosInput() : base(1, 15, "", "", SearchOrder.Asc)
    {
    }
}
