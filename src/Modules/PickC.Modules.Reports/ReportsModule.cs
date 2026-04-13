using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PickC.Modules.Reports.Application.Services;
using PickC.Modules.Reports.Infrastructure.Data;
using PickC.Modules.Reports.Infrastructure.Email;
using PickC.Modules.Reports.Infrastructure.Generators;
using QuestPDF.Infrastructure;

namespace PickC.Modules.Reports;

public static class ReportsModule
{
    public static IServiceCollection AddReportsModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ReportsDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("PickC")));

        services.Configure<SmtpSettings>(configuration.GetSection("SmtpSettings"));

        services.AddScoped<IReportQueryService, ReportQueryService>();
        services.AddScoped<IEmailService, SmtpEmailService>();
        services.AddSingleton<ExcelReportGenerator>();
        services.AddSingleton<PdfReportGenerator>();
        services.AddSingleton<InvoiceDocumentGenerator>();

        QuestPDF.Settings.License = LicenseType.Community;

        return services;
    }
}
