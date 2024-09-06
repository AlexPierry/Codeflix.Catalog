using MediatR;

namespace Application.UseCases.Category.ListCategories;
public interface IListCategories : IRequestHandler<ListCategoriesInput, ListCategoriesOutput>
{
}