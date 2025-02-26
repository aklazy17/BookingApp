using BookingApp.Domain.Results;

namespace BookingApp.Application.Queries.Booking;

public interface IGetBookingByIdQueryHandler
{
    Task<Result<BookingResponse?>> Handle(Guid id);
}