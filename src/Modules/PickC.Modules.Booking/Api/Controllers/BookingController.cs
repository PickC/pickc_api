using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PickC.Modules.Booking.Application.DTOs;
using PickC.Modules.Booking.Application.Services;
using PickC.SharedKernel.Notifications;

namespace PickC.Modules.Booking.Api.Controllers;

[ApiController]
[Route("api/booking/bookings")]
[Authorize]
public class BookingController : ControllerBase
{
    private readonly IBookingService _service;
    private readonly IBookingNotifier _notifier;

    public BookingController(IBookingService service, IBookingNotifier notifier)
    {
        _service = service;
        _notifier = notifier;
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
        var bookingNo = await _service.SaveAsync(dto, ct);
        return string.IsNullOrEmpty(bookingNo)
            ? BadRequest()
            : Ok(new { message = "Booking saved successfully", bookingNo });
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
        if (!result) return BadRequest(new { message = "Booking cannot be confirmed" });

        var otp = Random.Shared.Next(1000, 9999).ToString();
        await _notifier.NotifyBookingAcceptedAsync(dto.BookingNo, dto.DriverID, dto.VehicleNo, otp, ct);

        return Ok(new { message = "Booking confirmed", otp });
    }

    [HttpPut("cancel")]
    public async Task<IActionResult> CancelByCustomer([FromBody] BookingCancelDto dto, CancellationToken ct)
    {
        var result = await _service.CancelByCustomerAsync(dto, ct);
        if (!result) return BadRequest(new { message = "Booking cannot be cancelled" });

        await _notifier.NotifyBookingCancelledByCustomerAsync(dto.BookingNo, ct);
        return Ok(new { message = "Booking cancelled" });
    }

    [HttpPut("cancel-by-driver")]
    public async Task<IActionResult> CancelByDriver([FromBody] BookingDriverCancelDto dto, CancellationToken ct)
    {
        var result = await _service.CancelByDriverAsync(dto, ct);
        if (!result) return BadRequest(new { message = "Booking cannot be cancelled" });

        await _notifier.NotifyBookingCancelledByDriverAsync(dto.BookingNo, ct);
        return Ok(new { message = "Booking cancelled by driver" });
    }

    [HttpPut("{bookingNo}/complete")]
    public async Task<IActionResult> Complete(string bookingNo, CancellationToken ct)
    {
        var result = await _service.CompleteAsync(bookingNo, ct);
        if (!result) return BadRequest(new { message = "Booking cannot be completed" });

        await _notifier.NotifyBookingCompletedAsync(bookingNo, ct);
        return Ok(new { message = "Booking completed" });
    }

    [HttpPut("{bookingNo}/reach-pickup")]
    public async Task<IActionResult> ReachPickUp(string bookingNo, CancellationToken ct)
    {
        var result = await _service.UpdateReachPickUpAsync(bookingNo, ct);
        if (!result) return NotFound();

        await _notifier.NotifyDriverReachedPickupAsync(bookingNo, ct);
        return Ok(new { message = "Reached pickup location" });
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
