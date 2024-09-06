using Application.Interfaces;
using Application.UseCases.Category.Common;
using Domain.Repository;

namespace Application.UseCases.Category.UpdateCategory;

public class UpdateCategory : IUpdateCategory
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICategoryRepository _categoryRepository;

    public UpdateCategory(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CategoryModelOutput> Handle(UpdateCategoryInput request, CancellationToken cancellationToken)
    {
        var currentCategory = await _categoryRepository.Get(request.Id, cancellationToken);
        currentCategory.Update(request.Name, request.Description);
        if (request.IsActive != null && request.IsActive != currentCategory.IsActive)
        {
            if ((bool)request.IsActive)
                currentCategory.Activate();
            else
                currentCategory.Deactivate();
        }

        await _categoryRepository.Update(currentCategory, cancellationToken);
        await _unitOfWork.Commit(cancellationToken);

        return CategoryModelOutput.FromCategory(currentCategory);
    }
}