using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PickC.Modules.Trip.Application.DTOs;
using PickC.Modules.Trip.Application.Services;

namespace PickC.Modules.Trip.Api.Controllers;

[ApiController]
[Route("api/trip/trips")]
[Authorize]
public class TripController : ControllerBase
{
    private readonly ITripService _service;

    public TripController(ITripService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await _service.GetAllAsync(ct);
        return Ok(result);
    }

    [HttpGet("{tripId}")]
    public async Task<IActionResult> GetByTripId(string tripId, CancellationToken ct)
    {
        var result = await _service.GetByTripIdAsync(tripId, ct);
        return Ok(result);
    }

    [HttpGet("driver/{driverId}/current")]
    public async Task<IActionResult> GetCurrentByDriver(string driverId, CancellationToken ct)
    {
        var result = await _service.GetCurrentByDriverAsync(driverId, ct);
        return result is not null ? Ok(result) : NotFound(new { message = "No active trip found" });
    }

    [HttpGet("customer/{customerMobile}/current")]
    public async Task<IActionResult> GetCurrentByCustomer(string customerMobile, CancellationToken ct)
    {
        var result = await _service.GetCurrentByCustomerAsync(customerMobile, ct);
        return result is not null ? Ok(result) : NotFound(new { message = "No active trip found" });
    }

    [HttpGet("driver/{driverId}")]
    public async Task<IActionResult> GetByDriver(string driverId, CancellationToken ct)
    {
        var result = await _service.GetByDriverAsync(driverId, ct);
        return Ok(result);
    }

    [HttpGet("booking/{bookingNo}")]
    public async Task<IActionResult> GetByBookingNo(string bookingNo, CancellationToken ct)
    {
        var result = await _service.GetByBookingNoAsync(bookingNo, ct);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Save([FromBody] TripSaveDto dto, CancellationToken ct)
    {
        var tripId = await _service.SaveAsync(dto, ct);
        return tripId is not null
            ? Ok(new { message = "Trip saved successfully", tripId })
            : BadRequest(new { message = "Trip could not be saved" });
    }

    [HttpPut("end")]
    public async Task<IActionResult> EndTrip([FromBody] TripEndDto dto, CancellationToken ct)
    {
        var result = await _service.EndTripAsync(dto, ct);
        return result ? Ok(new { message = "Trip ended successfully" }) : BadRequest(new { message = "Trip cannot be ended" });
    }

    [HttpPatch("distance")]
    public async Task<IActionResult> UpdateDistance([FromBody] TripUpdateDistanceDto dto, CancellationToken ct)
    {
        var result = await _service.UpdateDistanceAsync(dto, ct);
        return result ? Ok(new { message = "Distance updated" }) : NotFound();
    }

    [HttpDelete("{tripId}")]
    public async Task<IActionResult> Delete(string tripId, CancellationToken ct)
    {
        var result = await _service.DeleteAsync(tripId, ct);
        return result ? Ok(new { message = "Trip deleted" }) : NotFound();
    }
}
