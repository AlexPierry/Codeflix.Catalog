using Application.Common;
using Domain.SeedWork.SearchableRepository;
using MediatR;

namespace Application.UseCases.Category.ListCategories;

public class ListCategoriesInput : PaginatedListInput, IRequest<ListCategoriesOutput>
{
    public ListCategoriesInput(int page = 1, int perPage = 15, string search = "", string sort = "", SearchOrder dir = SearchOrder.Asc)
        : base(page, perPage, search, sort, dir)
    {
    }

    public ListCategoriesInput() : base(1, 15, "", "", SearchOrder.Asc)
    {
    }
}