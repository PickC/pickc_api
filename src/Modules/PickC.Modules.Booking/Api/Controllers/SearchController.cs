using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PickC.Modules.Booking.Application.DTOs;
using PickC.Modules.Booking.Application.Services;
using PickC.SharedKernel.Helpers;

namespace PickC.Modules.Booking.Api.Controllers;

[ApiController]
[Route("api/booking/search")]
[Authorize]
public class SearchController : ControllerBase
{
    private readonly IBookingService _service;

    public SearchController(IBookingService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] BookingSearchDto dto, CancellationToken ct)
    {
        var result = await _service.SearchAsync(dto, ct);
        return Ok(result);
    }

    [HttpGet("today")]
    public async Task<IActionResult> GetByCurrentDate(CancellationToken ct)
    {
        var today = IstClock.Now.Date;
        var dto = new BookingSearchDto
        {
            FromDate = today,
            ToDate = today.AddDays(1).AddTicks(-1)
        };
        var result = await _service.SearchAsync(dto, ct);
        return Ok(result);
    }
}
