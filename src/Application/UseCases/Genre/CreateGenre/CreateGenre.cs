using Application.Exceptions;
using Application.Interfaces;
using Application.UseCases.Genre.Common;
using Domain.Repository;
using Entities = Domain.Entity;

namespace Application.UseCases.Genre.CreateGenre;

public class CreateGenre : ICreateGenre
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IGenreRepository _genreRepository;
    private readonly ICategoryRepository _categoryRepository;

    public CreateGenre(IGenreRepository genreRepository, IUnitOfWork unitOfWork, ICategoryRepository categoryRepository)
    {
        _genreRepository = genreRepository;
        _unitOfWork = unitOfWork;
        _categoryRepository = categoryRepository;
    }

    public async Task<GenreModelOutput> Handle(CreateGenreInput input, CancellationToken cancellationToken)
    {
        var genre = new Entities.Genre(input.Name, input.IsActive);

        await ValidateCategoryIds(input.Categories, cancellationToken);

        input.Categories?.ForEach(genre.AddCategory);

        await _genreRepository.Insert(genre, cancellationToken);
        await _unitOfWork.Commit(cancellationToken);

        return GenreModelOutput.FromGenre(genre);
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