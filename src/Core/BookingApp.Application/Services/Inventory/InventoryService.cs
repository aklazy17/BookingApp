using AutoMapper;
using BookingApp.Domain.Dtos;
using BookingApp.Domain.Interfaces;
using BookingApp.Domain.Results;
using Entity = BookingApp.Domain.Entities;

namespace BookingApp.Application.Services.Inventory;

public class InventoryService : IInventoryService
{
    private readonly IMapper _mapper;
    private readonly IInventoryRepository _inventoryRepository;

    public InventoryService(IMapper mapper,
        IInventoryRepository inventoryRepository)
    {
        _mapper = mapper;
        _inventoryRepository = inventoryRepository;
    }

    public async Task<Result<Entity.Inventory>> CreateAsync(CreateInventoryRequestDto dto)
    {
        var inventory = _mapper.Map<Entity.Inventory>(dto);

        var result = await _inventoryRepository.InsertAsync(inventory);

        return Result<Entity.Inventory>.Success(result);
    }

    public async Task<Result<List<Entity.Inventory>>> BulkCreate(List<BulkCreateInventoryRequestDto> dtos)
    {
        var inventories = _mapper.Map<List<Entity.Inventory>>(dtos);

        var result = await _inventoryRepository.BulkInsertAsync(inventories);

        return Result<List<Entity.Inventory>>.Success(result);
    }

    public async Task<Result<Entity.Inventory?>> GetByIdAsync(Guid inventoryId)
    {
        var inventory = await _inventoryRepository.GetByIdAsync(inventoryId);
        if (inventory is not null)
        {
            return Result<Entity.Inventory?>.Success(inventory);
        }

        return Result<Entity.Inventory?>.Fail(Error.NotFound("Inventory not found."));
    }

    public async Task<Result<IEnumerable<Entity.Inventory>>> GetAllAsync()
    {
        var inventories = await _inventoryRepository.GetAllAsync();

        return Result<IEnumerable<Entity.Inventory>>.Success(inventories);
    }

    public async Task<Result<Entity.Inventory>> UpdateRemainingCountAsync(Guid inventoryId, int remainingCount)
    {
        var inventory = await _inventoryRepository.GetByIdAsync(inventoryId);
        if (inventory is not null)
        {
            inventory.RemainingCount = remainingCount;

            var result = await _inventoryRepository.UpdateAsync(inventory);
            if (result is not null)
            {
                return Result<Entity.Inventory>.Success(inventory);
            }
        }

        return Result<Entity.Inventory>.Fail(Error.NotFound("Inventory not found."));
    }
}