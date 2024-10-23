using Application.Common;

namespace Api.Models.Response;

public class ApiResponseList<TItemData> : ApiResponse<IReadOnlyList<TItemData>>
{
    public ApiResponseListMeta Meta { get; private set; }

    public ApiResponseList(int currentPage, int perPage, int total, IReadOnlyList<TItemData> data) : base(data)
    {
        Meta = new ApiResponseListMeta(currentPage, perPage, total);
    }

    public ApiResponseList(PaginatedListOutput<TItemData> paginatedList) : base(paginatedList.Items)
    {
        Meta = new ApiResponseListMeta(
            paginatedList.Page,
            paginatedList.PerPage,
            paginatedList.Total
        );
    }
}