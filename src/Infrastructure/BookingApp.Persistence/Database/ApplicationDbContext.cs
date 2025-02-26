using BookingApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookingApp.Persistence.Database;

public class ApplicationDbContext(DbContextOptions options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
    }

    public DbSet<Member> Members { get; set; }
    public DbSet<Inventory> Inventories { get; set; }
    public DbSet<Booking> Bookings { get; set; }
}