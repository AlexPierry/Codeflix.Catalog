using Application.UseCases.Genre.Common;
using MediatR;

namespace Application.UseCases.Genre.UpdateGenre;
public interface IUpdateGenre : IRequestHandler<UpdateGenreInput, GenreModelOutput>
{
}