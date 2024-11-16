using Domain.Repository;

namespace Application.UseCases.Genre.ListGenres;

public class ListGenres : IListGenres
{
    private readonly IGenreRepository _genreRepository;
    private readonly ICategoryRepository _categoryRepository;

    public ListGenres(IGenreRepository genreRepository, ICategoryRepository categoryRepository)
    {
        _genreRepository = genreRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<ListGenresOutput> Handle(ListGenresInput request, CancellationToken cancellationToken)
    {
        var searchOutput = await _genreRepository.Search(request.ToSearchInput(), cancellationToken);

        var output = ListGenresOutput.FromSearchOutput(searchOutput);

        var relatedCategoriesIds = searchOutput.Items.SelectMany(item => item.Categories).Distinct().ToList();

        if (relatedCategoriesIds.Count > 0)
        {
            var relatedCategories = await _categoryRepository.GetListByIds(relatedCategoriesIds, cancellationToken);
            output.FillWithCategoryNames(relatedCategories);
        }

        return output;
    }
}