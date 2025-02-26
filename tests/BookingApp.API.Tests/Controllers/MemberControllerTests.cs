using BookingApp.API.Controllers;
using BookingApp.Application.Queries.Member;
using BookingApp.Application.Services.Member;
using BookingApp.Domain.Dtos;
using BookingApp.Domain.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Text;
using Entity = BookingApp.Domain.Entities;

namespace BookingApp.API.Tests.Controllers;

public class MemberControllerTests
{
    /// <summary>
    /// Test Case 1: Successfully Retrieve All Members
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task Get_ShouldReturnOk_WhenMembersExist()
    {
        // Arrange
        var mockMemberService = new Mock<IMemberService>();
        var mockGetMemberByIdQueryHandler = new Mock<IGetMemberByIdQueryHandler>();

        var members = new List<Entity.Member>
        {
            new() { Id = Guid.NewGuid(), Name = "Sophie", SurName = "Davis", BookingCount = 0 },
            new() { Id = Guid.NewGuid(), Name = "Emily", SurName = "Johnson", BookingCount = 0 }
        };

        mockMemberService.Setup(x => x.GetAllAsync())
            .ReturnsAsync(Result<IEnumerable<Entity.Member>>.Success(members));

        var controller = new MemberController(mockMemberService.Object, mockGetMemberByIdQueryHandler.Object);

        // Act
        var result = await controller.Get();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedMembers = Assert.IsType<List<Entity.Member>>(okResult.Value);
        Assert.Equal(2, returnedMembers.Count);
    }

    /// <summary>
    /// Test Case 2: Fail to Retrieve All Members
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task Get_ShouldReturnBadRequest_WhenMembersFailToRetrieve()
    {
        // Arrange
        var mockMemberService = new Mock<IMemberService>();
        var mockGetMemberByIdQueryHandler = new Mock<IGetMemberByIdQueryHandler>();

        mockMemberService.Setup(x => x.GetAllAsync())
            .ReturnsAsync(Result<IEnumerable<Entity.Member>>.Fail(Error.ValidationError("Failed to retrieve members.")));

        var controller = new MemberController(mockMemberService.Object, mockGetMemberByIdQueryHandler.Object);

        // Act
        var result = await controller.Get();

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Failed to retrieve members.", badRequestResult.Value);
    }

    /// <summary>
    /// Test Case 3: Successfully Retrieve Member by ID
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetById_ShouldReturnOk_WhenMemberExists()
    {
        // Arrange
        var mockMemberService = new Mock<IMemberService>();
        var mockGetMemberByIdQueryHandler = new Mock<IGetMemberByIdQueryHandler>();

        var memberId = Guid.NewGuid();
        var member = new MemberResponse { Id = memberId, Name = "Sophie", SurName = "Davis", BookingCount = 0 };

        mockGetMemberByIdQueryHandler.Setup(x => x.Handle(memberId))
            .ReturnsAsync(Result<MemberResponse?>.Success(member));

        var controller = new MemberController(mockMemberService.Object, mockGetMemberByIdQueryHandler.Object);

        // Act
        var result = await controller.Get(memberId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedMember = Assert.IsType<MemberResponse>(okResult.Value);
        Assert.Equal(memberId, returnedMember.Id);
    }

    /// <summary>
    /// Test Case 4: Fail to Retrieve Member - Member Not Found
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenMemberDoesNotExist()
    {
        // Arrange
        var mockMemberService = new Mock<IMemberService>();
        var mockGetMemberByIdQueryHandler = new Mock<IGetMemberByIdQueryHandler>();

        var memberId = Guid.NewGuid();

        mockGetMemberByIdQueryHandler.Setup(x => x.Handle(memberId))
            .ReturnsAsync(Result<MemberResponse?>.Fail(Error.NotFound("Member not found.")));

        var controller = new MemberController(mockMemberService.Object, mockGetMemberByIdQueryHandler.Object);

        // Act
        var result = await controller.Get(memberId);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Member not found.", notFoundResult.Value);
    }

    /// <summary>
    /// Test Case 5: Fail to Retrieve Member - Invalid ID
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetById_ShouldReturnBadRequest_WhenIdIsEmpty()
    {
        // Arrange
        var mockMemberService = new Mock<IMemberService>();
        var mockGetMemberByIdQueryHandler = new Mock<IGetMemberByIdQueryHandler>();

        var controller = new MemberController(mockMemberService.Object, mockGetMemberByIdQueryHandler.Object);

        // Act
        var result = await controller.Get(Guid.Empty);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Member id is null or empty.", badRequestResult.Value);
    }

    /// <summary>
    /// Test Case 6: Successfully Create a Member
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task Create_ShouldReturnOk_WhenMemberIsCreated()
    {
        // Arrange
        var mockMemberService = new Mock<IMemberService>();
        var mockGetMemberByIdQueryHandler = new Mock<IGetMemberByIdQueryHandler>();

        var request = new CreateMemberRequestDto { Name = "Sophie", SurName = "Davis" };
        var member = new Entity.Member { Id = Guid.NewGuid(), Name = "Sophie", SurName = "Davis", BookingCount = 0 };

        mockMemberService.Setup(x => x.CreateAsync(request))
            .ReturnsAsync(Result<Entity.Member>.Success(member));

        var controller = new MemberController(mockMemberService.Object, mockGetMemberByIdQueryHandler.Object);

        // Act
        var result = await controller.Create(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedMember = Assert.IsType<Entity.Member>(okResult.Value);
        Assert.Equal(member.Id, returnedMember.Id);
    }

    /// <summary>
    /// Test Case 7: Fail to Create Member - Invalid Request
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task Create_ShouldReturnBadRequest_WhenRequestIsNull()
    {
        // Arrange
        var mockMemberService = new Mock<IMemberService>();
        var mockGetMemberByIdQueryHandler = new Mock<IGetMemberByIdQueryHandler>();

        var controller = new MemberController(mockMemberService.Object, mockGetMemberByIdQueryHandler.Object);

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
        var mockMemberService = new Mock<IMemberService>();
        var mockGetMemberByIdQueryHandler = new Mock<IGetMemberByIdQueryHandler>();

        var csvContent = "name,surname,booking_count,date_joined\nSophie,Davis,0,2024-01-02T12:10:11";
        var csvFile = new Mock<IFormFile>();
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));

        csvFile.Setup(x => x.OpenReadStream()).Returns(stream);
        csvFile.Setup(x => x.FileName).Returns("members.csv");

        var members = new List<Entity.Member>
        {
            new() { Id = Guid.NewGuid(), Name = "Sophie", SurName = "Davis", BookingCount = 0 }
        };

        mockMemberService.Setup(x => x.BulkCreate(It.IsAny<List<BulkCreateMemberRequestDto>>()))
            .ReturnsAsync(Result<List<Entity.Member>>.Success(members));

        var controller = new MemberController(mockMemberService.Object, mockGetMemberByIdQueryHandler.Object);

        // Act
        var result = await controller.Upload(csvFile.Object);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedMembers = Assert.IsType<List<Entity.Member>>(okResult.Value);
        Assert.Single(returnedMembers);
    }

    /// <summary>
    /// Test Case 9: Fail to Upload CSV File - Invalid File Format
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task Upload_ShouldReturnBadRequest_WhenFileIsNotCsv()
    {
        // Arrange
        var mockMemberService = new Mock<IMemberService>();
        var mockGetMemberByIdQueryHandler = new Mock<IGetMemberByIdQueryHandler>();

        var csvFile = new Mock<IFormFile>();
        csvFile.Setup(x => x.FileName).Returns("members.txt");

        var controller = new MemberController(mockMemberService.Object, mockGetMemberByIdQueryHandler.Object);

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
        var mockMemberService = new Mock<IMemberService>();
        var mockGetMemberByIdQueryHandler = new Mock<IGetMemberByIdQueryHandler>();

        var csvContent = "invalid_header1,invalid_header2\nSophie,Davis";
        var csvFile = new Mock<IFormFile>();
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));

        csvFile.Setup(x => x.OpenReadStream()).Returns(stream);
        csvFile.Setup(x => x.FileName).Returns("members.csv");

        var controller = new MemberController(mockMemberService.Object, mockGetMemberByIdQueryHandler.Object);

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
        var mockMemberService = new Mock<IMemberService>();
        var mockGetMemberByIdQueryHandler = new Mock<IGetMemberByIdQueryHandler>();

        var csvContent = "name,surname,booking_count,date_joined\nSophie,Davis,0,invalid_date";
        var csvFile = new Mock<IFormFile>();
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));

        csvFile.Setup(x => x.OpenReadStream()).Returns(stream);
        csvFile.Setup(x => x.FileName).Returns("members.csv");

        var controller = new MemberController(mockMemberService.Object, mockGetMemberByIdQueryHandler.Object);

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
        var mockMemberService = new Mock<IMemberService>();
        var mockGetMemberByIdQueryHandler = new Mock<IGetMemberByIdQueryHandler>();

        var csvContent = "name,surname,booking_count,date_joined\n";
        var csvFile = new Mock<IFormFile>();
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));

        csvFile.Setup(x => x.OpenReadStream()).Returns(stream);
        csvFile.Setup(x => x.FileName).Returns("members.csv");

        var controller = new MemberController(mockMemberService.Object, mockGetMemberByIdQueryHandler.Object);

        // Act
        var result = await controller.Upload(csvFile.Object);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("No records found in the CSV file.", badRequestResult.Value);
    }
}