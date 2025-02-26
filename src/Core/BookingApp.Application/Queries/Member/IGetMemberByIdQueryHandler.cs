using BookingApp.Domain.Results;

namespace BookingApp.Application.Queries.Member;

public interface IGetMemberByIdQueryHandler
{
    Task<Result<MemberResponse?>> Handle(Guid id);
}