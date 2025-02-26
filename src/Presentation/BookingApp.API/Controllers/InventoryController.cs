using BookingApp.Application.Queries.Inventory;
using BookingApp.Application.Services.Inventory;
using BookingApp.Domain.CsvClassMaps;
using BookingApp.Domain.Dtos;
using CsvHelper;
using CsvHelper.TypeConversion;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace BookingApp.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class InventoryController : ControllerBase
{
    private readonly List<string> ExpectedHeaders = ["title", "description", "remaining_count", "expiration_date"];
    private readonly IInventoryService _inventoryService;
    private readonly IGetInventoryByIdQueryHandler _getInventoryByIdQueryHandler;

    public InventoryController(IInventoryService inventoryService,
        IGetInventoryByIdQueryHandler getInventoryByIdQueryHandler)
    {
        _inventoryService = inventoryService;
        _getInventoryByIdQueryHandler = getInventoryByIdQueryHandler;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var result = await _inventoryService.GetAllAsync();
        if (result.IsFailure)
        {
            return BadRequest(result.Error!.Message);
        }

        return Ok(result.Value);
    }

    [HttpGet]
    [Route("{id:Guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        if (id == Guid.Empty)
        {
            return BadRequest("Inventory id is null or empty.");
        }

        var result = await _getInventoryByIdQueryHandler.Handle(id);
        if (result.IsFailure)
        {
            return NotFound(result.Error!.Message);
        }

        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateInventoryRequestDto request)
    {
        if (request is null)
        {
            return BadRequest("Request is null or empty.");
        }

        var result = await _inventoryService.CreateAsync(request);
        if (result.IsFailure)
        {
            return BadRequest(result.Error!.Message);
        }

        return Ok(result.Value);
    }

    [HttpPost]
    [Route("upload")]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file is null || Path.GetExtension(file.FileName).ToLower() != ".csv")
        {
            return BadRequest("Invalid file format. Please upload a CSV file.");
        }

        using var stream = new StreamReader(file.OpenReadStream());
        using var csv = new CsvReader(stream, CultureInfo.InvariantCulture);
        csv.Read();
        csv.ReadHeader();

        if (csv.HeaderRecord is not null && !csv.HeaderRecord.SequenceEqual(ExpectedHeaders))
        {
            return BadRequest("CSV headers do not match expected format.");
        }

        csv.Context.RegisterClassMap<BulkCreateInventoryRequestDtoMap>();

        List<BulkCreateInventoryRequestDto> inventories;

        try
        {
            inventories = csv.GetRecords<BulkCreateInventoryRequestDto>()
                .Where(x => !string.IsNullOrEmpty(x.Title) &&
                            !string.IsNullOrEmpty(x.Description)).ToList();
        }
        catch (TypeConverterException ex)
        {
            Console.WriteLine(ex.Message);
            return BadRequest("CSV file contains data conversion errors. Please check the format of the data.");
        }

        if (inventories.Count == 0)
        {
            return BadRequest("No records found in the CSV file.");
        }

        var result = await _inventoryService.BulkCreate(inventories);
        if (result.IsFailure)
        {
            return BadRequest(result.Error!.Message);
        }

        return Ok(result.Value);
    }
}