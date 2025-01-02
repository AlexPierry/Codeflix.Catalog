using Infra.Data.EF;
using Microsoft.EntityFrameworkCore;
using Entities = Domain.Entity;

namespace EndToEndTests.Api.CastMember;

public class CastMemberPersistence
{
    private readonly CodeflixCatalogDbContext _context;

    public CastMemberPersistence(CodeflixCatalogDbContext context)
    {
        _context = context;
    }

    public async Task<Entities.CastMember?> GetById(Guid id)
    {
        return await _context.CastMembers.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task InsertCastMember(Entities.CastMember castMember)
    {
        await _context.CastMembers.AddAsync(castMember);
        await _context.SaveChangesAsync();
    }

    public async Task InsertList(List<Entities.CastMember> exampleCastMemberList)
    {
        await _context.CastMembers.AddRangeAsync(exampleCastMemberList);
        await _context.SaveChangesAsync();
    }
}