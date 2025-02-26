using BookingApp.Domain.Dtos;
using BookingApp.Domain.Results;
using Entity = BookingApp.Domain.Entities;

namespace BookingApp.Application.Services.Booking;

public interface IBookingService
{
    Task<Result<Entity.Booking>> CreateAsync(CreateBookingRequestDto dto);
    Task<Result<Entity.Booking>> CancelAsync(Guid bookingId);
    Task<Result<Entity.Booking?>> GetByIdAsync(Guid bookingId);
    Task<Result<IEnumerable<Entity.Booking>>> GetAllAsync();
}