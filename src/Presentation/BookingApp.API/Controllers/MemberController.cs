using BookingApp.Application.Queries.Member;
using BookingApp.Application.Services.Member;
using BookingApp.Domain.CsvClassMaps;
using BookingApp.Domain.Dtos;
using CsvHelper;
using CsvHelper.TypeConversion;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace BookingApp.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MemberController : ControllerBase
{
    private readonly List<string> ExpectedHeaders = ["name", "surname", "booking_count", "date_joined"];
    private readonly IMemberService _memberService;
    private readonly IGetMemberByIdQueryHandler _getMemberByIdQueryHandler;

    public MemberController(IMemberService memberService,
        IGetMemberByIdQueryHandler getMemberByIdQueryHandler)
    {
        _memberService = memberService;
        _getMemberByIdQueryHandler = getMemberByIdQueryHandler;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var result = await _memberService.GetAllAsync();
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
            return BadRequest("Member id is null or empty.");
        }

        var result = await _getMemberByIdQueryHandler.Handle(id);
        if (result.IsFailure)
        {
            return NotFound(result.Error!.Message);
        }

        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateMemberRequestDto request)
    {
        if (request is null)
        {
            return BadRequest("Request is null or empty.");
        }

        var result = await _memberService.CreateAsync(request);
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

        csv.Context.RegisterClassMap<BulkCreateMemberRequestDtoMap>();

        List<BulkCreateMemberRequestDto> members;

        try
        {
            members = csv.GetRecords<BulkCreateMemberRequestDto>()
               .Where(x => !string.IsNullOrEmpty(x.Name)).ToList();
        }
        catch (TypeConverterException ex)
        {
            Console.WriteLine(ex.Message);
            return BadRequest("CSV file contains data conversion errors. Please check the format of the data.");
        }

        if (members.Count == 0)
        {
            return BadRequest("No records found in the CSV file.");
        }

        var result = await _memberService.BulkCreate(members);
        if (result.IsFailure)
        {
            return BadRequest(result.Error!.Message);
        }

        return Ok(result.Value);
    }
}