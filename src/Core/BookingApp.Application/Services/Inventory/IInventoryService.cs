using BookingApp.Domain.Dtos;
using BookingApp.Domain.Results;
using Entity = BookingApp.Domain.Entities;

namespace BookingApp.Application.Services.Inventory;

public interface IInventoryService
{
    Task<Result<Entity.Inventory>> CreateAsync(CreateInventoryRequestDto dto);
    Task<Result<List<Entity.Inventory>>> BulkCreate(List<BulkCreateInventoryRequestDto> dtos);
    Task<Result<Entity.Inventory>> UpdateRemainingCountAsync(Guid inventoryId, int remainingCount);
    Task<Result<Entity.Inventory?>> GetByIdAsync(Guid inventoryId);
    Task<Result<IEnumerable<Entity.Inventory>>> GetAllAsync();
}