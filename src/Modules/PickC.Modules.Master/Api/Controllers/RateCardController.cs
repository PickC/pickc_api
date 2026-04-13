using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PickC.Modules.Master.Application.DTOs;
using PickC.Modules.Master.Domain.Entities;
using PickC.Modules.Master.Infrastructure.Data;

namespace PickC.Modules.Master.Api.Controllers;

[ApiController]
[Route("api/master/rate-cards")]
[Authorize]
public class RateCardController : ControllerBase
{
    private readonly MasterDbContext _context;

    public RateCardController(MasterDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var items = await _context.RateCards.AsNoTracking().ToListAsync(ct);
        return Ok(items);
    }

    [HttpGet("{category:int}/{vehicleType:int}/{rateType:int}")]
    public async Task<IActionResult> GetByKey(short category, short vehicleType, short rateType, CancellationToken ct)
    {
        var item = await _context.RateCards
            .FirstOrDefaultAsync(r => r.Category == category && r.VehicleType == vehicleType && r.RateType == rateType, ct);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> Save([FromBody] RateCardDto dto, CancellationToken ct)
    {
        var existing = await _context.RateCards
            .FirstOrDefaultAsync(r => r.Category == dto.Category && r.VehicleType == dto.VehicleType && r.RateType == dto.RateType, ct);

        if (existing is null)
        {
            _context.RateCards.Add(new RateCard
            {
                Category = dto.Category,
                VehicleType = dto.VehicleType,
                RateType = dto.RateType,
                BaseFare = dto.BaseFare,
                BaseKM = dto.BaseKM,
                DistanceFare = dto.DistanceFare,
                RideTimeFare = dto.RideTimeFare,
                WaitingFare = dto.WaitingFare,
                CancellationFee = dto.CancellationFee,
                DriverAssistance = dto.DriverAssistance,
                OverNightCharges = dto.OverNightCharges
            });
        }
        else
        {
            _context.Entry(existing).CurrentValues.SetValues(dto);
        }

        await _context.SaveChangesAsync(ct);
        return Ok(new { message = "Rate card saved" });
    }
}
