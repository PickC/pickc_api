namespace appify.models
{
    public partial class AvailableDriver
    {
        public string DriverID { get; set; }
        public string DriverName { get; set; }
        public int VehicleGroupID { get; set; }
        public string VehicleGroupName { get; set; }
        public int VehicleTypeID { get; set; }
        public string VehicleTypeName { get; set; }
        public string VehicleNumber { get; set; }
        public decimal CurrentLatitude { get; set; }
        public decimal CurrentLongitude { get; set; }
    }
}
