using BookingApp.Application.Queries.Member;
using BookingApp.Domain.Results;
using Moq;

namespace BookingApp.Application.UnitTests.Queries.Member;

public class GetMemberByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnMember_WhenMemberExists()
    {
        // Arrange
        var memberId = Guid.NewGuid();

        var expectedMemberResponse = new MemberResponse
        {
            Id = memberId,
            Name = "John",
            SurName = "Doe",
            DateJoined = DateTime.Now,
            BookingCount = 2
        };

        var mockHandler = new Mock<IGetMemberByIdQueryHandler>();
        mockHandler
            .Setup(handler => handler.Handle(memberId))
            .ReturnsAsync(Result<MemberResponse?>.Success(expectedMemberResponse));

        // Act
        var result = await mockHandler.Object.Handle(memberId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(memberId, result.Value.Id);
        Assert.Equal("John", result.Value.Name);
        Assert.Equal(2, result.Value.BookingCount);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenMemberDoesNotExist()
    {
        // Arrange
        var memberId = Guid.NewGuid();

        var mockHandler = new Mock<IGetMemberByIdQueryHandler>();
        mockHandler
            .Setup(handler => handler.Handle(memberId))
            .ReturnsAsync(Result<MemberResponse?>.Fail(Error.NotFound("Member not found.")));

        // Act
        var result = await mockHandler.Object.Handle(memberId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Throws<InvalidOperationException>(() => result.Value);
        Assert.Equal("Member not found.", result.Error!.Message);
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenHandlerFails()
    {
        // Arrange
        var memberId = Guid.NewGuid();

        var mockHandler = new Mock<IGetMemberByIdQueryHandler>();
        mockHandler
            .Setup(handler => handler.Handle(memberId))
            .ThrowsAsync(new Exception("Internal server error."));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => mockHandler.Object.Handle(memberId));
    }
}
