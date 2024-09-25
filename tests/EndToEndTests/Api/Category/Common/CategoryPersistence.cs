using Infra.Data.EF;
using Microsoft.EntityFrameworkCore;
using Entities = Domain.Entity;

namespace EndToEndTests.Api.Category.Common;

public class CategoryPersistence
{
    private readonly CodeflixCatalogDbContext _context;

    public CategoryPersistence(CodeflixCatalogDbContext context)
    {
        _context = context;
    }

    public async Task<Entities.Category?> GetById(Guid id)
    {
        return await _context.Categories.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
    }
}