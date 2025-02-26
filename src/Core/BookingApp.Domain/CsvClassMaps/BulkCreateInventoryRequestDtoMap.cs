using BookingApp.Domain.Dtos;
using CsvHelper.Configuration;

namespace BookingApp.Domain.CsvClassMaps;

public class BulkCreateInventoryRequestDtoMap : ClassMap<BulkCreateInventoryRequestDto>
{
    public BulkCreateInventoryRequestDtoMap()
    {
        Map(m => m.Title).Name("title");
        Map(m => m.Description).Name("description");
        Map(m => m.RemainingCount).Name("remaining_count");
        Map(m => m.ExpirationDate).Name("expiration_date").TypeConverterOption.Format("dd-MM-yyyy");
    }
}