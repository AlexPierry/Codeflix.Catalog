using Domain.Entity;
using Domain.SeedWork.SearchableRepository;

namespace Domain.Repository;

public interface ICategoryRepository : IGenericRepository<Category>, ISearchableRepository<Category>
{

}