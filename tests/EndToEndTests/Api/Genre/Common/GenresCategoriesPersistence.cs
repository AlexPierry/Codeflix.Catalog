using Infra.Data.EF;
using Infra.Data.EF.Models;
using Microsoft.EntityFrameworkCore;

namespace EndToEndTests.Api.Genre.Common;

public class GenresCategoriesPersistence
{
    private readonly CodeflixCatalogDbContext _context;

    public GenresCategoriesPersistence(CodeflixCatalogDbContext context)
    {
        _context = context;
    }

    public async Task InsertList(List<GenresCategories> genresCategories)
    {
        await _context.GenresCategories.AddRangeAsync(genresCategories);
        await _context.SaveChangesAsync();
    }

    internal async Task<List<GenresCategories>> GetByGenreId(Guid id)
    {
        return await _context.GenresCategories.Where(x => x.GenreId == id).ToListAsync();
    }
}