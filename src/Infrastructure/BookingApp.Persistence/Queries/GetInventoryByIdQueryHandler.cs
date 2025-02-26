using BookingApp.Application.Queries.Inventory;
using BookingApp.Domain.Results;
using BookingApp.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace BookingApp.Persistence.Queries;

public class GetInventoryByIdQueryHandler : IGetInventoryByIdQueryHandler
{
    private readonly ApplicationDbContext _dbContext;

    public GetInventoryByIdQueryHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<InventoryResponse?>> Handle(Guid id)
    {
        var inventoryResponse = await _dbContext.Inventories
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new InventoryResponse
            {
                Id = x.Id,
                Title = x.Title,
                Description = x.Description,
                RemainingCount = x.RemainingCount,
                ExpirationDate = x.ExpirationDate
            })
            .FirstOrDefaultAsync();

        if (inventoryResponse is null)
        {
            return Result<InventoryResponse?>.Fail(Error.NotFound($"Inventory with id {id} not found."));
        }

        return Result<InventoryResponse?>.Success(inventoryResponse);
    }
}