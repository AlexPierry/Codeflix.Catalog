using Application.Common;
using Application.UseCases.Category.Common;
using MediatR;

namespace Application.UseCases.Category.ListCategories;

public class ListCategoriesOutput : PaginatedListOutput<CategoryModelOutput>, IRequest<ListCategoriesOutput>
{
    public ListCategoriesOutput(int page, int perPage, int total, IReadOnlyList<CategoryModelOutput> items)
        : base(page, perPage, total, items)
    {
    }
}