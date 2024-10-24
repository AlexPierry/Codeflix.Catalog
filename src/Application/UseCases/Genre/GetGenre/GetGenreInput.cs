using Application.UseCases.Genre.Common;
using MediatR;

namespace Application.UseCases.Genre.GetGenre;

public class GetGenreInput : IRequest<GenreModelOutput>
{
    public Guid Id { get; set; }

    public GetGenreInput(Guid id)
    {
        Id = id;
    }
}