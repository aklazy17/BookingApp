using BookingApp.Domain.Entities;

namespace BookingApp.Domain.Interfaces;

public interface IMemberRepository
{
    Task<Member> InsertAsync(Member entity);
    Task<Member> UpdateAsync(Member entity);
    Task<List<Member>> BulkInsertAsync(List<Member> entities);
    Task<Member?> GetByIdAsync(Guid MemberId);
    Task<IEnumerable<Member>> GetAllAsync();
}