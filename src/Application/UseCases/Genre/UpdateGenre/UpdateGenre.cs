using Application.Exceptions;
using Application.Interfaces;
using Application.UseCases.Genre.Common;
using Domain.Repository;

namespace Application.UseCases.Genre.UpdateGenre;

public class UpdateGenre : IUpdateGenre
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IGenreRepository _genreRepository;
    private readonly ICategoryRepository _categoryRepository;

    public UpdateGenre(IGenreRepository genreRepository, IUnitOfWork unitOfWork, ICategoryRepository categoryRepository)
    {
        _genreRepository = genreRepository;
        _unitOfWork = unitOfWork;
        _categoryRepository = categoryRepository;
    }

    public async Task<GenreModelOutput> Handle(UpdateGenreInput request, CancellationToken cancellationToken)
    {
        var currentGenre = await _genreRepository.Get(request.Id, cancellationToken);
        currentGenre.Update(request.Name);

        if (request.IsActive != null && request.IsActive != currentGenre.IsActive)
        {
            if ((bool)request.IsActive)
                currentGenre.Activate();
            else
                currentGenre.Deactivate();
        }

        if (request.Categories is not null)
        {
            await ValidateCategoryIds(request.Categories, cancellationToken);
            currentGenre.RemoveAllCategories();
            request.Categories.ForEach(currentGenre.AddCategory);
        }

        await _genreRepository.Update(currentGenre, cancellationToken);
        await _unitOfWork.Commit(cancellationToken);

        return GenreModelOutput.FromGenre(currentGenre);
    }

    private async Task ValidateCategoryIds(List<Guid>? categoryIds, CancellationToken cancellationToken)
    {
        if (categoryIds is not null && categoryIds.Count > 0)
        {
            var persistedList = await _categoryRepository.GetIdsListByIds(categoryIds, cancellationToken);
            var notFoundIds = categoryIds.Except(persistedList);
            if (notFoundIds.Count() > 0)
            {
                throw new RelatedAggregateException("Related category id(s) not found: " +
                    $"{string.Join(", ", notFoundIds)}");
            }
        }
    }
}