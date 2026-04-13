using PickC.Modules.Trip.Application.DTOs;
using PickC.Modules.Trip.Domain.Entities;
using PickC.Modules.Trip.Domain.Interfaces;

namespace PickC.Modules.Trip.Application.Services;

public interface ITripMonitorService
{
    Task<List<TripMonitorDto>> GetAllAsync(CancellationToken ct = default);
    Task<List<TripMonitorDto>> GetByTripIdAsync(string tripId, CancellationToken ct = default);
    Task<List<TripMonitorDto>> GetByDriverIdAsync(string driverId, CancellationToken ct = default);
    Task<bool> SaveAsync(TripMonitorSaveDto dto, CancellationToken ct = default);
}

public class TripMonitorService : ITripMonitorService
{
    private readonly ITripMonitorRepository _repository;

    public TripMonitorService(ITripMonitorRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<TripMonitorDto>> GetAllAsync(CancellationToken ct = default)
    {
        var monitors = await _repository.GetAllAsync(ct);
        return monitors.Select(MapToDto).ToList();
    }

    public async Task<List<TripMonitorDto>> GetByTripIdAsync(string tripId, CancellationToken ct = default)
    {
        var monitors = await _repository.GetByTripIdAsync(tripId, ct);
        return monitors.Select(MapToDto).ToList();
    }

    public async Task<List<TripMonitorDto>> GetByDriverIdAsync(string driverId, CancellationToken ct = default)
    {
        var monitors = await _repository.GetByDriverIdAsync(driverId, ct);
        return monitors.Select(MapToDto).ToList();
    }

    public async Task<bool> SaveAsync(TripMonitorSaveDto dto, CancellationToken ct = default)
    {
        var monitor = new TripMonitor
        {
            DriverID = dto.DriverID,
            TripID = dto.TripID,
            VehicleNo = dto.VehicleNo,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude,
            TripType = dto.TripType,
            Bearing = dto.Bearing,
            SpeedKmh = dto.SpeedKmh
        };

        return await _repository.SaveAsync(monitor, ct);
    }

    private static TripMonitorDto MapToDto(TripMonitor m) => new()
    {
        DriverID = m.DriverID,
        TripID = m.TripID,
        VehicleNo = m.VehicleNo,
        RefreshDate = m.RefreshDate,
        Latitude = m.Latitude,
        Longitude = m.Longitude,
        TripType = m.TripType,
        Bearing = m.Bearing,
        SpeedKmh = m.SpeedKmh
    };
}
