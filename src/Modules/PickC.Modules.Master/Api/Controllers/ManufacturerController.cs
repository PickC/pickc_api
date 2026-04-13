using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PickC.Modules.Master.Application.DTOs;
using PickC.Modules.Master.Domain.Entities;
using PickC.Modules.Master.Infrastructure.Data;

namespace PickC.Modules.Master.Api.Controllers;

[ApiController]
[Route("api/master/manufacturers")]
[Authorize]
public class ManufacturerController : ControllerBase
{
    private readonly MasterDbContext _context;

    public ManufacturerController(MasterDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var items = await _context.VehicleManufacturers.AsNoTracking().ToListAsync(ct);
        return Ok(items);
    }

    [HttpPost]
    public async Task<IActionResult> Save([FromBody] VehicleManufacturerDto dto, CancellationToken ct)
    {
        if (dto.ManufacturerId > 0)
        {
            var existing = await _context.VehicleManufacturers
                .FirstOrDefaultAsync(m => m.ManufacturerId == dto.ManufacturerId, ct);
            if (existing is not null)
            {
                _context.Entry(existing).CurrentValues.SetValues(dto);
                await _context.SaveChangesAsync(ct);
                return Ok(new { message = "Manufacturer updated" });
            }
        }

        _context.VehicleManufacturers.Add(new VehicleManufacturer
        {
            Manufacturer = dto.Manufacturer,
            MakeType = dto.MakeType,
            Capacity = dto.Capacity
        });

        await _context.SaveChangesAsync(ct);
        return Ok(new { message = "Manufacturer saved" });
    }
}
