using AutoMapper;
using BookingApp.Application.Services.Inventory;
using BookingApp.Domain.Dtos;
using BookingApp.Domain.Interfaces;
using Entity = BookingApp.Domain.Entities;
using Moq;

namespace BookingApp.Application.UnitTests.Services.Inventory;

public class InventoryServiceTests
{
    /// <summary>
    /// Test Case 1: Successfully Create an Inventory Item
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task CreateAsync_ShouldReturnSuccess_WhenInventoryIsCreated()
    {
        // Arrange
        var mockMapper = new Mock<IMapper>();
        var mockInventoryRepo = new Mock<IInventoryRepository>();

        var requestDto = new CreateInventoryRequestDto { Title = "Bali", Description = "Test Description", RemainingCount = 5 };
        var inventory = new Entity.Inventory { Id = Guid.NewGuid(), Title = "Bali", Description = "Test Description", RemainingCount = 5 };

        mockMapper.Setup(x => x.Map<Entity.Inventory>(requestDto))
            .Returns(inventory);

        mockInventoryRepo.Setup(x => x.InsertAsync(inventory))
            .ReturnsAsync(inventory);

        var inventoryService = new InventoryService(mockMapper.Object, mockInventoryRepo.Object);

        // Act
        var result = await inventoryService.CreateAsync(requestDto);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(inventory, result.Value);
    }

    /// <summary>
    /// Test Case 2: Fail to Create Inventory - Mapping Error
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task CreateAsync_ShouldThrowException_WhenMappingFails()
    {
        // Arrange
        var mockMapper = new Mock<IMapper>();
        var mockInventoryRepo = new Mock<IInventoryRepository>();

        var requestDto = new CreateInventoryRequestDto { Title = "Bali", Description = "Test Description", RemainingCount = 5 };

        mockMapper.Setup(x => x.Map<Entity.Inventory>(requestDto))
            .Throws(new Exception("Mapping failed."));

        var inventoryService = new InventoryService(mockMapper.Object, mockInventoryRepo.Object);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => inventoryService.CreateAsync(requestDto));
    }

    /// <summary>
    /// Test Case 3: Successfully Bulk Create Inventory Items
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task BulkCreate_ShouldReturnSuccess_WhenInventoriesAreCreated()
    {
        // Arrange
        var mockMapper = new Mock<IMapper>();
        var mockInventoryRepo = new Mock<IInventoryRepository>();

        var requestDtos = new List<BulkCreateInventoryRequestDto>
        {
            new BulkCreateInventoryRequestDto { Title = "Bali", Description = "Test Description", RemainingCount = 5 },
            new BulkCreateInventoryRequestDto { Title = "Paris", Description = "Test Description", RemainingCount = 3 }
        };

        var inventories = new List<Entity.Inventory>
        {
            new() { Id = Guid.NewGuid(), Title = "Bali", Description = "Test Description", RemainingCount = 5 },
            new() { Id = Guid.NewGuid(), Title = "Paris", Description = "Test Description", RemainingCount = 3 }
        };

        mockMapper.Setup(x => x.Map<List<Entity.Inventory>>(requestDtos))
            .Returns(inventories);

        mockInventoryRepo.Setup(x => x.BulkInsertAsync(inventories))
            .ReturnsAsync(inventories);

        var inventoryService = new InventoryService(mockMapper.Object, mockInventoryRepo.Object);

        // Act
        var result = await inventoryService.BulkCreate(requestDtos);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Count);
    }

    /// <summary>
    /// Test Case 4: Successfully Retrieve Inventory by ID
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetByIdAsync_ShouldReturnSuccess_WhenInventoryExists()
    {
        // Arrange
        var mockMapper = new Mock<IMapper>();
        var mockInventoryRepo = new Mock<IInventoryRepository>();

        var inventoryId = Guid.NewGuid();
        var inventory = new Entity.Inventory { Id = inventoryId, Title = "Bali", Description = "Test Description", RemainingCount = 5 };

        mockInventoryRepo.Setup(x => x.GetByIdAsync(inventoryId))
            .ReturnsAsync(inventory);

        var inventoryService = new InventoryService(mockMapper.Object, mockInventoryRepo.Object);

        // Act
        var result = await inventoryService.GetByIdAsync(inventoryId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(inventory, result.Value);
    }

    /// <summary>
    /// Test Case 5: Fail to Retrieve Inventory - Inventory Not Found
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetByIdAsync_ShouldReturnNotFound_WhenInventoryDoesNotExist()
    {
        // Arrange
        var mockMapper = new Mock<IMapper>();
        var mockInventoryRepo = new Mock<IInventoryRepository>();

        var inventoryId = Guid.NewGuid();

        mockInventoryRepo.Setup(x => x.GetByIdAsync(inventoryId))
            .ReturnsAsync((Entity.Inventory?)null);

        var inventoryService = new InventoryService(mockMapper.Object, mockInventoryRepo.Object);

        // Act
        var result = await inventoryService.GetByIdAsync(inventoryId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Inventory not found.", result.Error!.Message);
    }

    /// <summary>
    /// Test Case 6: Successfully Retrieve All Inventory Items
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetAllAsync_ShouldReturnAllInventories()
    {
        // Arrange
        var mockMapper = new Mock<IMapper>();
        var mockInventoryRepo = new Mock<IInventoryRepository>();

        var inventories = new List<Entity.Inventory>
        {
            new() { Id = Guid.NewGuid(), Description = "Test Description", Title = "Bali", RemainingCount = 5 },
            new() { Id = Guid.NewGuid(), Description = "Test Description", Title = "Paris", RemainingCount = 3 }
        };

        mockInventoryRepo.Setup(x => x.GetAllAsync())
            .ReturnsAsync(inventories);

        var inventoryService = new InventoryService(mockMapper.Object, mockInventoryRepo.Object);

        // Act
        var result = await inventoryService.GetAllAsync();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Count());
    }

    /// <summary>
    /// Test Case 7: Successfully Update Remaining Count
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task UpdateRemainingCountAsync_ShouldReturnSuccess_WhenInventoryIsUpdated()
    {
        // Arrange
        var mockMapper = new Mock<IMapper>();
        var mockInventoryRepo = new Mock<IInventoryRepository>();

        var inventoryId = Guid.NewGuid();
        var inventory = new Entity.Inventory { Id = inventoryId, Title = "Bali", Description = "Test Description", RemainingCount = 5 };

        mockInventoryRepo.Setup(x => x.GetByIdAsync(inventoryId))
            .ReturnsAsync(inventory);

        mockInventoryRepo.Setup(x => x.UpdateAsync(inventory))
            .ReturnsAsync(inventory);

        var inventoryService = new InventoryService(mockMapper.Object, mockInventoryRepo.Object);

        // Act
        var result = await inventoryService.UpdateRemainingCountAsync(inventoryId, 3);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(3, result.Value.RemainingCount);
    }

    /// <summary>
    /// Test Case 8: Fail to Update Remaining Count - Inventory Not Found
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task UpdateRemainingCountAsync_ShouldReturnNotFound_WhenInventoryDoesNotExist()
    {
        // Arrange
        var mockMapper = new Mock<IMapper>();
        var mockInventoryRepo = new Mock<IInventoryRepository>();

        var inventoryId = Guid.NewGuid();

        mockInventoryRepo.Setup(x => x.GetByIdAsync(inventoryId))
            .ReturnsAsync((Entity.Inventory?)null);

        var inventoryService = new InventoryService(mockMapper.Object, mockInventoryRepo.Object);

        // Act
        var result = await inventoryService.UpdateRemainingCountAsync(inventoryId, 3);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Inventory not found.", result.Error!.Message);
    }
}