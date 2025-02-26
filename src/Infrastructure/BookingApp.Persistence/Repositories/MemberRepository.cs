using BookingApp.Domain.Entities;
using BookingApp.Domain.Interfaces;
using BookingApp.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace BookingApp.Persistence.Repositories;

public class MemberRepository : IMemberRepository
{
    private readonly ApplicationDbContext _dbContext;

    public MemberRepository(ApplicationDbContext dbContext) => _dbContext = dbContext;

    public async Task<List<Member>> BulkInsertAsync(List<Member> entities)
    {
        await _dbContext.Members.AddRangeAsync(entities);
        await _dbContext.SaveChangesAsync();

        return entities;
    }

    public async Task<IEnumerable<Member>> GetAllAsync()
    {
        return await _dbContext.Members.ToListAsync();
    }

    public async Task<Member?> GetByIdAsync(Guid MemberId)
    {
        return await _dbContext.Members.FindAsync(MemberId);
    }

    public async Task<Member> InsertAsync(Member entity)
    {
        await _dbContext.Members.AddAsync(entity);
        await _dbContext.SaveChangesAsync();

        return entity;
    }

    public async Task<Member> UpdateAsync(Member entity)
    {
        _dbContext.Members.Update(entity);
        await _dbContext.SaveChangesAsync();

        return entity;
    }
}