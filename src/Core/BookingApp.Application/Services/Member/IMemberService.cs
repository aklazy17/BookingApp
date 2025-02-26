using BookingApp.Domain.Dtos;
using BookingApp.Domain.Results;
using Entity = BookingApp.Domain.Entities;

namespace BookingApp.Application.Services.Member;

public interface IMemberService
{
    Task<Result<Entity.Member>> CreateAsync(CreateMemberRequestDto dto);
    Task<Result<List<Entity.Member>>> BulkCreate(List<BulkCreateMemberRequestDto> dtos);
    Task<Result<Entity.Member>> UpdateBookingCountAsync(Guid memberId, int bookingCount);
    Task<Result<Entity.Member>> GetByIdAsync(Guid memberId);
    Task<Result<IEnumerable<Entity.Member>>> GetAllAsync();
}