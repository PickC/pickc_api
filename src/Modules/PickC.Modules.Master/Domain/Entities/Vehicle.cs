namespace PickC.Modules.Master.Domain.Entities;

public class Vehicle
{
    public string VehicleNo { get; set; } = string.Empty;
    public short VehicleGroup { get; set; }
    public short VehicleType { get; set; }
    public string? OperatorID { get; set; }
}
