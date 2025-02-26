using BookingApp.Application.Queries.Booking;
using BookingApp.Domain.Results;
using BookingApp.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace BookingApp.Persistence.Queries;

public class GetBookingByIdQueryHandler : IGetBookingByIdQueryHandler
{
    private readonly ApplicationDbContext _dbContext;

    public GetBookingByIdQueryHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<BookingResponse?>> Handle(Guid id)
    {
        var bookingResponse = await _dbContext.Bookings
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new BookingResponse
            {
                Id = x.Id,
                MemberId = x.MemberId,
                InventoryId = x.InventoryId,
                BookingDateTime = x.BookingDateTime,
                Status = x.Status
            })
            .FirstOrDefaultAsync();

        if (bookingResponse is null)
        {
            return Result<BookingResponse?>.Fail(Error.NotFound($"Booking with id {id} not found."));
        }

        return Result<BookingResponse?>.Success(bookingResponse);
    }
}