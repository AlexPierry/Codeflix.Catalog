using Application.UseCases.Category.Common;
using MediatR;

namespace Application.UseCases.Category.CreateCategory;
public interface ICreateCategory : IRequestHandler<CreateCategoryInput, CategoryModelOutput>
{
}