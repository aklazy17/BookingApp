using BookingApp.Application.Queries.Inventory;
using BookingApp.Domain.Results;
using Moq;

namespace BookingApp.Application.UnitTests.Queries.Inventory;

public class GetInventoryByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnInventory_WhenInventoryExists()
    {
        // Arrange
        var inventoryId = Guid.NewGuid();

        var expectedInventoryResponse = new InventoryResponse
        {
            Id = inventoryId,
            Title = "Test Title",
            Description = "Test Description",
            RemainingCount = 4,
            ExpirationDate = DateTime.Now
        };

        var mockHandler = new Mock<IGetInventoryByIdQueryHandler>();
        mockHandler
            .Setup(handler => handler.Handle(inventoryId))
            .ReturnsAsync(Result<InventoryResponse?>.Success(expectedInventoryResponse));

        // Act
        var result = await mockHandler.Object.Handle(inventoryId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(inventoryId, result.Value.Id);
        Assert.Equal("Test Title", result.Value.Title);
        Assert.Equal(4, result.Value.RemainingCount);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenInventoryDoesNotExist()
    {
        // Arrange
        var inventoryId = Guid.NewGuid();

        var mockHandler = new Mock<IGetInventoryByIdQueryHandler>();
        mockHandler
            .Setup(handler => handler.Handle(inventoryId))
            .ReturnsAsync(Result<InventoryResponse?>.Fail(Error.NotFound("Inventory not found.")));

        // Act
        var result = await mockHandler.Object.Handle(inventoryId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Throws<InvalidOperationException>(() => result.Value);
        Assert.Equal("Inventory not found.", result.Error!.Message);
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenHandlerFails()
    {
        // Arrange
        var inventoryId = Guid.NewGuid();

        var mockHandler = new Mock<IGetInventoryByIdQueryHandler>();
        mockHandler
            .Setup(handler => handler.Handle(inventoryId))
            .ThrowsAsync(new Exception("Internal server error."));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => mockHandler.Object.Handle(inventoryId));
    }
}