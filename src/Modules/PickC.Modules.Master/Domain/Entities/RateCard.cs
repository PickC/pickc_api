namespace PickC.Modules.Master.Domain.Entities;

public class RateCard
{
    public short Category { get; set; }
    public short VehicleType { get; set; }
    public short RateType { get; set; }
    public decimal BaseFare { get; set; }
    public decimal BaseKM { get; set; }
    public decimal DistanceFare { get; set; }
    public decimal RideTimeFare { get; set; }
    public decimal WaitingFare { get; set; }
    public decimal CancellationFee { get; set; }
    public decimal DriverAssistance { get; set; }
    public decimal OverNightCharges { get; set; }
}
