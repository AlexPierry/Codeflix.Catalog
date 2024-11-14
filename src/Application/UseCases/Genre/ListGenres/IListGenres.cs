using MediatR;

namespace Application.UseCases.Genre.ListGenres;

public interface IListGenres : IRequestHandler<ListGenresInput, ListGenresOutput>
{

}