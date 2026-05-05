using appify.models;

namespace appify.DataAccess.Contract
{
    public interface IDriverRepository
    {
        List<AvailableDriver> GetAvailableDrivers(int? vehicleGroupId);
    }
}
