using appify.models;

namespace appify.Business.Contract
{
    public interface IDriverBusiness
    {
        List<AvailableDriver> GetAvailableDrivers(int? vehicleGroupId);
    }
}
