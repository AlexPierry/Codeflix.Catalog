using Infra.Data.EF;
using Infra.Data.EF.Models;
using Microsoft.EntityFrameworkCore;
using Entities = Domain.Entity;

namespace EndToEndTests.Api.Genre.Common;

public class GenrePersistence
{
    private readonly CodeflixCatalogDbContext _context;

    public GenrePersistence(CodeflixCatalogDbContext context)
    {
        _context = context;
    }

    public async Task<Entities.Genre?> GetById(Guid id)
    {
        return await _context.Genres.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task InsertGenre(Entities.Genre genre)
    {
        await _context.Genres.AddAsync(genre);
        await _context.SaveChangesAsync();
    }

    public async Task InsertList(List<Entities.Genre> exampleGenreList)
    {
        await _context.Genres.AddRangeAsync(exampleGenreList);
        await _context.SaveChangesAsync();
    }

    public async Task InsertList(List<Entities.Category> exampleCategoryList)
    {
        await _context.Categories.AddRangeAsync(exampleCategoryList);
        await _context.SaveChangesAsync();
    }
}