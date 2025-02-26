using BookingApp.Application.Queries.Member;
using BookingApp.Domain.Results;
using BookingApp.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace BookingApp.Persistence.Queries;

public class GetMemberByIdQueryHandler : IGetMemberByIdQueryHandler
{
    private readonly ApplicationDbContext _dbContext;
    public GetMemberByIdQueryHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<Result<MemberResponse?>> Handle(Guid id)
    {
        var memberResponse = await _dbContext.Members
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new MemberResponse
            {
                Id = x.Id,
                Name = x.Name,
                SurName = x.SurName,
                BookingCount = x.BookingCount,
                DateJoined = x.DateJoined
            })
            .FirstOrDefaultAsync();

        if (memberResponse is null)
        {
            return Result<MemberResponse?>.Fail(Error.NotFound($"Member with id {id} not found."));
        }

        return Result<MemberResponse?>.Success(memberResponse);
    }
}
