using Microsoft.EntityFrameworkCore;
using PickC.Modules.Billing.Domain.Entities;
using PickC.Modules.Billing.Domain.Interfaces;
using PickC.Modules.Billing.Infrastructure.Data;
using PickC.SharedKernel.Helpers;

namespace PickC.Modules.Billing.Infrastructure.Repositories;

public class InvoiceRepository : IInvoiceRepository
{
    private readonly BillingDbContext _context;

    public InvoiceRepository(BillingDbContext context)
    {
        _context = context;
    }

    public async Task<List<Invoice>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.Invoices
            .AsNoTracking()
            .OrderByDescending(i => i.InvoiceDate)
            .ToListAsync(ct);
    }

    public async Task<Invoice?> GetByKeyAsync(string invoiceNo, string tripId, CancellationToken ct = default)
    {
        return await _context.Invoices
            .FirstOrDefaultAsync(i => i.InvoiceNo == invoiceNo && i.TripID == tripId, ct);
    }

    public async Task<Invoice?> GetByBookingNoAsync(string bookingNo, CancellationToken ct = default)
    {
        return await _context.Invoices
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.BookingNo == bookingNo, ct);
    }

    public async Task<List<Invoice>> GetByTripIdAsync(string tripId, CancellationToken ct = default)
    {
        return await _context.Invoices
            .AsNoTracking()
            .Where(i => i.TripID == tripId)
            .ToListAsync(ct);
    }

    public async Task<bool> SaveAsync(Invoice invoice, CancellationToken ct = default)
    {
        var existing = await _context.Invoices
            .FirstOrDefaultAsync(i => i.InvoiceNo == invoice.InvoiceNo && i.TripID == invoice.TripID, ct);

        if (existing is null)
        {
            invoice.CreatedOn = IstClock.Now;
            invoice.InvoiceDate = IstClock.Now;
            _context.Invoices.Add(invoice);
        }
        else
        {
            _context.Entry(existing).CurrentValues.SetValues(invoice);
        }

        return await _context.SaveChangesAsync(ct) > 0;
    }

    public async Task<bool> DeleteAsync(string invoiceNo, string tripId, CancellationToken ct = default)
    {
        var invoice = await _context.Invoices
            .FirstOrDefaultAsync(i => i.InvoiceNo == invoiceNo && i.TripID == tripId, ct);

        if (invoice is null) return false;

        _context.Invoices.Remove(invoice);
        return await _context.SaveChangesAsync(ct) > 0;
    }

    public async Task<bool> MarkPaidAsync(string invoiceNo, string tripId, decimal paidAmount, CancellationToken ct = default)
    {
        return await _context.Invoices
            .Where(i => i.InvoiceNo == invoiceNo && i.TripID == tripId && !i.IsPaid)
            .ExecuteUpdateAsync(s => s
                .SetProperty(i => i.IsPaid, true)
                .SetProperty(i => i.PaidAmount, paidAmount)
                .SetProperty(i => i.PaidDate, IstClock.Now), ct) > 0;
    }

    public async Task<bool> MarkMailSentAsync(string invoiceNo, string tripId, CancellationToken ct = default)
    {
        return await _context.Invoices
            .Where(i => i.InvoiceNo == invoiceNo && i.TripID == tripId)
            .ExecuteUpdateAsync(s =>
                s.SetProperty(i => i.IsMailSent, true), ct) > 0;
    }
}
