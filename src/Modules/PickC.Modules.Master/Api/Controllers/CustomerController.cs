using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PickC.Modules.Master.Application.DTOs;
using PickC.Modules.Master.Application.Services;

namespace PickC.Modules.Master.Api.Controllers;

[ApiController]
[Route("api/master/customers")]
[Authorize]
public class CustomerController : ControllerBase
{
    private readonly ICustomerService _service;

    public CustomerController(ICustomerService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await _service.GetAllAsync(ct);
        return Ok(result);
    }

    [HttpGet("{mobileNo}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByMobile(string mobileNo, CancellationToken ct)
    {
        var result = await _service.GetByMobileAsync(mobileNo, ct);
        return Ok(result);
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Save([FromBody] CustomerSaveDto dto, CancellationToken ct)
    {
        var result = await _service.SaveAsync(dto, ct);
        return result ? Ok(new { message = "Customer saved successfully" }) : BadRequest();
    }

    [HttpDelete("{mobileNo}")]
    public async Task<IActionResult> Delete(string mobileNo, CancellationToken ct)
    {
        var result = await _service.DeleteAsync(mobileNo, ct);
        return result ? Ok(new { message = "Customer deleted" }) : NotFound();
    }

    [HttpPut("device")]
    public async Task<IActionResult> UpdateDevice([FromBody] CustomerUpdateDeviceDto dto, CancellationToken ct)
    {
        var result = await _service.UpdateDeviceIdAsync(dto, ct);
        return result ? Ok(new { message = "Device updated" }) : NotFound();
    }

    [HttpPut("password")]
    public async Task<IActionResult> UpdatePassword([FromBody] CustomerUpdatePasswordDto dto, CancellationToken ct)
    {
        var result = await _service.UpdatePasswordAsync(dto, ct);
        return result ? Ok(new { message = "Password updated" }) : NotFound();
    }
}
