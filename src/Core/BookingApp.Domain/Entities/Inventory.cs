using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingApp.Domain.Entities;

public class Inventory
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public int RemainingCount { get; set; }
    public DateTime ExpirationDate { get; set; }
}