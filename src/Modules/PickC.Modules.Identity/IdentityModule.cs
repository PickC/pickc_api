using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PickC.Modules.Identity.Application.Services;
using PickC.Modules.Identity.Domain.Interfaces;
using PickC.Modules.Identity.Infrastructure.Data;
using PickC.Modules.Identity.Infrastructure.Repositories;

namespace PickC.Modules.Identity;

public static class IdentityModule
{
    public static IServiceCollection AddIdentityModule(
        this IServiceCollection services, IConfiguration configuration)
    {
        // DbContext
        services.AddDbContext<IdentityDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("PickC"),
                sql => sql.MigrationsHistoryTable("__EFMigrationsHistory", "Security")));

        // JWT Settings
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

        // Services
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IOtpService, OtpService>();

        // Repositories
        services.AddScoped<IAuthRepository, AuthRepository>();

        // Validators
        services.AddValidatorsFromAssembly(typeof(IdentityModule).Assembly);

        return services;
    }
}
