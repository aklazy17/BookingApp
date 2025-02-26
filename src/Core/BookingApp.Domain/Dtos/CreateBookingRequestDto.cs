namespace BookingApp.Domain.Dtos;

public class CreateBookingRequestDto
{
    public Guid MemberId { get; set; }
    public Guid InventoryId { get; set; }
    public DateTime BookingDateTime { get; set; }
}