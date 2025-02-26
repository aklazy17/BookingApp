namespace BookingApp.Domain.Dtos;

public class CreateMemberRequestDto
{
    public required string Name { get; set; }
    public string? SurName { get; set; }
}