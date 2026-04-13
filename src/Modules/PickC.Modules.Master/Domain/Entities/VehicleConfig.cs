namespace PickC.Modules.Master.Domain.Entities;

public class VehicleConfig
{
    public int VehicleModelId { get; set; }
    public string Model { get; set; } = string.Empty;
    public string Maker { get; set; } = string.Empty;
    public short VehicleGroup { get; set; }
    public decimal Tonnage { get; set; }
}
