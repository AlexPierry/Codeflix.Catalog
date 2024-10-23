using Domain.Entity;
using Domain.SeedWork.SearchableRepository;

namespace Domain.Repository;

public interface IGenreRepository : IGenericRepository<Genre>, ISearchableRepository<Genre>
{

}