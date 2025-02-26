namespace BookingApp.Domain.Dtos;

public class CreateInventoryRequestDto
{
    public required string Title { get; set; }
    public required string Description { get; set; }
    public int RemainingCount { get; set; }
    public DateTime ExpirationDate { get; set; }
}