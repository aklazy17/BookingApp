using BookingApp.Domain.Entities;
using BookingApp.Domain.Interfaces;
using BookingApp.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace BookingApp.Persistence.Repositories;

public class BookingRepository : IBookingRepository
{
    private readonly ApplicationDbContext _dbContext;

    public BookingRepository(ApplicationDbContext dbContext) => _dbContext = dbContext;

    public async Task<IEnumerable<Booking>> GetAllAsync()
    {
        return await _dbContext.Bookings.ToListAsync();
    }

    public async Task<Booking?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Bookings.FindAsync(id);
    }

    public async Task<Booking> InsertAsync(Booking entity)
    {
        await _dbContext.Bookings.AddAsync(entity);
        await _dbContext.SaveChangesAsync();

        return entity;
    }

    public async Task<Booking> UpdateAsync(Booking entity)
    {
        _dbContext.Bookings.Update(entity);
        await _dbContext.SaveChangesAsync();

        return entity;
    }
}