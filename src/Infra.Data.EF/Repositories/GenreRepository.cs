using Application.Exceptions;
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

    public async Task<Genre> Get(Guid id, CancellationToken cancellationToken)
    {
        var genre = await _genres.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        NotFoundException.ThrowIfNull(genre, $"Genre '{id}' not found.");
        var categoriesIds = await _genresCategories
            .Where(x => x.GenreId == genre!.Id)
            .Select(x => x.CategoryId)
            .ToListAsync(cancellationToken);
        categoriesIds.ForEach(genre!.AddCategory);
        return genre;
    }

    public Task Delete(Genre genre, CancellationToken cancellationToken)
    {
        _genresCategories.RemoveRange(_genresCategories.Where(x => x.GenreId == genre.Id));
        _genres.Remove(genre);
        return Task.CompletedTask;
    }

    public async Task Update(Genre genre, CancellationToken cancellationToken)
    {
        _genres.Update(genre);
        _genresCategories.RemoveRange(_genresCategories.Where(x => x.GenreId == genre.Id));
        if (genre.Categories.Count > 0)
        {
            var relations = genre.Categories.Select(categoryId => new GenresCategories(categoryId, genre.Id));
            await _genresCategories.AddRangeAsync(relations);
        }
    }

    public async Task<SearchOutput<Genre>> Search(SearchInput input, CancellationToken cancellationToken)
    {
        var toSkip = (input.Page - 1) * input.PerPage;

        var query = _genres.AsNoTracking();

        query = AddOrderToQuery(query, input.OrderBy, input.Order);

        if (!string.IsNullOrWhiteSpace(input.Search))
        {
            query = query.Where(x => x.Name.Contains(input.Search));
        }

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip(toSkip)
            .Take(input.PerPage)
            .ToListAsync(cancellationToken);

        var relations = await _genresCategories.AsNoTracking()
            .Where(x => items.Select(g => g.Id).ToList().Contains(x.GenreId)).ToListAsync();

        foreach (var item in items)
        {
            foreach (var relation in relations.Where(x => x.GenreId == item.Id))
            {
                item.AddCategory(relation.CategoryId);
            }
        }

        return new SearchOutput<Genre>(input.Page, input.PerPage, total, items);
    }

    private IQueryable<Genre> AddOrderToQuery(IQueryable<Genre> query, string orderProperty, SearchOrder order)
    {
        var orderedQuery = (orderProperty.ToLower(), order) switch
        {
            ("name", SearchOrder.Asc) => query.OrderBy(x => x.Name),
            ("name", SearchOrder.Desc) => query.OrderByDescending(x => x.Name),
            ("id", SearchOrder.Asc) => query.OrderBy(x => x.Id),
            ("id", SearchOrder.Desc) => query.OrderByDescending(x => x.Id),
            ("createdat", SearchOrder.Asc) => query.OrderBy(x => x.CreatedAt),
            ("createdat", SearchOrder.Desc) => query.OrderByDescending(x => x.CreatedAt),
            _ => query.OrderBy(x => x.Name)
        };

        return orderedQuery.ThenBy(x => x.CreatedAt);
    }

    public Task<IReadOnlyList<Guid>> GetIdsListByIds(List<Guid> guids, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}