using AutoMapper;
using BookingApp.Domain.Dtos;
using BookingApp.Domain.Interfaces;
using BookingApp.Domain.Results;
using Entity = BookingApp.Domain.Entities;

namespace BookingApp.Application.Services.Member;

public class MemberService : IMemberService
{
    private readonly IMapper _mapper;
    private readonly IMemberRepository _memberRepository;

    public MemberService(IMapper mapper,
        IMemberRepository memberRepository)
    {
        _mapper = mapper;
        _memberRepository = memberRepository;
    }

    public async Task<Result<Entity.Member>> CreateAsync(CreateMemberRequestDto dto)
    {
        var member = _mapper.Map<Entity.Member>(dto);
        var result = await _memberRepository.InsertAsync(member);

        return Result<Entity.Member>.Success(result);
    }

    public async Task<Result<List<Entity.Member>>> BulkCreate(List<BulkCreateMemberRequestDto> dtos)
    {
        var members = _mapper.Map<List<Entity.Member>>(dtos);
        var result = await _memberRepository.BulkInsertAsync(members);

        return Result<List<Entity.Member>>.Success(result);
    }

    public async Task<Result<IEnumerable<Entity.Member>>> GetAllAsync()
    {
        var result = await _memberRepository.GetAllAsync();

        return Result<IEnumerable<Entity.Member>>.Success(result);
    }

    public async Task<Result<Entity.Member>> GetByIdAsync(Guid memberId)
    {
        var member = await _memberRepository.GetByIdAsync(memberId);
        if (member is not null)
        {
            return Result<Entity.Member>.Success(member);
        }

        return Result<Entity.Member>.Fail(Error.NotFound("Member not found."));
    }

    public async Task<Result<Entity.Member>> UpdateBookingCountAsync(Guid memberId, int bookingCount)
    {
        var member = await _memberRepository.GetByIdAsync(memberId);
        if (member is not null)
        {
            member.BookingCount = bookingCount;

            var result = await _memberRepository.UpdateAsync(member);
            if (result is not null)
            {
                return Result<Entity.Member>.Success(member);
            }
        }

        return Result<Entity.Member>.Fail(Error.NotFound("Member not found."));
    }
}