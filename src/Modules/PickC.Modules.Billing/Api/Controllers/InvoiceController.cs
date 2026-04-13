using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PickC.Modules.Billing.Application.DTOs;
using PickC.Modules.Billing.Application.Services;

namespace PickC.Modules.Billing.Api.Controllers;

[ApiController]
[Route("api/billing/invoices")]
[Authorize]
public class InvoiceController : ControllerBase
{
    private readonly IInvoiceService _service;

    public InvoiceController(IInvoiceService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await _service.GetAllAsync(ct);
        return Ok(result);
    }

    [HttpGet("{invoiceNo}/{tripId}")]
    public async Task<IActionResult> GetByKey(string invoiceNo, string tripId, CancellationToken ct)
    {
        var result = await _service.GetByKeyAsync(invoiceNo, tripId, ct);
        return Ok(result);
    }

    [HttpGet("booking/{bookingNo}")]
    public async Task<IActionResult> GetByBookingNo(string bookingNo, CancellationToken ct)
    {
        var result = await _service.GetByBookingNoAsync(bookingNo, ct);
        return result is not null ? Ok(result) : NotFound();
    }

    [HttpGet("trip/{tripId}")]
    public async Task<IActionResult> GetByTripId(string tripId, CancellationToken ct)
    {
        var result = await _service.GetByTripIdAsync(tripId, ct);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Save([FromBody] InvoiceSaveDto dto, CancellationToken ct)
    {
        var result = await _service.SaveAsync(dto, ct);
        return result ? Ok(new { message = "Invoice saved successfully" }) : BadRequest();
    }

    [HttpDelete("{invoiceNo}/{tripId}")]
    public async Task<IActionResult> Delete(string invoiceNo, string tripId, CancellationToken ct)
    {
        var result = await _service.DeleteAsync(invoiceNo, tripId, ct);
        return result ? Ok(new { message = "Invoice deleted" }) : NotFound();
    }

    [HttpPut("pay")]
    public async Task<IActionResult> MarkPaid([FromBody] InvoicePayDto dto, CancellationToken ct)
    {
        var result = await _service.MarkPaidAsync(dto, ct);
        return result ? Ok(new { message = "Invoice marked as paid" }) : BadRequest(new { message = "Invoice cannot be marked as paid" });
    }

    [HttpPut("mail-sent")]
    public async Task<IActionResult> MarkMailSent([FromBody] InvoiceMailDto dto, CancellationToken ct)
    {
        var result = await _service.MarkMailSentAsync(dto, ct);
        return result ? Ok(new { message = "Invoice marked as mail sent" }) : NotFound();
    }
}
