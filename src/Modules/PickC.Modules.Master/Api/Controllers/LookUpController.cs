using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PickC.Modules.Master.Application.DTOs;
using PickC.Modules.Master.Application.Services;

namespace PickC.Modules.Master.Api.Controllers;

[ApiController]
[Route("api/master/lookups")]
[Authorize]
public class LookUpController : ControllerBase
{
    private readonly ILookUpService _service;

    public LookUpController(ILookUpService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await _service.GetAllAsync(ct);
        return Ok(result);
    }

    [HttpGet("category/{category}")]
    public async Task<IActionResult> GetByCategory(string category, CancellationToken ct)
    {
        var result = await _service.GetByCategoryAsync(category, ct);
        return Ok(result);
    }

    [HttpGet("vehicle-groups")]
    public async Task<IActionResult> GetVehicleGroups(CancellationToken ct)
        => Ok(await _service.GetByCategoryAsync("VehicleGroup", ct));

    [HttpGet("vehicle-types")]
    public async Task<IActionResult> GetVehicleTypes(CancellationToken ct)
        => Ok(await _service.GetByCategoryAsync("VehicleType", ct));

    [HttpGet("cargo-types")]
    public async Task<IActionResult> GetCargoTypes(CancellationToken ct)
        => Ok(await _service.GetByCategoryAsync("CargoType", ct));

    [HttpGet("loading-types")]
    public async Task<IActionResult> GetLoadingTypes(CancellationToken ct)
        => Ok(await _service.GetByCategoryAsync("LoadingUnLoading", ct));

    [HttpPost]
    public async Task<IActionResult> Save([FromBody] LookUpDto dto, CancellationToken ct)
    {
        var result = await _service.SaveAsync(dto, ct);
        return result ? Ok(new { message = "LookUp saved" }) : BadRequest();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(short id, CancellationToken ct)
    {
        var result = await _service.DeleteAsync(id, ct);
        return result ? Ok(new { message = "LookUp deleted" }) : NotFound();
    }
}
