using Application.Interfaces;
using Domain.Repository;

namespace Application.UseCases.Genre.DeleteGenre;

public class DeleteGenre : IDeleteGenre
{
    private readonly IGenreRepository _genreRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteGenre(IGenreRepository repository, IUnitOfWork unitOfWork)
    {
        _genreRepository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteGenreInput request, CancellationToken cancellationToken)
    {
        var genre = await _genreRepository.Get(request.Id, cancellationToken);
        await _genreRepository.Delete(genre, cancellationToken);
        await _unitOfWork.Commit(cancellationToken);
    }
}