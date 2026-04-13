using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PickC.Modules.Master.Application.DTOs;
using PickC.Modules.Master.Application.Services;

namespace PickC.Modules.Master.Api.Controllers;

[ApiController]
[Route("api/master/addresses")]
[Authorize]
public class AddressController : ControllerBase
{
    private readonly IAddressService _service;

    public AddressController(IAddressService service)
    {
        _service = service;
    }

    [HttpGet("{linkId}")]
    public async Task<IActionResult> GetByLinkId(string linkId, CancellationToken ct)
    {
        var result = await _service.GetByLinkIdAsync(linkId, ct);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Save([FromBody] AddressSaveDto dto, CancellationToken ct)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "system";
        var result = await _service.SaveAsync(dto, userId, ct);
        return result ? Ok(new { message = "Address saved" }) : BadRequest();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var result = await _service.DeleteAsync(id, ct);
        return result ? Ok(new { message = "Address deleted" }) : NotFound();
    }
}
