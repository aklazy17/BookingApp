using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingApp.Domain.Entities;

public class Member
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? SurName { get; set; }
    public int BookingCount { get; set; } = 0;
    public DateTime DateJoined { get; set; } = DateTime.Now;
}