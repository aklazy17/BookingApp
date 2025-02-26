using BookingApp.Application.Queries.Booking;
using BookingApp.Application.Services.Booking;
using BookingApp.Domain.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace BookingApp.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BookController : ControllerBase
{
    private readonly IBookingService _bookingService;
    private readonly IGetBookingByIdQueryHandler _getBookingByIdQueryHandler;

    public BookController(IBookingService bookingService,
        IGetBookingByIdQueryHandler getBookingByIdQueryHandler)
    {
        _bookingService = bookingService;
        _getBookingByIdQueryHandler = getBookingByIdQueryHandler;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var result = await _bookingService.GetAllAsync();
        if (result.IsFailure)
        {
            return BadRequest(result.Error!.Message);
        }

        return Ok(result.Value);
    }

    [HttpGet]
    [Route("{bookingId}")]
    public async Task<IActionResult> GetById(Guid bookingId)
    {
        var result = await _getBookingByIdQueryHandler.Handle(bookingId);
        if (result.IsFailure)
        {
            return NotFound(result.Error!.Message);
        }

        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateBookingRequestDto request)
    {
        if (request is null)
        {
            return BadRequest("Request is null or empty.");
        }

        var result = await _bookingService.CreateAsync(request);
        if (result.IsFailure)
        {
            return BadRequest(result.Error!.Message);
        }

        return Ok(result.Value);
    }

    [HttpPut]
    [Route("cancel/{bookingId}")]
    public async Task<IActionResult> Cancel(Guid bookingId)
    {
        var result = await _bookingService.CancelAsync(bookingId);
        if (result.IsFailure)
        {
            return BadRequest(result.Error!.Message);
        }

        return Ok(result.Value);
    }
}