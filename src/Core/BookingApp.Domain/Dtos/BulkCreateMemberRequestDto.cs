namespace BookingApp.Domain.Dtos;

public class BulkCreateMemberRequestDto
{
    public required string Name { get; set; }
    public string? SurName { get; set; }
    public int BookingCount { get; set; }
    public DateTime DateJoined { get; set; }
}