using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PickC.Modules.Master.Infrastructure.Data;

namespace PickC.Modules.Master.Api.Controllers;

[ApiController]
[Route("api/master/vehicle-configs")]
[Authorize]
public class VehicleConfigController : ControllerBase
{
    private readonly MasterDbContext _context;

    public VehicleConfigController(MasterDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var items = await _context.VehicleConfigs.AsNoTracking().ToListAsync(ct);
        return Ok(items);
    }

    [HttpGet("{model}")]
    public async Task<IActionResult> GetByModel(string model, CancellationToken ct)
    {
        var item = await _context.VehicleConfigs.FirstOrDefaultAsync(v => v.Model == model, ct);
        return item is null ? NotFound() : Ok(item);
    }
}
