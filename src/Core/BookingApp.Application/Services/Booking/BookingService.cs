using AutoMapper;
using BookingApp.Application.Services.Inventory;
using BookingApp.Application.Services.Member;
using BookingApp.Domain.Dtos;
using BookingApp.Domain.Enums;
using BookingApp.Domain.Interfaces;
using BookingApp.Domain.Results;
using Entity = BookingApp.Domain.Entities;

namespace BookingApp.Application.Services.Booking;

public class BookingService : IBookingService
{
    private const int BOOKING_COUNT_THRESHOLD = 2;

    private readonly IMapper _mapper;
    private readonly IBookingRepository _bookingRepository;
    private readonly IMemberService _memberService;
    private readonly IInventoryService _inventoryService;

    public BookingService(IMapper mapper,
        IBookingRepository bookingRepository,
        IMemberService memberService,
        IInventoryService inventoryService)
    {
        _mapper = mapper;
        _bookingRepository = bookingRepository;
        _memberService = memberService;
        _inventoryService = inventoryService;
    }

    public async Task<Result<Entity.Booking>> CreateAsync(CreateBookingRequestDto dto)
    {
        var memberResult = await _memberService.GetByIdAsync(dto.MemberId);
        if (memberResult.IsFailure)
        {
            return Result<Entity.Booking>.Fail(Error.NotFound("Member not found"));
        }

        if (memberResult.Value.BookingCount >= BOOKING_COUNT_THRESHOLD)
        {
            return Result<Entity.Booking>.Fail(Error.ValidationError("Member has reached the maximum booking limit of 2."));
        }

        var inventoryResult = await _inventoryService.GetByIdAsync(dto.InventoryId);
        if (inventoryResult.IsFailure)
        {

            return Result<Entity.Booking>.Fail(Error.NotFound("Inventory item not found"));
        }

        if (inventoryResult.Value!.RemainingCount <= 0)
        {
            return Result<Entity.Booking>.Fail(Error.ValidationError("No remaining items available for booking."));
        }

        // Update member booking count
        var bookingCount = memberResult.Value.BookingCount += 1;
        await _memberService.UpdateBookingCountAsync(memberResult.Value.Id, bookingCount);

        // Update inventory remaining count
        var remainingCount = inventoryResult.Value.RemainingCount -= 1;
        await _inventoryService.UpdateRemainingCountAsync(inventoryResult.Value.Id, remainingCount);

        var booking = _mapper.Map<Entity.Booking>(dto);
        booking.Status = BookingStatus.Active;

        var result = await _bookingRepository.InsertAsync(booking);

        return Result<Entity.Booking>.Success(result);
    }

    public async Task<Result<Entity.Booking>> CancelAsync(Guid bookingId)
    {
        var booking = await _bookingRepository.GetByIdAsync(bookingId) ?? throw new ApplicationException("Booking not found");
        if (booking.Status == BookingStatus.Cancelled)
        {
            return Result<Entity.Booking>.Fail(Error.BadRequest("Booking is already cancelled."));
        }
        
        booking.Status = BookingStatus.Cancelled;

        var result = await _bookingRepository.UpdateAsync(booking);

        return Result<Entity.Booking>.Success(result);
    }

    public async Task<Result<IEnumerable<Entity.Booking>>> GetAllAsync()
    {
        var bookings = await _bookingRepository.GetAllAsync();

        return Result<IEnumerable<Entity.Booking>>.Success(bookings);
    }

    public async Task<Result<Entity.Booking?>> GetByIdAsync(Guid bookingId)
    {
        var booking = await _bookingRepository.GetByIdAsync(bookingId);
        if (booking is not null)
        {
            return Result<Entity.Booking?>.Success(booking);
        }

        return Result<Entity.Booking?>.Fail(Error.NotFound("Booking not found"));
    }
}