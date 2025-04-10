using Domain.Entity;
using Domain.SeedWork.SearchableRepository;

namespace Domain.Repository;

public interface ICastMemberRepository : IGenericRepository<CastMember>, ISearchableRepository<CastMember>
{
    public Task<IReadOnlyList<Guid>> GetIdsListByIds(List<Guid> ids, CancellationToken cancellationToken);
    public Task<IReadOnlyList<CastMember>> GetListByIds(List<Guid> ids, CancellationToken cancellationToken);
}