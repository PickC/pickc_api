using PickC.Modules.Reports.Application.DTOs;

namespace PickC.Modules.Reports.Application.Services;

public interface IReportQueryService
{
    Task<DailyBookingReportSummary> GetDailyBookingsAsync(DateTime date, CancellationToken ct = default);
    Task<List<DailyTripReportDto>> GetDailyTripsAsync(DateTime date, CancellationToken ct = default);
    Task<DailyPaymentReportSummary> GetDailyPaymentsAsync(DateTime date, CancellationToken ct = default);
    Task<SummaryReportDto> GetSummaryAsync(DateTime startDate, DateTime endDate, CancellationToken ct = default);
    Task<List<DriverEarningsReportDto>> GetDriverEarningsAsync(DateTime startDate, DateTime endDate, string? driverId, CancellationToken ct = default);
    Task<DriverTipReportSummary> GetDriverTipsAsync(DateTime startDate, DateTime endDate, string? driverId, CancellationToken ct = default);
    Task<InvoiceReportDto> GetInvoiceAsync(string invoiceNo, string tripId, CancellationToken ct = default);
    Task MarkInvoiceMailSentAsync(string invoiceNo, string tripId, CancellationToken ct = default);
}
