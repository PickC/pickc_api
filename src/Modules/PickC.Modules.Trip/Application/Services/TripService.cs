using PickC.Modules.Trip.Application.DTOs;
using PickC.Modules.Trip.Domain.Interfaces;
using PickC.SharedKernel.Exceptions;

namespace PickC.Modules.Trip.Application.Services;

public interface ITripService
{
    Task<List<TripDto>> GetAllAsync(CancellationToken ct = default);
    Task<TripDto> GetByTripIdAsync(string tripId, CancellationToken ct = default);
    Task<TripDto?> GetCurrentByDriverAsync(string driverId, CancellationToken ct = default);
    Task<TripDto?> GetCurrentByCustomerAsync(string customerMobile, CancellationToken ct = default);
    Task<List<TripDto>> GetByDriverAsync(string driverId, CancellationToken ct = default);
    Task<List<TripDto>> GetByBookingNoAsync(string bookingNo, CancellationToken ct = default);
    Task<string?> SaveAsync(TripSaveDto dto, CancellationToken ct = default);
    Task<bool> EndTripAsync(TripEndDto dto, CancellationToken ct = default);
    Task<bool> UpdateDistanceAsync(TripUpdateDistanceDto dto, CancellationToken ct = default);
    Task<bool> DeleteAsync(string tripId, CancellationToken ct = default);
}

public class TripService : ITripService
{
    private readonly ITripRepository _repository;

    public TripService(ITripRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<TripDto>> GetAllAsync(CancellationToken ct = default)
    {
        var trips = await _repository.GetAllAsync(ct);
        return trips.Select(MapToDto).ToList();
    }

    public async Task<TripDto> GetByTripIdAsync(string tripId, CancellationToken ct = default)
    {
        var trip = await _repository.GetByTripIdAsync(tripId, ct)
            ?? throw new NotFoundException("Trip", tripId);
        return MapToDto(trip);
    }

    public async Task<TripDto?> GetCurrentByDriverAsync(string driverId, CancellationToken ct = default)
    {
        var trip = await _repository.GetCurrentByDriverAsync(driverId, ct);
        return trip is null ? null : MapToDto(trip);
    }

    public async Task<TripDto?> GetCurrentByCustomerAsync(string customerMobile, CancellationToken ct = default)
    {
        var trip = await _repository.GetCurrentByCustomerAsync(customerMobile, ct);
        return trip is null ? null : MapToDto(trip);
    }

    public async Task<List<TripDto>> GetByDriverAsync(string driverId, CancellationToken ct = default)
    {
        var trips = await _repository.GetByDriverAsync(driverId, ct);
        return trips.Select(MapToDto).ToList();
    }

    public async Task<List<TripDto>> GetByBookingNoAsync(string bookingNo, CancellationToken ct = default)
    {
        var trips = await _repository.GetByBookingNoAsync(bookingNo, ct);
        return trips.Select(MapToDto).ToList();
    }

    public async Task<string?> SaveAsync(TripSaveDto dto, CancellationToken ct = default)
    {
        var trip = new Domain.Entities.Trip
        {
            TripID = dto.TripID,
            CustomerMobile = dto.CustomerMobile,
            DriverID = dto.DriverID,
            VehicleNo = dto.VehicleNo,
            VehicleType = dto.VehicleType,
            VehicleGroup = dto.VehicleGroup,
            LocationFrom = dto.LocationFrom,
            LocationTo = dto.LocationTo,
            Distance = dto.Distance,
            WaitingMinutes = dto.WaitingMinutes,
            TotalWeight = dto.TotalWeight,
            CargoDescription = dto.CargoDescription,
            Remarks = dto.Remarks,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude,
            BookingNo = dto.BookingNo
        };

        return await _repository.SaveAsync(trip, ct);
    }

    public async Task<bool> EndTripAsync(TripEndDto dto, CancellationToken ct = default)
    {
        return await _repository.EndTripAsync(
            dto.TripID, dto.TripEndLat, dto.TripEndLong, dto.DistanceTravelled, dto.TripMinutes, ct);
    }

    public async Task<bool> UpdateDistanceAsync(TripUpdateDistanceDto dto, CancellationToken ct = default)
    {
        return await _repository.UpdateDistanceTravelledAsync(dto.TripID, dto.DistanceTravelled, ct);
    }

    public async Task<bool> DeleteAsync(string tripId, CancellationToken ct = default)
    {
        return await _repository.DeleteAsync(tripId, ct);
    }

    private static TripDto MapToDto(Domain.Entities.Trip t) => new()
    {
        TripID = t.TripID,
        TripDate = t.TripDate,
        CustomerMobile = t.CustomerMobile,
        DriverID = t.DriverID,
        VehicleNo = t.VehicleNo,
        VehicleType = t.VehicleType,
        VehicleGroup = t.VehicleGroup,
        LocationFrom = t.LocationFrom,
        LocationTo = t.LocationTo,
        Distance = t.Distance,
        StartTime = t.StartTime,
        EndTime = t.EndTime,
        TripMinutes = t.TripMinutes,
        WaitingMinutes = t.WaitingMinutes,
        TotalWeight = t.TotalWeight,
        CargoDescription = t.CargoDescription,
        Remarks = t.Remarks,
        Latitude = t.Latitude,
        Longitude = t.Longitude,
        TripEndLat = t.TripEndLat,
        TripEndLong = t.TripEndLong,
        DistanceTravelled = t.DistanceTravelled,
        BookingNo = t.BookingNo
    };
}
