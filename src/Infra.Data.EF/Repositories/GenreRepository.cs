using Domain.Entity;
using Domain.Repository;
using Domain.SeedWork.SearchableRepository;
using Infra.Data.EF.Models;
using Microsoft.EntityFrameworkCore;

namespace Infra.Data.EF.Repositories;

public class GenreRepository : IGenreRepository
{

    private readonly CodeflixCatalogDbContext _context;

    private DbSet<Genre> _genres => _context.Set<Genre>();
    private DbSet<GenresCategories> _genresCategories => _context.Set<GenresCategories>();

    public GenreRepository(CodeflixCatalogDbContext context)
    {
        _context = context;
    }

    public async Task Insert(Genre genre, CancellationToken cancellationToken)
    {
        await _genres.AddAsync(genre, cancellationToken);
        if (genre.Categories.Count > 0)
        {
            var relations = genre.Categories.Select(categoryId => new GenresCategories(categoryId, genre.Id));
            await _genresCategories.AddRangeAsync(relations);
        }
    }

    public Task<Genre> Get(Guid id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task Delete(Genre aggregate, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task Update(Genre aggregate, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<SearchOutput<Genre>> Search(SearchInput input, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}