namespace BookingApp.Application.Queries.Member;

public class MemberResponse
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? SurName { get; set; }
    public int BookingCount { get; set; }
    public DateTime DateJoined { get; set; }
}