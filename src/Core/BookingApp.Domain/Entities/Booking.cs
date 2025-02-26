using BookingApp.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingApp.Domain.Entities;

public class Booking
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    public Guid MemberId { get; set; }
    public Guid InventoryId { get; set; }
    public DateTime BookingDateTime { get; set; }
    public BookingStatus Status { get; set; }
}