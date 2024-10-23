using Application.Interfaces;
using Application.UseCases.Genre.Common;
using Domain.Repository;
using Entities = Domain.Entity;

namespace Application.UseCases.Genre.CreateGenre;

public class CreateGenre : ICreateGenre
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IGenreRepository _genreRepository;

    public CreateGenre(IGenreRepository genreRepository, IUnitOfWork unitOfWork)
    {
        _genreRepository = genreRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<GenreModelOutput> Handle(CreateGenreInput input, CancellationToken cancellationToken)
    {
        var genre = new Entities.Genre(input.Name, input.IsActive);
        input.Categories?.ForEach(genre.AddCategory);

        await _genreRepository.Insert(genre, cancellationToken);
        await _unitOfWork.Commit(cancellationToken);

        return GenreModelOutput.FromGenre(genre);
    }
}