using BookingApp.Application.Queries.Booking;
using BookingApp.Application.Queries.Inventory;
using BookingApp.Application.Queries.Member;
using BookingApp.Application.Services.Booking;
using BookingApp.Application.Services.Inventory;
using BookingApp.Application.Services.Member;
using BookingApp.Domain.Configs;
using BookingApp.Domain.Interfaces;
using BookingApp.Persistence.Database;
using BookingApp.Persistence.Queries;
using BookingApp.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Automapper
builder.Services.AddAutoMapper(typeof(DomainAutoMapperProfile));

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<IMemberService, MemberService>();

builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
builder.Services.AddScoped<IMemberRepository, MemberRepository>();

builder.Services.AddScoped<IGetBookingByIdQueryHandler, GetBookingByIdQueryHandler>();
builder.Services.AddScoped<IGetInventoryByIdQueryHandler, GetInventoryByIdQueryHandler>();
builder.Services.AddScoped<IGetMemberByIdQueryHandler, GetMemberByIdQueryHandler>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    string databaseName = builder.Configuration.GetConnectionString("InMemoryDatabaseConnection") ?? throw new InvalidOperationException("InMemoryDatabaseConnection is not set");
    options.UseInMemoryDatabase(databaseName);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();