using PickC.Modules.Billing.Application.DTOs;
using PickC.Modules.Billing.Domain.Entities;
using PickC.Modules.Billing.Domain.Interfaces;
using PickC.SharedKernel.Exceptions;

namespace PickC.Modules.Billing.Application.Services;

public interface IInvoiceService
{
    Task<List<InvoiceDto>> GetAllAsync(CancellationToken ct = default);
    Task<InvoiceDto> GetByKeyAsync(string invoiceNo, string tripId, CancellationToken ct = default);
    Task<InvoiceDto?> GetByBookingNoAsync(string bookingNo, CancellationToken ct = default);
    Task<List<InvoiceDto>> GetByTripIdAsync(string tripId, CancellationToken ct = default);
    Task<bool> SaveAsync(InvoiceSaveDto dto, CancellationToken ct = default);
    Task<bool> DeleteAsync(string invoiceNo, string tripId, CancellationToken ct = default);
    Task<bool> MarkPaidAsync(InvoicePayDto dto, CancellationToken ct = default);
    Task<bool> MarkMailSentAsync(InvoiceMailDto dto, CancellationToken ct = default);
}

public class InvoiceService : IInvoiceService
{
    private readonly IInvoiceRepository _repository;

    public InvoiceService(IInvoiceRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<InvoiceDto>> GetAllAsync(CancellationToken ct = default)
    {
        var invoices = await _repository.GetAllAsync(ct);
        return invoices.Select(MapToDto).ToList();
    }

    public async Task<InvoiceDto> GetByKeyAsync(string invoiceNo, string tripId, CancellationToken ct = default)
    {
        var invoice = await _repository.GetByKeyAsync(invoiceNo, tripId, ct)
            ?? throw new NotFoundException("Invoice", $"{invoiceNo}/{tripId}");
        return MapToDto(invoice);
    }

    public async Task<InvoiceDto?> GetByBookingNoAsync(string bookingNo, CancellationToken ct = default)
    {
        var invoice = await _repository.GetByBookingNoAsync(bookingNo, ct);
        return invoice is null ? null : MapToDto(invoice);
    }

    public async Task<List<InvoiceDto>> GetByTripIdAsync(string tripId, CancellationToken ct = default)
    {
        var invoices = await _repository.GetByTripIdAsync(tripId, ct);
        return invoices.Select(MapToDto).ToList();
    }

    public async Task<bool> SaveAsync(InvoiceSaveDto dto, CancellationToken ct = default)
    {
        var invoice = new Invoice
        {
            InvoiceNo = dto.InvoiceNo,
            TripID = dto.TripID,
            TripAmount = dto.TripAmount,
            TaxAmount = dto.TaxAmount,
            TipAmount = dto.TipAmount,
            TotalAmount = dto.TotalAmount,
            PaymentType = dto.PaymentType,
            BookingNo = dto.BookingNo
        };

        return await _repository.SaveAsync(invoice, ct);
    }

    public async Task<bool> DeleteAsync(string invoiceNo, string tripId, CancellationToken ct = default)
    {
        return await _repository.DeleteAsync(invoiceNo, tripId, ct);
    }

    public async Task<bool> MarkPaidAsync(InvoicePayDto dto, CancellationToken ct = default)
    {
        return await _repository.MarkPaidAsync(dto.InvoiceNo, dto.TripID, dto.PaidAmount, ct);
    }

    public async Task<bool> MarkMailSentAsync(InvoiceMailDto dto, CancellationToken ct = default)
    {
        return await _repository.MarkMailSentAsync(dto.InvoiceNo, dto.TripID, ct);
    }

    private static InvoiceDto MapToDto(Invoice i) => new()
    {
        InvoiceNo = i.InvoiceNo,
        TripID = i.TripID,
        InvoiceDate = i.InvoiceDate,
        TripAmount = i.TripAmount,
        TaxAmount = i.TaxAmount,
        TipAmount = i.TipAmount,
        TotalAmount = i.TotalAmount,
        PaymentType = i.PaymentType,
        PaidAmount = i.PaidAmount,
        CreatedOn = i.CreatedOn,
        IsMailSent = i.IsMailSent,
        BookingNo = i.BookingNo,
        IsPaid = i.IsPaid,
        PaidDate = i.PaidDate
    };
}
