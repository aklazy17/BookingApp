using BookingApp.Domain.Results;

namespace BookingApp.Application.Queries.Inventory;

public interface IGetInventoryByIdQueryHandler
{
    Task<Result<InventoryResponse?>> Handle(Guid id);
}