namespace PickC.Modules.Master.Application.DTOs;

public class VehicleConfigDto
{
    public int VehicleModelId { get; set; }
    public string Model { get; set; } = string.Empty;
    public string Maker { get; set; } = string.Empty;
    public short VehicleGroup { get; set; }
    public decimal Tonnage { get; set; }
}

public class VehicleDto
{
    public string VehicleNo { get; set; } = string.Empty;
    public short VehicleGroup { get; set; }
    public short VehicleType { get; set; }
    public string? OperatorID { get; set; }
}

public class VehicleManufacturerDto
{
    public int ManufacturerId { get; set; }
    public string Manufacturer { get; set; } = string.Empty;
    public string MakeType { get; set; } = string.Empty;
    public decimal Capacity { get; set; }
}

public class OnDutyVehicleDto
{
    public string VehicleNo { get; set; } = string.Empty;
    public string DriverID { get; set; } = string.Empty;
    public string DriverName { get; set; } = string.Empty;
    public short VehicleGroup { get; set; }
    public string? VehicleGroupName { get; set; }
    public short VehicleType { get; set; }
    public string? VehicleTypeName { get; set; }
    public decimal? CurrentLatitude { get; set; }
    public decimal? CurrentLongitude { get; set; }
    public DateTime? DutyOnDate { get; set; }
}

public class LookUpDto
{
    public short LookupID { get; set; }
    public string LookupCode { get; set; } = string.Empty;
    public string LookupDescription { get; set; } = string.Empty;
    public string LookupCategory { get; set; } = string.Empty;
    public string? Image { get; set; }
}
