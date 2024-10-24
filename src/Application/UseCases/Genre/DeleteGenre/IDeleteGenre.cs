using MediatR;

namespace Application.UseCases.Genre.DeleteGenre;

public interface IDeleteGenre : IRequestHandler<DeleteGenreInput>
{

}