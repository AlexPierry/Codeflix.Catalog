using Application.UseCases.Category.Common;
using MediatR;

namespace Application.UseCases.Category.UpdateCategory;

public interface IUpdateCategory : IRequestHandler<UpdateCategoryInput, CategoryModelOutput>
{

}