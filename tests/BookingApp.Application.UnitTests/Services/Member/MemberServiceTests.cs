using AutoMapper;
using BookingApp.Application.Services.Member;
using BookingApp.Domain.Dtos;
using BookingApp.Domain.Interfaces;
using Entity = BookingApp.Domain.Entities;
using Moq;

namespace BookingApp.Application.UnitTests.Services.Member;

public class MemberServiceTests
{
    /// <summary>
    /// Test Case 1: Successfully Create a Member
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task CreateAsync_ShouldReturnSuccess_WhenMemberIsCreated()
    {
        // Arrange
        var mockMapper = new Mock<IMapper>();
        var mockMemberRepo = new Mock<IMemberRepository>();

        var requestDto = new CreateMemberRequestDto { Name = "Sophie", SurName = "Davis" };
        var member = new Entity.Member { Id = Guid.NewGuid(), Name = "Sophie", SurName = "Davis", BookingCount = 0 };

        mockMapper.Setup(x => x.Map<Entity.Member>(requestDto))
            .Returns(member);

        mockMemberRepo.Setup(x => x.InsertAsync(member))
            .ReturnsAsync(member);

        var memberService = new MemberService(mockMapper.Object, mockMemberRepo.Object);

        // Act
        var result = await memberService.CreateAsync(requestDto);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(member, result.Value);
    }

    /// <summary>
    /// Test Case 2: Fail to Create Member - Mapping Error
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task CreateAsync_ShouldThrowException_WhenMappingFails()
    {
        // Arrange
        var mockMapper = new Mock<IMapper>();
        var mockMemberRepo = new Mock<IMemberRepository>();

        var requestDto = new CreateMemberRequestDto { Name = "Sophie", SurName = "Davis" };

        mockMapper.Setup(x => x.Map<Entity.Member>(requestDto))
            .Throws(new Exception("Mapping failed."));

        var memberService = new MemberService(mockMapper.Object, mockMemberRepo.Object);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => memberService.CreateAsync(requestDto));
    }

    /// <summary>
    /// Test Case 3: Successfully Bulk Create Members
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task BulkCreate_ShouldReturnSuccess_WhenMembersAreCreated()
    {
        // Arrange
        var mockMapper = new Mock<IMapper>();
        var mockMemberRepo = new Mock<IMemberRepository>();

        var dtos = new List<BulkCreateMemberRequestDto>
        {
            new() { Name = "Sophie", SurName = "Davis", BookingCount = 1 },
            new() { Name = "Emily", SurName = "Johnson", BookingCount = 0 }
        };

        var members = new List<Entity.Member>
        {
            new() { Id = Guid.NewGuid(), Name = "Sophie", SurName = "Davis", BookingCount = 1 },
            new() { Id = Guid.NewGuid(), Name = "Emily", SurName = "Johnson", BookingCount = 0 }
        };

        mockMapper.Setup(x => x.Map<List<Entity.Member>>(dtos))
            .Returns(members);

        mockMemberRepo.Setup(x => x.BulkInsertAsync(members))
            .ReturnsAsync(members);

        var memberService = new MemberService(mockMapper.Object, mockMemberRepo.Object);

        // Act
        var result = await memberService.BulkCreate(dtos);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Count);
    }

    /// <summary>
    /// Test Case 4: Successfully Retrieve All Members
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetAllAsync_ShouldReturnAllMembers()
    {
        // Arrange
        var mockMapper = new Mock<IMapper>();
        var mockMemberRepo = new Mock<IMemberRepository>();

        var members = new List<Entity.Member>
        {
            new() { Id = Guid.NewGuid(), Name = "Sophie", SurName = "Davis", BookingCount = 1 },
            new() { Id = Guid.NewGuid(), Name = "Emily", SurName = "Johnson", BookingCount = 0 }
        };

        mockMemberRepo.Setup(x => x.GetAllAsync())
            .ReturnsAsync(members);

        var memberService = new MemberService(mockMapper.Object, mockMemberRepo.Object);

        // Act
        var result = await memberService.GetAllAsync();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Count());
    }

    /// <summary>
    /// Test Case 5: Successfully Retrieve Member by ID
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetByIdAsync_ShouldReturnSuccess_WhenMemberExists()
    {
        // Arrange
        var mockMapper = new Mock<IMapper>();
        var mockMemberRepo = new Mock<IMemberRepository>();

        var memberId = Guid.NewGuid();
        var member = new Entity.Member { Id = memberId, Name = "Sophie", SurName = "Davis", BookingCount = 0 };

        mockMemberRepo.Setup(x => x.GetByIdAsync(memberId))
            .ReturnsAsync(member);

        var memberService = new MemberService(mockMapper.Object, mockMemberRepo.Object);

        // Act
        var result = await memberService.GetByIdAsync(memberId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(member, result.Value);
    }

    /// <summary>
    /// Test Case 6: Fail to Retrieve Member - Member Not Found
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetByIdAsync_ShouldReturnNotFound_WhenMemberDoesNotExist()
    {
        // Arrange
        var mockMapper = new Mock<IMapper>();
        var mockMemberRepo = new Mock<IMemberRepository>();

        var memberId = Guid.NewGuid();

        mockMemberRepo.Setup(x => x.GetByIdAsync(memberId))
            .ReturnsAsync((Entity.Member?)null);

        var memberService = new MemberService(mockMapper.Object, mockMemberRepo.Object);

        // Act
        var result = await memberService.GetByIdAsync(memberId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Member not found.", result.Error!.Message);
    }

    /// <summary>
    /// Test Case 7: Successfully Update Booking Count
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task UpdateBookingCountAsync_ShouldReturnSuccess_WhenMemberIsUpdated()
    {
        // Arrange
        var mockMapper = new Mock<IMapper>();
        var mockMemberRepo = new Mock<IMemberRepository>();

        var memberId = Guid.NewGuid();
        var member = new Entity.Member { Id = memberId, Name = "Sophie", SurName = "Davis", BookingCount = 0 };

        mockMemberRepo.Setup(x => x.GetByIdAsync(memberId))
            .ReturnsAsync(member);

        mockMemberRepo.Setup(x => x.UpdateAsync(member))
            .ReturnsAsync(member);

        var memberService = new MemberService(mockMapper.Object, mockMemberRepo.Object);

        // Act
        var result = await memberService.UpdateBookingCountAsync(memberId, 2);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.BookingCount);
    }

    /// <summary>
    /// Test Case 8: Fail to Update Booking Count - Member Not Found
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task UpdateBookingCountAsync_ShouldReturnNotFound_WhenMemberDoesNotExist()
    {
        // Arrange
        var mockMapper = new Mock<IMapper>();
        var mockMemberRepo = new Mock<IMemberRepository>();

        var memberId = Guid.NewGuid();

        mockMemberRepo.Setup(x => x.GetByIdAsync(memberId))
            .ReturnsAsync((Entity.Member?)null);

        var memberService = new MemberService(mockMapper.Object, mockMemberRepo.Object);

        // Act
        var result = await memberService.UpdateBookingCountAsync(memberId, 2);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Member not found.", result.Error!.Message);
    }
}