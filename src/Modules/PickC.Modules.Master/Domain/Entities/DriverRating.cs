namespace PickC.Modules.Master.Domain.Entities;

public class DriverRating
{
    public string BookingNo { get; set; } = string.Empty;
    public string DriverID { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string? Remarks { get; set; }
}
