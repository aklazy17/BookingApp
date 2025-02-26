using BookingApp.Domain.Entities;

namespace BookingApp.Domain.Interfaces;

public interface IBookingRepository
{
    Task<Booking> InsertAsync(Booking entity);
    Task<Booking> UpdateAsync(Booking entity);
    Task<Booking?> GetByIdAsync(Guid id);
    Task<IEnumerable<Booking>> GetAllAsync();
}