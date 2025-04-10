using Domain.Entity;
using Domain.SeedWork.SearchableRepository;

namespace Domain.Repository;

public interface IGenreRepository : IGenericRepository<Genre>, ISearchableRepository<Genre>
{
    public Task<IReadOnlyList<Guid>> GetIdsListByIds(List<Guid> ids, CancellationToken cancellationToken);
    public Task<IReadOnlyList<Genre>> GetListByIds(List<Guid> ids, CancellationToken cancellationToken);
}