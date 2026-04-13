using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PickC.Modules.Master.Application.DTOs;
using PickC.Modules.Master.Application.Services;

namespace PickC.Modules.Master.Api.Controllers;

[ApiController]
[Route("api/master/drivers")]
[Authorize]
public class DriverController : ControllerBase
{
    private readonly IDriverService _service;

    public DriverController(IDriverService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await _service.GetAllAsync(ct);
        return Ok(result);
    }

    [HttpGet("{driverId}")]
    public async Task<IActionResult> GetById(string driverId, CancellationToken ct)
    {
        var result = await _service.GetByIdAsync(driverId, ct);
        return Ok(result);
    }

    [HttpGet("search")]
    public async Task<IActionResult> GetByName([FromQuery] string name, CancellationToken ct)
    {
        var result = await _service.GetByNameAsync(name, ct);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Save([FromBody] DriverSaveDto dto, CancellationToken ct)
    {
        var result = await _service.SaveAsync(dto, ct);
        return result ? Ok(new { message = "Driver saved successfully" }) : BadRequest();
    }

    [HttpPut("password")]
    public async Task<IActionResult> UpdatePassword([FromBody] DriverUpdatePasswordDto dto, CancellationToken ct)
    {
        var result = await _service.UpdatePasswordAsync(dto, ct);
        return result ? Ok(new { message = "Password updated" }) : NotFound(new { message = "Driver not found" });
    }

    [HttpDelete("{driverId}")]
    public async Task<IActionResult> Delete(string driverId, CancellationToken ct)
    {
        var result = await _service.DeleteAsync(driverId, ct);
        return result ? Ok(new { message = "Driver deleted" }) : NotFound();
    }
}
