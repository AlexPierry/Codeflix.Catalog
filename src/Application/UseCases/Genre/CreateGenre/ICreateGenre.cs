using Application.UseCases.Genre.Common;
using MediatR;

namespace Application.UseCases.Genre.CreateGenre;
public interface ICreateGenre : IRequestHandler<CreateGenreInput, GenreModelOutput>
{
}