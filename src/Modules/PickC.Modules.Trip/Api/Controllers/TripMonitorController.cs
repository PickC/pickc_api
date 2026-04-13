using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PickC.Modules.Trip.Application.DTOs;
using PickC.Modules.Trip.Application.Services;

namespace PickC.Modules.Trip.Api.Controllers;

[ApiController]
[Route("api/trip/monitors")]
[Authorize]
public class TripMonitorController : ControllerBase
{
    private readonly ITripMonitorService _service;

    public TripMonitorController(ITripMonitorService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await _service.GetAllAsync(ct);
        return Ok(result);
    }

    [HttpGet("trip/{tripId}")]
    public async Task<IActionResult> GetByTripId(string tripId, CancellationToken ct)
    {
        var result = await _service.GetByTripIdAsync(tripId, ct);
        return Ok(result);
    }

    [HttpGet("driver/{driverId}")]
    public async Task<IActionResult> GetByDriverId(string driverId, CancellationToken ct)
    {
        var result = await _service.GetByDriverIdAsync(driverId, ct);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Save([FromBody] TripMonitorSaveDto dto, CancellationToken ct)
    {
        var result = await _service.SaveAsync(dto, ct);
        return result ? Ok(new { message = "Trip monitor saved" }) : BadRequest();
    }
}
