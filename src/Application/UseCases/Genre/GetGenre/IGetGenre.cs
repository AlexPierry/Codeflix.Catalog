using Application.UseCases.Genre.Common;
using MediatR;

namespace Application.UseCases.Genre.GetGenre;

public interface IGetGenre : IRequestHandler<GetGenreInput, GenreModelOutput> { }