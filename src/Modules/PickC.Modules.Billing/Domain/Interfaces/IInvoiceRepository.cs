using PickC.Modules.Billing.Domain.Entities;

namespace PickC.Modules.Billing.Domain.Interfaces;

public interface IInvoiceRepository
{
    Task<List<Invoice>> GetAllAsync(CancellationToken ct = default);
    Task<Invoice?> GetByKeyAsync(string invoiceNo, string tripId, CancellationToken ct = default);
    Task<Invoice?> GetByBookingNoAsync(string bookingNo, CancellationToken ct = default);
    Task<List<Invoice>> GetByTripIdAsync(string tripId, CancellationToken ct = default);
    Task<bool> SaveAsync(Invoice invoice, CancellationToken ct = default);
    Task<bool> DeleteAsync(string invoiceNo, string tripId, CancellationToken ct = default);
    Task<bool> MarkPaidAsync(string invoiceNo, string tripId, decimal paidAmount, CancellationToken ct = default);
    Task<bool> MarkMailSentAsync(string invoiceNo, string tripId, CancellationToken ct = default);
}
