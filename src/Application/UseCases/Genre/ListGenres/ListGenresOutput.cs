using Application.Common;
using Application.UseCases.Genre.Common;
using Domain.SeedWork.SearchableRepository;
using MediatR;
using Entities = Domain.Entity;


namespace Application.UseCases.Genre.ListGenres;

public class ListGenresOutput : PaginatedListOutput<GenreModelOutput>, IRequest<ListGenresOutput>
{
    public ListGenresOutput(int page, int perPage, int total, IReadOnlyList<GenreModelOutput> items)
        : base(page, perPage, total, items)
    {
    }

    public static ListGenresOutput FromSearchOutput(SearchOutput<Entities.Genre> searchOutput)
    {
        return new ListGenresOutput(
            searchOutput.CurrentPage,
            searchOutput.PerPage,
            searchOutput.Total,
            searchOutput.Items
                .Select(GenreModelOutput.FromGenre)
                .ToList()
        );
    }
}