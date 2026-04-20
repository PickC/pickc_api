namespace PickC.Modules.Booking.Application.DTOs;

public class BookingDto
{
    public string BookingNo { get; set; } = string.Empty;
    public DateTime BookingDate { get; set; }
    public string CustomerID { get; set; } = string.Empty;
    public DateTime RequiredDate { get; set; }
    public string LocationFrom { get; set; } = string.Empty;
    public string LocationTo { get; set; } = string.Empty;
    public string CargoDescription { get; set; } = string.Empty;
    public short VehicleType { get; set; }
    public short? VehicleGroup { get; set; }
    public string CargoType { get; set; } = string.Empty;
    public string PayLoad { get; set; } = string.Empty;
    public short LoadingUnLoading { get; set; }
    public string Remarks { get; set; } = string.Empty;
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public decimal ToLatitude { get; set; }
    public decimal ToLongitude { get; set; }
    public string ReceiverMobileNo { get; set; } = string.Empty;
    public bool IsConfirm { get; set; }
    public DateTime? ConfirmDate { get; set; }
    public string DriverID { get; set; } = string.Empty;
    public string VehicleNo { get; set; } = string.Empty;
    public bool IsCancel { get; set; }
    public DateTime? CancelTime { get; set; }
    public string CancelRemarks { get; set; } = string.Empty;
    public bool IsCancelByDriver { get; set; }
    public DateTime? DriverCancelDateTime { get; set; }
    public bool IsComplete { get; set; }
    public DateTime? CompleteTime { get; set; }
    public bool IsReachPickUp { get; set; }
    public DateTime? PickupReachDateTime { get; set; }
    public bool IsReachDestination { get; set; }
    public DateTime? DestinationReachDateTime { get; set; }
    public int Status { get; set; }

    // Lookup-resolved display fields
    public string? VehicleTypeName { get; set; }
    public string? VehicleTypeIcon { get; set; }
    public string? VehicleGroupName { get; set; }
}

public class BookingSaveDto
{
    public string BookingNo { get; set; } = string.Empty;
    public string CustomerID { get; set; } = string.Empty;
    public DateTime RequiredDate { get; set; }
    public string LocationFrom { get; set; } = string.Empty;
    public string LocationTo { get; set; } = string.Empty;
    public string CargoDescription { get; set; } = string.Empty;
    public short VehicleType { get; set; }
    public short? VehicleGroup { get; set; }
    public string CargoType { get; set; } = string.Empty;
    public string PayLoad { get; set; } = string.Empty;
    public short LoadingUnLoading { get; set; }
    public string Remarks { get; set; } = string.Empty;
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public decimal ToLatitude { get; set; }
    public decimal ToLongitude { get; set; }
    public string ReceiverMobileNo { get; set; } = string.Empty;
    public int Status { get; set; }
}

public class BookingConfirmDto
{
    public string BookingNo { get; set; } = string.Empty;
    public string DriverID { get; set; } = string.Empty;
    public string VehicleNo { get; set; } = string.Empty;
}

public class BookingCancelDto
{
    public string BookingNo { get; set; } = string.Empty;
    public string CancelRemarks { get; set; } = string.Empty;
}

public class BookingDriverCancelDto
{
    public string BookingNo { get; set; } = string.Empty;
    public string DriverID { get; set; } = string.Empty;
    public string CancelRemarks { get; set; } = string.Empty;
}

public class BookingSearchDto
{
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int? Status { get; set; }
    public string? CustomerID { get; set; }
}

public class NearBookingDto
{
    public string BookingNo { get; set; } = string.Empty;
    public string CustomerID { get; set; } = string.Empty;
    public string LocationFrom { get; set; } = string.Empty;
    public string LocationTo { get; set; } = string.Empty;
    public string CargoDescription { get; set; } = string.Empty;
    public short VehicleType { get; set; }
    public short? VehicleGroup { get; set; }
    public string CargoType { get; set; } = string.Empty;
    public short LoadingUnLoading { get; set; }
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public decimal ToLatitude { get; set; }
    public decimal ToLongitude { get; set; }
    public DateTime RequiredDate { get; set; }
    public double DistanceKm { get; set; }
}
