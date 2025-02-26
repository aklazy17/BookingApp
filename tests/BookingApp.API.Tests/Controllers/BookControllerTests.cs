using BookingApp.Application.Queries.Booking;
using BookingApp.Application.Services.Booking;
using Entity = BookingApp.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Moq;
using BookingApp.Domain.Enums;
using BookingApp.Domain.Results;
using BookingApp.API.Controllers;
using BookingApp.Domain.Dtos;

namespace BookingApp.API.Tests.Controllers;

public class BookControllerTests
{
    /// <summary>
    /// Test Case 1: Successfully Retrieve All Bookings
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task Get_ShouldReturnOk_WhenBookingsExist()
    {
        // Arrange
        var mockBookingService = new Mock<IBookingService>();
        var mockGetBookingByIdQueryHandler = new Mock<IGetBookingByIdQueryHandler>();

        var bookings = new List<Entity.Booking>
        {
            new() { Id = Guid.NewGuid(), Status = BookingStatus.Active },
            new() { Id = Guid.NewGuid(), Status = BookingStatus.Cancelled }
        };

        mockBookingService.Setup(x => x.GetAllAsync())
            .ReturnsAsync(Result<IEnumerable<Entity.Booking>>.Success(bookings));

        var controller = new BookController(mockBookingService.Object,
            mockGetBookingByIdQueryHandler.Object);

        // Act
        var result = await controller.Get();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedBookings = Assert.IsType<List<Entity.Booking>>(okResult.Value);
        Assert.Equal(2, returnedBookings.Count);
    }

    /// <summary>
    /// Test Case 2: Fail to Retrieve All Bookings
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task Get_ShouldReturnBadRequest_WhenBookingsFailToRetrieve()
    {
        // Arrange
        var mockBookingService = new Mock<IBookingService>();
        var mockGetBookingByIdQueryHandler = new Mock<IGetBookingByIdQueryHandler>();

        mockBookingService.Setup(x => x.GetAllAsync())
            .ReturnsAsync(Result<IEnumerable<Entity.Booking>>.Fail(Error.ValidationError("Failed to retrieve bookings.")));

        var controller = new BookController(mockBookingService.Object,
            mockGetBookingByIdQueryHandler.Object);

        // Act
        var result = await controller.Get();

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Failed to retrieve bookings.", badRequestResult.Value);
    }

    /// <summary>
    /// Test Case 3: Successfully Retrieve Booking by ID
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetById_ShouldReturnOk_WhenBookingExists()
    {
        // Arrange
        var mockBookingService = new Mock<IBookingService>();
        var mockGetBookingByIdQueryHandler = new Mock<IGetBookingByIdQueryHandler>();

        var bookingId = Guid.NewGuid();
        var booking = new BookingResponse { Id = bookingId, MemberId = Guid.NewGuid(), InventoryId = Guid.NewGuid() };

        mockGetBookingByIdQueryHandler.Setup(x => x.Handle(bookingId))
            .ReturnsAsync(Result<BookingResponse?>.Success(booking));

        var controller = new BookController(mockBookingService.Object,
            mockGetBookingByIdQueryHandler.Object);

        // Act
        var result = await controller.GetById(bookingId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedBooking = Assert.IsType<BookingResponse>(okResult.Value);
        Assert.Equal(bookingId, returnedBooking.Id);
    }

    /// <summary>
    /// Test Case 4: Fail to Retrieve Booking - Booking Not Found
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenBookingDoesNotExist()
    {
        // Arrange
        var mockBookingService = new Mock<IBookingService>();
        var mockGetBookingByIdQueryHandler = new Mock<IGetBookingByIdQueryHandler>();

        var bookingId = Guid.NewGuid();

        mockGetBookingByIdQueryHandler.Setup(x => x.Handle(bookingId))
            .ReturnsAsync(Result<BookingResponse?>.Fail(Error.NotFound("Booking not found.")));

        var controller = new BookController(mockBookingService.Object,
            mockGetBookingByIdQueryHandler.Object);

        // Act
        var result = await controller.GetById(bookingId);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Booking not found.", notFoundResult.Value);
    }

    /// <summary>
    /// Test Case 5: Successfully Create a Booking
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task Post_ShouldReturnOk_WhenBookingIsCreated()
    {
        // Arrange
        var mockBookingService = new Mock<IBookingService>();
        var mockGetBookingByIdQueryHandler = new Mock<IGetBookingByIdQueryHandler>();

        var requestDto = new CreateBookingRequestDto { MemberId = Guid.NewGuid(), InventoryId = Guid.NewGuid() };
        var booking = new Entity.Booking { Id = Guid.NewGuid(), MemberId = requestDto.MemberId, InventoryId = requestDto.InventoryId, Status = BookingStatus.Active };

        mockBookingService.Setup(x => x.CreateAsync(requestDto))
            .ReturnsAsync(Result<Entity.Booking>.Success(booking));

        var controller = new BookController(mockBookingService.Object,
            mockGetBookingByIdQueryHandler.Object);

        // Act
        var result = await controller.Post(requestDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedBooking = Assert.IsType<Entity.Booking>(okResult.Value);
        Assert.Equal(booking.Id, returnedBooking.Id);
        Assert.Equal(booking.Status, returnedBooking.Status);
    }

    /// <summary>
    /// Test Case 6: Fail to Create Booking - Invalid Request
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task Post_ShouldReturnBadRequest_WhenRequestIsNull()
    {
        // Arrange
        var mockBookingService = new Mock<IBookingService>();
        var mockGetBookingByIdQueryHandler = new Mock<IGetBookingByIdQueryHandler>();

        var controller = new BookController(mockBookingService.Object,
            mockGetBookingByIdQueryHandler.Object);

        // Act
        var result = await controller.Post(null);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Request is null or empty.", badRequestResult.Value);
    }

    /// <summary>
    /// Test Case 7: Successfully Cancel a Booking
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task Cancel_ShouldReturnOk_WhenBookingIsCancelled()
    {
        // Arrange
        var mockBookingService = new Mock<IBookingService>();
        var mockGetBookingByIdQueryHandler = new Mock<IGetBookingByIdQueryHandler>();

        var bookingId = Guid.NewGuid();
        var booking = new Entity.Booking { Id = bookingId, Status = BookingStatus.Cancelled };

        mockBookingService.Setup(x => x.CancelAsync(bookingId))
            .ReturnsAsync(Result<Entity.Booking>.Success(booking));

        var controller = new BookController(mockBookingService.Object,
            mockGetBookingByIdQueryHandler.Object);

        // Act
        var result = await controller.Cancel(bookingId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedBooking = Assert.IsType<Entity.Booking>(okResult.Value);
        Assert.Equal(bookingId, returnedBooking.Id);
        Assert.Equal(BookingStatus.Cancelled, returnedBooking.Status);
    }

    /// <summary>
    /// Test Case 8: Fail to Cancel Booking - Booking Not Found
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task Cancel_ShouldReturnBadRequest_WhenBookingDoesNotExist()
    {
        // Arrange
        var mockBookingService = new Mock<IBookingService>();
        var mockGetBookingByIdQueryHandler = new Mock<IGetBookingByIdQueryHandler>();

        var bookingId = Guid.NewGuid();

        mockBookingService.Setup(x => x.CancelAsync(bookingId))
            .ReturnsAsync(Result<Entity.Booking>.Fail(Error.NotFound("Booking not found.")));

        var controller = new BookController(mockBookingService.Object,
            mockGetBookingByIdQueryHandler.Object);

        // Act
        var result = await controller.Cancel(bookingId);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Booking not found.", badRequestResult.Value);
    }
}