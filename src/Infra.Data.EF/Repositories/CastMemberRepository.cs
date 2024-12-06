using Application.Exceptions;
using Domain.Entity;
using Domain.Repository;
using Domain.SeedWork.SearchableRepository;
using Microsoft.EntityFrameworkCore;

namespace Infra.Data.EF.Repositories;

public class CastMemberRepository : ICastMemberRepository
{

    private readonly CodeflixCatalogDbContext _context;

    private DbSet<CastMember> _castMembers => _context.Set<CastMember>();

    public CastMemberRepository(CodeflixCatalogDbContext context)
    {
        _context = context;
    }

    public async Task Insert(CastMember aggregate, CancellationToken cancellationToken)
    {
        await _castMembers.AddAsync(aggregate, cancellationToken);
    }

    public async Task<CastMember> Get(Guid id, CancellationToken cancellationToken)
    {
        var castMember = await _castMembers.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        NotFoundException.ThrowIfNull(castMember, $"CastMember '{id}' not found.");
        return castMember!;
    }

    public Task Delete(CastMember aggregate, CancellationToken cancellationToken)
    {
        return Task.FromResult(_castMembers.Remove(aggregate));
    }

    public Task Update(CastMember aggregate, CancellationToken cancellationToken)
    {
        return Task.FromResult(_castMembers.Update(aggregate));
    }

    public async Task<SearchOutput<CastMember>> Search(SearchInput input, CancellationToken cancellationToken)
    {
        var toSkip = (input.Page - 1) * input.PerPage;

        var query = _castMembers.AsNoTracking();

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

        return new SearchOutput<CastMember>(input.Page, input.PerPage, total, items);
    }

    private IQueryable<CastMember> AddOrderToQuery(IQueryable<CastMember> query, string orderProperty, SearchOrder order)
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
}