using MediatR;

namespace Application.UseCases.Category.DeleteCategory;

public interface IDeleteCategory : IRequestHandler<DeleteCategoryInput>
{

}