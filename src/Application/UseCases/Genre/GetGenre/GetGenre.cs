using Application.UseCases.Genre.Common;
using Domain.Repository;

namespace Application.UseCases.Genre.GetGenre;

public class GetGenre : IGetGenre
{
    private readonly IGenreRepository _genreRepository;

    public GetGenre(IGenreRepository genreRepository)
    {
        _genreRepository = genreRepository;
    }

    public async Task<GenreModelOutput> Handle(GetGenreInput request, CancellationToken cancellationToken)
    {
        var output = await _genreRepository.Get(request.Id, cancellationToken);

        return GenreModelOutput.FromGenre(output);
    }
}
