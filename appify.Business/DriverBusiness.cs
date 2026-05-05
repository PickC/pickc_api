using appify.Business.Contract;
using appify.DataAccess.Contract;
using appify.models;

namespace appify.Business
{
    public partial class DriverBusiness : IDriverBusiness
    {
        private readonly IDriverRepository repository;

        public DriverBusiness(IDriverRepository repository)
        {
            this.repository = repository;
        }

        public List<AvailableDriver> GetAvailableDrivers(int? vehicleGroupId)
        {
            return repository.GetAvailableDrivers(vehicleGroupId);
        }
    }
}
