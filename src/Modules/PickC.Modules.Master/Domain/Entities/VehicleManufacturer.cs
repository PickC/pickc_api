namespace PickC.Modules.Master.Domain.Entities;

public class VehicleManufacturer
{
    public int ManufacturerId { get; set; }
    public string Manufacturer { get; set; } = string.Empty;
    public string MakeType { get; set; } = string.Empty;
    public decimal Capacity { get; set; }
}
