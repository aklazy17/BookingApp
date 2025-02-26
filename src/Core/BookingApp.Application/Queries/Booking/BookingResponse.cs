using BookingApp.Domain.Enums;

namespace BookingApp.Application.Queries.Booking;

public class BookingResponse
{
    public Guid Id { get; set; }
    public Guid MemberId { get; set; }
    public Guid InventoryId { get; set; }
    public DateTime BookingDateTime { get; set; }
    public BookingStatus Status { get; set; }
}