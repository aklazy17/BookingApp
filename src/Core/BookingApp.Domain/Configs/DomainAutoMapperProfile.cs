using AutoMapper;
using BookingApp.Domain.Dtos;
using BookingApp.Domain.Entities;

namespace BookingApp.Domain.Configs;

public class DomainAutoMapperProfile : Profile
{
    public DomainAutoMapperProfile()
    {
        CreateMap<CreateBookingRequestDto, Booking>();

        CreateMap<CreateInventoryRequestDto, Inventory>();
        CreateMap<BulkCreateInventoryRequestDto, Inventory>();

        CreateMap<CreateMemberRequestDto, Member>();
        CreateMap<BulkCreateMemberRequestDto, Member>();
    }
}