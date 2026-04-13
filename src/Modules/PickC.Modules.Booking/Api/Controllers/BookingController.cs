using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PickC.Modules.Booking.Application.DTOs;
using PickC.Modules.Booking.Application.Services;

namespace PickC.Modules.Booking.Api.Controllers;

[ApiController]
[Route("api/booking/bookings")]
[Authorize]
public class BookingController : ControllerBase
{
    private readonly IBookingService _service;

    public BookingController(IBookingService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await _service.GetAllAsync(ct);
        return Ok(result);
    }

    [HttpGet("{bookingNo}")]
    public async Task<IActionResult> GetByBookingNo(string bookingNo, CancellationToken ct)
    {
        var result = await _service.GetByBookingNoAsync(bookingNo, ct);
        return Ok(result);
    }

    [HttpGet("customer/{customerId}")]
    public async Task<IActionResult> GetByCustomer(string customerId, CancellationToken ct)
    {
        var result = await _service.GetByCustomerAsync(customerId, ct);
        return Ok(result);
    }

    [HttpGet("driver/{driverId}")]
    public async Task<IActionResult> GetByDriver(string driverId, CancellationToken ct)
    {
        var result = await _service.GetByDriverAsync(driverId, ct);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Save([FromBody] BookingSaveDto dto, CancellationToken ct)
    {
        var result = await _service.SaveAsync(dto, ct);
        return result ? Ok(new { message = "Booking saved successfully" }) : BadRequest();
    }

    [HttpDelete("{bookingNo}")]
    public async Task<IActionResult> Delete(string bookingNo, CancellationToken ct)
    {
        var result = await _service.DeleteAsync(bookingNo, ct);
        return result ? Ok(new { message = "Booking deleted" }) : NotFound();
    }

    [HttpPut("confirm")]
    public async Task<IActionResult> Confirm([FromBody] BookingConfirmDto dto, CancellationToken ct)
    {
        var result = await _service.ConfirmAsync(dto, ct);
        return result ? Ok(new { message = "Booking confirmed" }) : BadRequest(new { message = "Booking cannot be confirmed" });
    }

    [HttpPut("cancel")]
    public async Task<IActionResult> CancelByCustomer([FromBody] BookingCancelDto dto, CancellationToken ct)
    {
        var result = await _service.CancelByCustomerAsync(dto, ct);
        return result ? Ok(new { message = "Booking cancelled" }) : BadRequest(new { message = "Booking cannot be cancelled" });
    }

    [HttpPut("cancel-by-driver")]
    public async Task<IActionResult> CancelByDriver([FromBody] BookingDriverCancelDto dto, CancellationToken ct)
    {
        var result = await _service.CancelByDriverAsync(dto, ct);
        return result ? Ok(new { message = "Booking cancelled by driver" }) : BadRequest(new { message = "Booking cannot be cancelled" });
    }

    [HttpPut("{bookingNo}/complete")]
    public async Task<IActionResult> Complete(string bookingNo, CancellationToken ct)
    {
        var result = await _service.CompleteAsync(bookingNo, ct);
        return result ? Ok(new { message = "Booking completed" }) : BadRequest(new { message = "Booking cannot be completed" });
    }

    [HttpPut("{bookingNo}/reach-pickup")]
    public async Task<IActionResult> ReachPickUp(string bookingNo, CancellationToken ct)
    {
        var result = await _service.UpdateReachPickUpAsync(bookingNo, ct);
        return result ? Ok(new { message = "Reached pickup location" }) : NotFound();
    }

    [HttpPut("{bookingNo}/reach-destination")]
    public async Task<IActionResult> ReachDestination(string bookingNo, CancellationToken ct)
    {
        var result = await _service.UpdateReachDestinationAsync(bookingNo, ct);
        return result ? Ok(new { message = "Reached destination" }) : NotFound();
    }

    [HttpGet("nearby")]
    public async Task<IActionResult> GetNearBookings(
        [FromQuery] decimal lat, [FromQuery] decimal lng, [FromQuery] double range = 10, CancellationToken ct = default)
    {
        var result = await _service.GetNearBookingsForDriverAsync(lat, lng, range, ct);
        return Ok(result);
    }
}
