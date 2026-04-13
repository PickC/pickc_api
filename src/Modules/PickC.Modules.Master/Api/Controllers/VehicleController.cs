using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PickC.Modules.Master.Application.DTOs;
using PickC.Modules.Master.Domain.Entities;
using PickC.Modules.Master.Infrastructure.Data;

namespace PickC.Modules.Master.Api.Controllers;

[ApiController]
[Route("api/master/vehicles")]
[Authorize]
public class VehicleController : ControllerBase
{
    private readonly MasterDbContext _context;

    public VehicleController(MasterDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var items = await _context.Vehicles.AsNoTracking().ToListAsync(ct);
        return Ok(items);
    }

    [HttpGet("{vehicleNo}")]
    public async Task<IActionResult> GetByNo(string vehicleNo, CancellationToken ct)
    {
        var item = await _context.Vehicles.FirstOrDefaultAsync(v => v.VehicleNo == vehicleNo, ct);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> Save([FromBody] VehicleDto dto, CancellationToken ct)
    {
        var existing = await _context.Vehicles
            .FirstOrDefaultAsync(v => v.VehicleNo == dto.VehicleNo, ct);

        if (existing is null)
        {
            _context.Vehicles.Add(new Vehicle
            {
                VehicleNo = dto.VehicleNo,
                VehicleGroup = dto.VehicleGroup,
                VehicleType = dto.VehicleType,
                OperatorID = dto.OperatorID
            });
        }
        else
        {
            _context.Entry(existing).CurrentValues.SetValues(dto);
        }

        await _context.SaveChangesAsync(ct);
        return Ok(new { message = "Vehicle saved" });
    }
}
