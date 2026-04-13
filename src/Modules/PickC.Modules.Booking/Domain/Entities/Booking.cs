namespace PickC.Modules.Booking.Domain.Entities;

public class Booking
{
    public string BookingNo { get; set; } = string.Empty;
    public DateTime BookingDate { get; set; }
    public string CustomerID { get; set; } = string.Empty;
    public DateTime RequiredDate { get; set; }
    public string LocationFrom { get; set; } = string.Empty;
    public string LocationTo { get; set; } = string.Empty;
    public string CargoDescription { get; set; } = string.Empty;
    public short VehicleType { get; set; }
    public string Remarks { get; set; } = string.Empty;
    public bool IsConfirm { get; set; }
    public DateTime? ConfirmDate { get; set; }
    public string DriverID { get; set; } = string.Empty;
    public string VehicleNo { get; set; } = string.Empty;
    public bool IsCancel { get; set; }
    public DateTime? CancelTime { get; set; }
    public string CancelRemarks { get; set; } = string.Empty;
    public bool IsComplete { get; set; }
    public DateTime? CompleteTime { get; set; }
    public short? VehicleGroup { get; set; }
    public string PayLoad { get; set; } = string.Empty;
    public string CargoType { get; set; } = string.Empty;
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public decimal ToLatitude { get; set; }
    public decimal ToLongitude { get; set; }
    public bool IsReachPickUp { get; set; }
    public bool IsReachDestination { get; set; }
    public DateTime? PickupReachDateTime { get; set; }
    public DateTime? DestinationReachDateTime { get; set; }
    public string ReceiverMobileNo { get; set; } = string.Empty;
    public bool IsCancelByDriver { get; set; }
    public DateTime? DriverCancelDateTime { get; set; }
    public string DriverCancelRemarks { get; set; } = string.Empty;
    public short LoadingUnLoading { get; set; }
    public int Status { get; set; }
    public string? OTP { get; set; }
}
