using BookingApp.API.Controllers;
using BookingApp.Application.Queries.Inventory;
using BookingApp.Application.Services.Inventory;
using Entity = BookingApp.Domain.Entities;
using BookingApp.Domain.Results;
using Microsoft.AspNetCore.Mvc;
using Moq;
using BookingApp.Domain.Dtos;
using Microsoft.AspNetCore.Http;
using System.Text;

namespace BookingApp.API.Tests.Controllers;

public class InventoryControllerTests
{
    /// <summary>
    /// Test Case 1: Successfully Retrieve All Inventory Items
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task Get_ShouldReturnOk_WhenInventoriesExist()
    {
        // Arrange
        var mockInventoryService = new Mock<IInventoryService>();
        var mockGetInventoryByIdQueryHandler = new Mock<IGetInventoryByIdQueryHandler>();

        var inventories = new List<Entity.Inventory>
        {
            new() { Id = Guid.NewGuid(), Title = "Bali", Description="Test Description", RemainingCount = 5 },
            new() { Id = Guid.NewGuid(), Title = "Paris", Description="Test Description", RemainingCount = 3 }
        };

        mockInventoryService.Setup(x => x.GetAllAsync())
            .ReturnsAsync(Result<IEnumerable<Entity.Inventory>>.Success(inventories));

        var controller = new InventoryController(mockInventoryService.Object,
            mockGetInventoryByIdQueryHandler.Object);

        // Act
        var result = await controller.Get();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedInventories = Assert.IsType<List<Entity.Inventory>>(okResult.Value);
        Assert.Equal(2, returnedInventories.Count);
    }

    /// <summary>
    /// Test Case 2: Fail to Retrieve All Inventory Items
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task Get_ShouldReturnBadRequest_WhenInventoriesFailToRetrieve()
    {
        // Arrange
        var mockInventoryService = new Mock<IInventoryService>();
        var mockGetInventoryByIdQueryHandler = new Mock<IGetInventoryByIdQueryHandler>();

        mockInventoryService.Setup(x => x.GetAllAsync())
            .ReturnsAsync(Result<IEnumerable<Entity.Inventory>>.Fail(Error.ValidationError("Failed to retrieve inventories.")));

        var controller = new InventoryController(mockInventoryService.Object,
            mockGetInventoryByIdQueryHandler.Object);

        // Act
        var result = await controller.Get();

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Failed to retrieve inventories.", badRequestResult.Value);
    }

    /// <summary>
    /// Test Case 3: Successfully Retrieve Inventory by ID
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetById_ShouldReturnOk_WhenInventoryExists()
    {
        // Arrange
        var mockInventoryService = new Mock<IInventoryService>();
        var mockGetInventoryByIdQueryHandler = new Mock<IGetInventoryByIdQueryHandler>();

        var inventoryId = Guid.NewGuid();
        var inventory = new InventoryResponse { Id = inventoryId, Title = "Bali", Description = "Test Description", RemainingCount = 5 };

        mockGetInventoryByIdQueryHandler.Setup(x => x.Handle(inventoryId))
            .ReturnsAsync(Result<InventoryResponse?>.Success(inventory));

        var controller = new InventoryController(mockInventoryService.Object,
            mockGetInventoryByIdQueryHandler.Object);

        // Act
        var result = await controller.Get(inventoryId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedInventory = Assert.IsType<InventoryResponse>(okResult.Value);
        Assert.Equal(inventoryId, returnedInventory.Id);
        Assert.Equal("Bali", returnedInventory.Title);
    }

    /// <summary>
    /// Test Case 4: Fail to Retrieve Inventory - Inventory Not Found
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenInventoryDoesNotExist()
    {
        // Arrange
        var mockInventoryService = new Mock<IInventoryService>();
        var mockGetInventoryByIdQueryHandler = new Mock<IGetInventoryByIdQueryHandler>();

        var inventoryId = Guid.NewGuid();

        mockGetInventoryByIdQueryHandler.Setup(x => x.Handle(inventoryId))
            .ReturnsAsync(Result<InventoryResponse?>.Fail(Error.NotFound("Inventory not found.")));

        var controller = new InventoryController(mockInventoryService.Object,
            mockGetInventoryByIdQueryHandler.Object);

        // Act
        var result = await controller.Get(inventoryId);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Inventory not found.", notFoundResult.Value);
    }

    /// <summary>
    /// Test Case 5: Fail to Retrieve Inventory - Invalid ID
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetById_ShouldReturnBadRequest_WhenIdIsEmpty()
    {
        // Arrange
        var mockInventoryService = new Mock<IInventoryService>();
        var mockGetInventoryByIdQueryHandler = new Mock<IGetInventoryByIdQueryHandler>();

        var controller = new InventoryController(mockInventoryService.Object,
            mockGetInventoryByIdQueryHandler.Object);

        // Act
        var result = await controller.Get(Guid.Empty);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Inventory id is null or empty.", badRequestResult.Value);
    }

    /// <summary>
    /// Test Case 6: Successfully Create an Inventory Item
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task Create_ShouldReturnOk_WhenInventoryIsCreated()
    {
        // Arrange
        var mockInventoryService = new Mock<IInventoryService>();
        var mockGetInventoryByIdQueryHandler = new Mock<IGetInventoryByIdQueryHandler>();

        var request = new CreateInventoryRequestDto { Title = "Bali", Description = "Test Description", RemainingCount = 5 };
        var inventory = new Entity.Inventory { Id = Guid.NewGuid(), Title = "Bali", Description = "Test Description", RemainingCount = 5 };

        mockInventoryService.Setup(x => x.CreateAsync(request))
            .ReturnsAsync(Result<Entity.Inventory>.Success(inventory));

        var controller = new InventoryController(mockInventoryService.Object, 
            mockGetInventoryByIdQueryHandler.Object);

        // Act
        var result = await controller.Create(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedInventory = Assert.IsType<Entity.Inventory>(okResult.Value);
        Assert.Equal(inventory.Id, returnedInventory.Id);
    }

    /// <summary>
    /// Test Case 7: Fail to Create Inventory - Invalid Request
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task Create_ShouldReturnBadRequest_WhenRequestIsNull()
    {
        // Arrange
        var mockInventoryService = new Mock<IInventoryService>();
        var mockGetInventoryByIdQueryHandler = new Mock<IGetInventoryByIdQueryHandler>();

        var controller = new InventoryController(mockInventoryService.Object,
            mockGetInventoryByIdQueryHandler.Object);

        // Act
        var result = await controller.Create(null);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Request is null or empty.", badRequestResult.Value);
    }

    /// <summary>
    /// Test Case 8: Successfully Upload CSV File
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task Upload_ShouldReturnOk_WhenCsvFileIsValid()
    {
        // Arrange
        var mockInventoryService = new Mock<IInventoryService>();
        var mockGetInventoryByIdQueryHandler = new Mock<IGetInventoryByIdQueryHandler>();

        var csvContent = "title,description,remaining_count,expiration_date\nBali,Suspendisse congue erat ac ex venenatis mattis.,5,19-11-2030";
        var csvFile = new Mock<IFormFile>();
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));

        csvFile.Setup(x => x.OpenReadStream()).Returns(stream);
        csvFile.Setup(x => x.FileName).Returns("inventory.csv");

        var inventories = new List<Entity.Inventory>
        {
            new() { Id = Guid.NewGuid(), Title = "Bali", Description = "Suspendisse congue erat ac ex venenatis mattis.", RemainingCount = 5 }
        };

        mockInventoryService.Setup(x => x.BulkCreate(It.IsAny<List<BulkCreateInventoryRequestDto>>()))
            .ReturnsAsync(Result<List<Entity.Inventory>>.Success(inventories));

        var controller = new InventoryController(mockInventoryService.Object, 
            mockGetInventoryByIdQueryHandler.Object);

        // Act
        var result = await controller.Upload(csvFile.Object);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedInventories = Assert.IsType<List<Entity.Inventory>>(okResult.Value);
        Assert.Single(returnedInventories);
    }

    /// <summary>
    /// Test Case 9: Fail to Upload CSV File - Invalid File Format
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task Upload_ShouldReturnBadRequest_WhenFileIsNotCsv()
    {
        // Arrange
        var mockInventoryService = new Mock<IInventoryService>();
        var mockGetInventoryByIdQueryHandler = new Mock<IGetInventoryByIdQueryHandler>();

        var csvFile = new Mock<IFormFile>();
        csvFile.Setup(x => x.FileName).Returns("inventory.txt");

        var controller = new InventoryController(mockInventoryService.Object,
            mockGetInventoryByIdQueryHandler.Object);

        // Act
        var result = await controller.Upload(csvFile.Object);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Invalid file format. Please upload a CSV file.", badRequestResult.Value);
    }

    /// <summary>
    /// Test Case 10: Fail to Upload CSV File - Invalid Headers
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task Upload_ShouldReturnBadRequest_WhenCsvHeadersAreInvalid()
    {
        // Arrange
        var mockInventoryService = new Mock<IInventoryService>();
        var mockGetInventoryByIdQueryHandler = new Mock<IGetInventoryByIdQueryHandler>();

        var csvContent = "invalid_header1,invalid_header2\nBali,Suspendisse congue erat ac ex venenatis mattis.";
        var csvFile = new Mock<IFormFile>();
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));

        csvFile.Setup(x => x.OpenReadStream()).Returns(stream);
        csvFile.Setup(x => x.FileName).Returns("inventory.csv");

        var controller = new InventoryController(mockInventoryService.Object, mockGetInventoryByIdQueryHandler.Object);

        // Act
        var result = await controller.Upload(csvFile.Object);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("CSV headers do not match expected format.", badRequestResult.Value);
    }

    /// <summary>
    /// Test Case 11: Fail to Upload CSV File - Data Conversion Error
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task Upload_ShouldReturnBadRequest_WhenCsvDataConversionFails()
    {
        // Arrange
        var mockInventoryService = new Mock<IInventoryService>();
        var mockGetInventoryByIdQueryHandler = new Mock<IGetInventoryByIdQueryHandler>();

        var csvContent = "title,description,remaining_count,expiration_date\nBali,Suspendisse congue erat ac ex venenatis mattis.,5,invalid_date";
        var csvFile = new Mock<IFormFile>();
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));

        csvFile.Setup(x => x.OpenReadStream()).Returns(stream);
        csvFile.Setup(x => x.FileName).Returns("inventory.csv");

        var controller = new InventoryController(mockInventoryService.Object, mockGetInventoryByIdQueryHandler.Object);

        // Act
        var result = await controller.Upload(csvFile.Object);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("CSV file contains data conversion errors. Please check the format of the data.", badRequestResult.Value);
    }

    /// <summary>
    /// Test Case 12: Fail to Upload CSV File - No Records Found
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task Upload_ShouldReturnBadRequest_WhenNoRecordsFound()
    {
        // Arrange
        var mockInventoryService = new Mock<IInventoryService>();
        var mockGetInventoryByIdQueryHandler = new Mock<IGetInventoryByIdQueryHandler>();

        var csvContent = "title,description,remaining_count,expiration_date\n";
        var csvFile = new Mock<IFormFile>();
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));

        csvFile.Setup(x => x.OpenReadStream()).Returns(stream);
        csvFile.Setup(x => x.FileName).Returns("inventory.csv");

        var controller = new InventoryController(mockInventoryService.Object, mockGetInventoryByIdQueryHandler.Object);

        // Act
        var result = await controller.Upload(csvFile.Object);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("No records found in the CSV file.", badRequestResult.Value);
    }
}