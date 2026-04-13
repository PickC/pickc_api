using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using PickC.Infrastructure.Middleware;
using PickC.Infrastructure.Extensions;
using PickC.Modules.Identity;
using PickC.Modules.Identity.Application.Services;
using PickC.Modules.Master;
using PickC.Modules.Booking;
using PickC.Modules.Trip;
using PickC.Modules.Billing;
using PickC.Modules.Notification;
using PickC.Modules.Reports;
using PickC.Api.Hubs;

var builder = WebApplication.CreateBuilder(args);

var isDevelopment = builder.Environment.IsDevelopment();

// Serilog
builder.Host.UseSerilog((context, config) =>
    config.ReadFrom.Configuration(context.Configuration));

// Controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "PickC API",
        Version = "v1",
        Description = "Pick-C Logistics API - Modular Monolith"
    });

    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter JWT token"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

// Authentication & Authorization
if (isDevelopment)
{
    // In Development, skip JWT — allow all requests through as anonymous
    builder.Services.AddAuthentication("DevBypass")
        .AddScheme<Microsoft.AspNetCore.Authentication.AuthenticationSchemeOptions, DevBypassAuthHandler>(
            "DevBypass", null);

    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("CustomerOnly", policy => policy.RequireAssertion(_ => true));
        options.AddPolicy("DriverOnly", policy => policy.RequireAssertion(_ => true));
        options.AddPolicy("CustomerOrDriver", policy => policy.RequireAssertion(_ => true));
    });
}
else
{
    // Production JWT Authentication
    var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>()!;

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings.Secret)),
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("CustomerOnly", policy =>
            policy.RequireClaim("userType", "CUSTOMER"));
        options.AddPolicy("DriverOnly", policy =>
            policy.RequireClaim("userType", "DRIVER"));
        options.AddPolicy("CustomerOrDriver", policy =>
            policy.RequireClaim("userType", "CUSTOMER", "DRIVER"));
    });
}

// Shared Infrastructure
builder.Services.AddSharedInfrastructure(builder.Configuration);

// Module Registration
builder.Services.AddIdentityModule(builder.Configuration);
builder.Services.AddMasterModule(builder.Configuration);
builder.Services.AddBookingModule(builder.Configuration);
builder.Services.AddTripModule(builder.Configuration);
builder.Services.AddBillingModule(builder.Configuration);
builder.Services.AddNotificationModule(builder.Configuration);
builder.Services.AddReportsModule(builder.Configuration);

// SignalR
builder.Services.AddSignalR();

// Health Checks
builder.Services.AddHealthChecks();

var app = builder.Build();

// Middleware pipeline
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "PickC API v1");
    options.RoutePrefix = string.Empty; // Swagger as default page at /
});

app.UseHttpsRedirection();
app.UseCors();

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");
app.MapHub<TripHub>("/hubs/trip");

app.Run();

// Make Program class accessible for integration tests
public partial class Program { }
