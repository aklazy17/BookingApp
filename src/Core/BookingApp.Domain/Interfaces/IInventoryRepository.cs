using BookingApp.Domain.Entities;

namespace BookingApp.Domain.Interfaces;

public interface IInventoryRepository
{
    Task<Inventory> InsertAsync(Inventory entity);
    Task<Inventory> UpdateAsync(Inventory entity);
    Task<List<Inventory>> BulkInsertAsync(List<Inventory> entities);
    Task<Inventory?> GetByIdAsync(Guid inventoryId);
    Task<IEnumerable<Inventory>> GetAllAsync();
}