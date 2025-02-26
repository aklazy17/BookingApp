using BookingApp.Domain.Entities;
using BookingApp.Domain.Interfaces;
using BookingApp.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace BookingApp.Persistence.Repositories;

public class InventoryRepository : IInventoryRepository
{
    private readonly ApplicationDbContext _dbContext;

    public InventoryRepository(ApplicationDbContext dbContext) => _dbContext = dbContext;

    public async Task<List<Inventory>> BulkInsertAsync(List<Inventory> entities)
    {
        await _dbContext.Inventories.AddRangeAsync(entities);
        await _dbContext.SaveChangesAsync();

        return entities;
    }

    public async Task<IEnumerable<Inventory>> GetAllAsync()
    {
        return await _dbContext.Inventories.ToListAsync();
    }

    public async Task<Inventory?> GetByIdAsync(Guid inventoryId)
    {
        return await _dbContext.Inventories.FindAsync(inventoryId);
    }

    public async Task<Inventory> InsertAsync(Inventory entity)
    {
        await _dbContext.Inventories.AddAsync(entity);
        await _dbContext.SaveChangesAsync();

        return entity;
    }

    public async Task<Inventory> UpdateAsync(Inventory entity)
    {
        _dbContext.Inventories.Update(entity);
        await _dbContext.SaveChangesAsync();

        return entity;
    }
}