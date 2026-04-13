using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PickC.Modules.Booking.Application.Services;
using PickC.Modules.Booking.Domain.Interfaces;
using PickC.Modules.Booking.Infrastructure.Data;
using PickC.Modules.Booking.Infrastructure.Repositories;

namespace PickC.Modules.Booking;

public static class BookingModule
{
    public static IServiceCollection AddBookingModule(
        this IServiceCollection services, IConfiguration configuration)
    {
        // DbContext
        services.AddDbContext<BookingDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("PickC"),
                sql => sql.MigrationsHistoryTable("__EFMigrationsHistory", "Operation")));

        // Repositories
        services.AddScoped<IBookingRepository, BookingRepository>();

        // Application Services
        services.AddScoped<IBookingService, BookingService>();

        // Validators
        services.AddValidatorsFromAssembly(typeof(BookingModule).Assembly);

        return services;
    }
}
