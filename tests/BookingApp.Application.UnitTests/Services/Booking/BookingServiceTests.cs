using AutoMapper;
using BookingApp.Application.Services.Booking;
using BookingApp.Application.Services.Inventory;
using BookingApp.Application.Services.Member;
using BookingApp.Domain.Dtos;
using Entity = BookingApp.Domain.Entities;
using BookingApp.Domain.Enums;
using BookingApp.Domain.Interfaces;
using BookingApp.Domain.Results;
using Moq;

namespace BookingApp.Application.UnitTests.Services.Booking;

public class BookingServiceTests
{
    /// <summary>
    /// Test Case 1: Successfully Create a Booking
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task CreateAsync_ShouldReturnSuccess_WhenBookingIsCreated()
    {
        // Arrange
        var mockMapper = new Mock<IMapper>();
        var mockBookingRepo = new Mock<IBookingRepository>();
        var mockMemberService = new Mock<IMemberService>();
        var mockInventoryService = new Mock<IInventoryService>();

        var member = new Entity.Member
        {
            Id = Guid.NewGuid(),
            Name = "John",
            SurName = "Doe",
            DateJoined = DateTime.Now,
            BookingCount = 1
        };

        var inventory = new Entity.Inventory
        {
            Id = Guid.NewGuid(),
            Title = "Test Inventory",
            Description = "Test Description",
            ExpirationDate = DateTime.Now.AddDays(5),
            RemainingCount = 5
        };

        var booking = new Entity.Booking
        {
            Id = Guid.NewGuid(),
            MemberId = member.Id,
            InventoryId = inventory.Id,
            BookingDateTime = DateTime.Now.AddDays(5),
            Status = BookingStatus.Active
        };

        mockMemberService.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(Result<Entity.Member>.Success(member));

        mockInventoryService.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(Result<Entity.Inventory?>.Success(inventory));

        mockMapper.Setup(x => x.Map<Entity.Booking>(It.IsAny<CreateBookingRequestDto>()))
            .Returns(booking);

        mockBookingRepo.Setup(x => x.InsertAsync(It.IsAny<Entity.Booking>()))
            .ReturnsAsync(booking);

        var bookingService = new BookingService(mockMapper.Object,
            mockBookingRepo.Object, mockMemberService.Object, mockInventoryService.Object);

        var requestDto = new CreateBookingRequestDto
        { 
            MemberId = member.Id, 
            InventoryId = inventory.Id,
            BookingDateTime = DateTime.Now.AddDays(5)
        };

        // Act
        var result = await bookingService.CreateAsync(requestDto);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(booking, result.Value);
    }

    /// <summary>
    /// Test Case 2: Fail to Create Booking - Member Not Found
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task CreateAsync_ShouldReturnNotFound_WhenMemberDoesNotExist()
    {
        // Arrange
        var mockMapper = new Mock<IMapper>();
        var mockBookingRepo = new Mock<IBookingRepository>();
        var mockMemberService = new Mock<IMemberService>();
        var mockInventoryService = new Mock<IInventoryService>();

        mockMemberService.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(Result<Entity.Member>.Fail(Error.NotFound("Member not found")));

        var bookingService = new BookingService(mockMapper.Object, 
            mockBookingRepo.Object, mockMemberService.Object, mockInventoryService.Object);

        var requestDto = new CreateBookingRequestDto 
        { 
            MemberId = Guid.NewGuid(),
            InventoryId = Guid.NewGuid(),
            BookingDateTime = DateTime.Now.AddDays(5)
        };

        // Act
        var result = await bookingService.CreateAsync(requestDto);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Member not found", result.Error!.Message);
    }

    /// <summary>
    /// Test Case 3: Fail to Create Booking - Member Reached Maximum Booking Limit
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task CreateAsync_ShouldReturnValidationError_WhenMemberReachedMaxBookingLimit()
    {
        // Arrange
        var mockMapper = new Mock<IMapper>();
        var mockBookingRepo = new Mock<IBookingRepository>();
        var mockMemberService = new Mock<IMemberService>();
        var mockInventoryService = new Mock<IInventoryService>();

        var member = new Entity.Member
        {
            Id = Guid.NewGuid(),
            Name = "John",
            SurName = "Doe",
            DateJoined = DateTime.Now,
            BookingCount = 2
        };

        mockMemberService.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(Result<Entity.Member>.Success(member));

        var bookingService = new BookingService(mockMapper.Object,
            mockBookingRepo.Object, mockMemberService.Object, mockInventoryService.Object);

        var requestDto = new CreateBookingRequestDto 
        { 
            MemberId = member.Id, 
            InventoryId = Guid.NewGuid(),
            BookingDateTime = DateTime.Now.AddDays(5)
        };

        // Act
        var result = await bookingService.CreateAsync(requestDto);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Member has reached the maximum booking limit of 2.", result.Error!.Message);
    }

    /// <summary>
    /// Test Case 4: Successfully Cancel a Booking
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task CancelAsync_ShouldReturnSuccess_WhenBookingIsCancelled()
    {
        // Arrange
        var mockMapper = new Mock<IMapper>();
        var mockBookingRepo = new Mock<IBookingRepository>();
        var mockMemberService = new Mock<IMemberService>();
        var mockInventoryService = new Mock<IInventoryService>();

        var booking = new Entity.Booking { Id = Guid.NewGuid(), Status = BookingStatus.Active };

        mockBookingRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(booking);

        mockBookingRepo.Setup(x => x.UpdateAsync(It.IsAny<Entity.Booking>()))
            .ReturnsAsync(booking);

        var bookingService = new BookingService(mockMapper.Object,
            mockBookingRepo.Object, mockMemberService.Object, mockInventoryService.Object);

        // Act
        var result = await bookingService.CancelAsync(booking.Id);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(BookingStatus.Cancelled, result.Value.Status);
    }

    /// <summary>
    /// Test Case 5: Fail to Cancel Booking - Booking Already Cancelled
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task CancelAsync_ShouldReturnBadRequest_WhenBookingIsAlreadyCancelled()
    {
        // Arrange
        var mockMapper = new Mock<IMapper>();
        var mockBookingRepo = new Mock<IBookingRepository>();
        var mockMemberService = new Mock<IMemberService>();
        var mockInventoryService = new Mock<IInventoryService>();

        var booking = new Entity.Booking { Id = Guid.NewGuid(), Status = BookingStatus.Cancelled };

        mockBookingRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(booking);

        var bookingService = new BookingService(mockMapper.Object,
            mockBookingRepo.Object, mockMemberService.Object, mockInventoryService.Object);

        // Act
        var result = await bookingService.CancelAsync(booking.Id);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Booking is already cancelled.", result.Error!.Message);
    }

    /// <summary>
    /// Test Case 6: Successfully Retrieve All Bookings
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetAllAsync_ShouldReturnAllBookings()
    {
        // Arrange
        var mockMapper = new Mock<IMapper>();
        var mockBookingRepo = new Mock<IBookingRepository>();
        var mockMemberService = new Mock<IMemberService>();
        var mockInventoryService = new Mock<IInventoryService>();

        var bookings = new List<Entity.Booking>
        {
            new() { Id = Guid.NewGuid(), Status = BookingStatus.Active },
            new() { Id = Guid.NewGuid(), Status = BookingStatus.Cancelled }
        };

        mockBookingRepo.Setup(x => x.GetAllAsync())
            .ReturnsAsync(bookings);

        var bookingService = new BookingService(mockMapper.Object,
            mockBookingRepo.Object, mockMemberService.Object, mockInventoryService.Object);

        // Act
        var result = await bookingService.GetAllAsync();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Count());
    }

    /// <summary>
    /// Test Case 7: Successfully Retrieve Booking by ID
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetByIdAsync_ShouldReturnBooking_WhenBookingExists()
    {
        // Arrange
        var mockMapper = new Mock<IMapper>();
        var mockBookingRepo = new Mock<IBookingRepository>();
        var mockMemberService = new Mock<IMemberService>();
        var mockInventoryService = new Mock<IInventoryService>();

        var booking = new Entity.Booking { Id = Guid.NewGuid(), Status = BookingStatus.Active };

        mockBookingRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(booking);

        var bookingService = new BookingService(mockMapper.Object,
            mockBookingRepo.Object, mockMemberService.Object, mockInventoryService.Object);

        // Act
        var result = await bookingService.GetByIdAsync(booking.Id);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(booking, result.Value);
    }

    /// <summary>
    /// Test Case 8: Fail to Retrieve Booking - Booking Not Found
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetByIdAsync_ShouldReturnNotFound_WhenBookingDoesNotExist()
    {
        // Arrange
        var mockMapper = new Mock<IMapper>();
        var mockBookingRepo = new Mock<IBookingRepository>();
        var mockMemberService = new Mock<IMemberService>();
        var mockInventoryService = new Mock<IInventoryService>();

        mockBookingRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Entity.Booking?)null);

        var bookingService = new BookingService(mockMapper.Object, 
            mockBookingRepo.Object, mockMemberService.Object, mockInventoryService.Object);

        // Act
        var result = await bookingService.GetByIdAsync(Guid.NewGuid());

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Booking not found", result.Error!.Message);
    }
}