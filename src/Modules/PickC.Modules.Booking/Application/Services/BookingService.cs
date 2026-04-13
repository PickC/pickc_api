using PickC.Modules.Booking.Application.DTOs;
using PickC.Modules.Booking.Domain.Interfaces;
using PickC.SharedKernel.Exceptions;
using PickC.SharedKernel.Helpers;

namespace PickC.Modules.Booking.Application.Services;

public interface IBookingService
{
    Task<List<BookingDto>> GetAllAsync(CancellationToken ct = default);
    Task<BookingDto> GetByBookingNoAsync(string bookingNo, CancellationToken ct = default);
    Task<List<BookingDto>> GetByCustomerAsync(string customerId, CancellationToken ct = default);
    Task<List<BookingDto>> GetByDriverAsync(string driverId, CancellationToken ct = default);
    Task<bool> SaveAsync(BookingSaveDto dto, CancellationToken ct = default);
    Task<bool> DeleteAsync(string bookingNo, CancellationToken ct = default);
    Task<bool> ConfirmAsync(BookingConfirmDto dto, CancellationToken ct = default);
    Task<bool> CancelByCustomerAsync(BookingCancelDto dto, CancellationToken ct = default);
    Task<bool> CancelByDriverAsync(BookingDriverCancelDto dto, CancellationToken ct = default);
    Task<bool> CompleteAsync(string bookingNo, CancellationToken ct = default);
    Task<bool> UpdateReachPickUpAsync(string bookingNo, CancellationToken ct = default);
    Task<bool> UpdateReachDestinationAsync(string bookingNo, CancellationToken ct = default);
    Task<List<BookingDto>> SearchAsync(BookingSearchDto dto, CancellationToken ct = default);
    Task<List<NearBookingDto>> GetNearBookingsForDriverAsync(decimal latitude, decimal longitude, double rangeKm, CancellationToken ct = default);
}

public class BookingService : IBookingService
{
    private readonly IBookingRepository _repository;

    public BookingService(IBookingRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<BookingDto>> GetAllAsync(CancellationToken ct = default)
    {
        var bookings = await _repository.GetAllAsync(ct);
        return bookings.Select(MapToDto).ToList();
    }

    public async Task<BookingDto> GetByBookingNoAsync(string bookingNo, CancellationToken ct = default)
    {
        var booking = await _repository.GetByBookingNoAsync(bookingNo, ct)
            ?? throw new NotFoundException("Booking", bookingNo);
        return MapToDto(booking);
    }

    public async Task<List<BookingDto>> GetByCustomerAsync(string customerId, CancellationToken ct = default)
    {
        var bookings = await _repository.GetByCustomerAsync(customerId, ct);
        return bookings.Select(MapToDto).ToList();
    }

    public async Task<List<BookingDto>> GetByDriverAsync(string driverId, CancellationToken ct = default)
    {
        var bookings = await _repository.GetByDriverAsync(driverId, ct);
        return bookings.Select(MapToDto).ToList();
    }

    public async Task<bool> SaveAsync(BookingSaveDto dto, CancellationToken ct = default)
    {
        var booking = new Domain.Entities.Booking
        {
            BookingNo = dto.BookingNo,
            CustomerID = dto.CustomerID,
            RequiredDate = dto.RequiredDate,
            LocationFrom = dto.LocationFrom,
            LocationTo = dto.LocationTo,
            CargoDescription = dto.CargoDescription,
            VehicleType = dto.VehicleType,
            VehicleGroup = dto.VehicleGroup,
            CargoType = dto.CargoType,
            PayLoad = dto.PayLoad,
            LoadingUnLoading = dto.LoadingUnLoading,
            Remarks = dto.Remarks,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude,
            ToLatitude = dto.ToLatitude,
            ToLongitude = dto.ToLongitude,
            ReceiverMobileNo = dto.ReceiverMobileNo,
            Status = dto.Status
        };

        return await _repository.SaveAsync(booking, ct);
    }

    public async Task<bool> DeleteAsync(string bookingNo, CancellationToken ct = default)
    {
        return await _repository.DeleteAsync(bookingNo, ct);
    }

    public async Task<bool> ConfirmAsync(BookingConfirmDto dto, CancellationToken ct = default)
    {
        return await _repository.ConfirmAsync(dto.BookingNo, dto.DriverID, dto.VehicleNo, ct);
    }

    public async Task<bool> CancelByCustomerAsync(BookingCancelDto dto, CancellationToken ct = default)
    {
        return await _repository.CancelByCustomerAsync(dto.BookingNo, dto.CancelRemarks, ct);
    }

    public async Task<bool> CancelByDriverAsync(BookingDriverCancelDto dto, CancellationToken ct = default)
    {
        return await _repository.CancelByDriverAsync(dto.BookingNo, dto.DriverID, dto.CancelRemarks, ct);
    }

    public async Task<bool> CompleteAsync(string bookingNo, CancellationToken ct = default)
    {
        return await _repository.CompleteAsync(bookingNo, ct);
    }

    public async Task<bool> UpdateReachPickUpAsync(string bookingNo, CancellationToken ct = default)
    {
        return await _repository.UpdateReachPickUpAsync(bookingNo, ct);
    }

    public async Task<bool> UpdateReachDestinationAsync(string bookingNo, CancellationToken ct = default)
    {
        return await _repository.UpdateReachDestinationAsync(bookingNo, ct);
    }

    public async Task<List<BookingDto>> SearchAsync(BookingSearchDto dto, CancellationToken ct = default)
    {
        List<Domain.Entities.Booking> bookings;

        if (dto.FromDate.HasValue && dto.ToDate.HasValue)
        {
            bookings = await _repository.SearchByDateAsync(dto.FromDate.Value, dto.ToDate.Value, ct);
        }
        else if (dto.Status.HasValue)
        {
            bookings = await _repository.SearchByStatusAsync(dto.Status.Value, ct);
        }
        else if (!string.IsNullOrEmpty(dto.CustomerID))
        {
            bookings = await _repository.GetByCustomerAsync(dto.CustomerID, ct);
        }
        else
        {
            bookings = await _repository.GetAllAsync(ct);
        }

        return bookings.Select(MapToDto).ToList();
    }

    public async Task<List<NearBookingDto>> GetNearBookingsForDriverAsync(
        decimal latitude, decimal longitude, double rangeKm, CancellationToken ct = default)
    {
        var bookings = await _repository.GetNearBookingsForDriverAsync(latitude, longitude, rangeKm, ct);

        return bookings.Select(b => new NearBookingDto
        {
            BookingNo = b.BookingNo,
            CustomerID = b.CustomerID,
            LocationFrom = b.LocationFrom,
            LocationTo = b.LocationTo,
            CargoDescription = b.CargoDescription,
            VehicleType = b.VehicleType,
            VehicleGroup = b.VehicleGroup,
            CargoType = b.CargoType,
            LoadingUnLoading = b.LoadingUnLoading,
            Latitude = b.Latitude,
            Longitude = b.Longitude,
            ToLatitude = b.ToLatitude,
            ToLongitude = b.ToLongitude,
            RequiredDate = b.RequiredDate,
            DistanceKm = CoordinateDistanceHelper.CalculateDistanceKm(
                (double)latitude, (double)longitude,
                (double)b.Latitude, (double)b.Longitude)
        }).ToList();
    }

    private static BookingDto MapToDto(Domain.Entities.Booking b) => new()
    {
        BookingNo = b.BookingNo,
        BookingDate = b.BookingDate,
        CustomerID = b.CustomerID,
        RequiredDate = b.RequiredDate,
        LocationFrom = b.LocationFrom,
        LocationTo = b.LocationTo,
        CargoDescription = b.CargoDescription,
        VehicleType = b.VehicleType,
        VehicleGroup = b.VehicleGroup,
        CargoType = b.CargoType,
        PayLoad = b.PayLoad,
        LoadingUnLoading = b.LoadingUnLoading,
        Remarks = b.Remarks,
        Latitude = b.Latitude,
        Longitude = b.Longitude,
        ToLatitude = b.ToLatitude,
        ToLongitude = b.ToLongitude,
        ReceiverMobileNo = b.ReceiverMobileNo,
        IsConfirm = b.IsConfirm,
        ConfirmDate = b.ConfirmDate,
        DriverID = b.DriverID,
        VehicleNo = b.VehicleNo,
        IsCancel = b.IsCancel,
        CancelTime = b.CancelTime,
        CancelRemarks = b.CancelRemarks,
        IsCancelByDriver = b.IsCancelByDriver,
        DriverCancelDateTime = b.DriverCancelDateTime,
        IsComplete = b.IsComplete,
        CompleteTime = b.CompleteTime,
        IsReachPickUp = b.IsReachPickUp,
        PickupReachDateTime = b.PickupReachDateTime,
        IsReachDestination = b.IsReachDestination,
        DestinationReachDateTime = b.DestinationReachDateTime,
        Status = b.Status
    };
}
