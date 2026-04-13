namespace PickC.Modules.Booking.Domain.Entities;

public class DriverCancellationHistory
{
    public string DriverID { get; set; } = string.Empty;
    public string BookingNo { get; set; } = string.Empty;
    public string CancelRemarks { get; set; } = string.Empty;
    public DateTime? CancelTime { get; set; }
}
