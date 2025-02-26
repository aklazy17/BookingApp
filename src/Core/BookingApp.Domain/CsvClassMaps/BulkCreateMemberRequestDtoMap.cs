using BookingApp.Domain.Dtos;
using CsvHelper.Configuration;

namespace BookingApp.Domain.CsvClassMaps;

public class BulkCreateMemberRequestDtoMap : ClassMap<BulkCreateMemberRequestDto>
{
    public BulkCreateMemberRequestDtoMap()
    {
        Map(m => m.Name).Name("name");
        Map(m => m.SurName).Name("surname");
        Map(m => m.BookingCount).Name("booking_count");
        Map(m => m.DateJoined).Name("date_joined");
    }
}