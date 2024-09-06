using Application.UseCases.Category.Common;
using Application.UseCases.Category.GetCategory;
using MediatR;

public interface IGetCategory : IRequestHandler<GetCategoryInput, CategoryModelOutput>
{

}