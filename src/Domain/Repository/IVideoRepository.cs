using Domain.Entity;
using Domain.SeedWork.SearchableRepository;

namespace Domain.Repository;

public interface IVideoRepository : IGenericRepository<Video>, ISearchableRepository<Video>
{
}
