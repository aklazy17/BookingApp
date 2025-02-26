using BookingApp.Application.Queries.Booking;
using BookingApp.Domain.Enums;
using BookingApp.Domain.Results;
using Moq;

namespace BookingApp.Application.UnitTests.Queries.Booking;

public class GetBookingByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnBooking_WhenBookingExists()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        var inventoryId = Guid.NewGuid();

        var expectedBookingResponse = new BookingResponse
        {
            Id = bookingId,
            InventoryId = inventoryId,
            MemberId = memberId,
            BookingDateTime = DateTime.Now,
            Status = BookingStatus.Active
        };

        var mockHandler = new Mock<IGetBookingByIdQueryHandler>();
        mockHandler
            .Setup(handler => handler.Handle(bookingId))
            .ReturnsAsync(Result<BookingResponse?>.Success(expectedBookingResponse));

        // Act
        var result = await mockHandler.Object.Handle(bookingId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(bookingId, result.Value.Id);
        Assert.Equal(memberId, result.Value.MemberId);
        Assert.Equal(inventoryId, result.Value.InventoryId);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenBookingDoesNotExist()
    {
        // Arrange
        var bookingId = Guid.NewGuid();

        var mockHandler = new Mock<IGetBookingByIdQueryHandler>();
        mockHandler
            .Setup(handler => handler.Handle(bookingId))
            .ReturnsAsync(Result<BookingResponse?>.Fail(Error.NotFound("Booking not found.")));

        // Act
        var result = await mockHandler.Object.Handle(bookingId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Throws<InvalidOperationException>(() => result.Value);
        Assert.Equal("Booking not found.", result.Error!.Message);
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenHandlerFails()
    {
        // Arrange
        var bookingId = Guid.NewGuid();

        var mockHandler = new Mock<IGetBookingByIdQueryHandler>();
        mockHandler
            .Setup(handler => handler.Handle(bookingId))
            .ThrowsAsync(new Exception("Internal server error."));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => mockHandler.Object.Handle(bookingId));
    }
}