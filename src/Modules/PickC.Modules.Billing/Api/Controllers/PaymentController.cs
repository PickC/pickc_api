using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PickC.Modules.Billing.Application.Services;

namespace PickC.Modules.Billing.Api.Controllers;

[ApiController]
[Route("api/billing/drivers")]
[Authorize]
public class PaymentController : ControllerBase
{
    private readonly IDriverBillingService _service;

    public PaymentController(IDriverBillingService service)
    {
        _service = service;
    }

    [HttpGet("{driverId}/payments")]
    public async Task<IActionResult> GetDriverPayments(string driverId, CancellationToken ct)
    {
        var result = await _service.GetDriverPaymentsAsync(driverId, ct);
        return Ok(result);
    }

    [HttpGet("{driverId}/summary")]
    public async Task<IActionResult> GetDriverSummary(string driverId, CancellationToken ct)
    {
        var result = await _service.GetDriverSummaryAsync(driverId, ct);
        return Ok(result);
    }

    [HttpGet("{driverId}/invoices")]
    public async Task<IActionResult> GetDriverInvoices(string driverId, CancellationToken ct)
    {
        var result = await _service.GetDriverInvoicesAsync(driverId, ct);
        return Ok(result);
    }
}
