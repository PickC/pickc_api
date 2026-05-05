using appify.DataAccess.Contract;
using appify.models;
using appify.utility;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace appify.DataAccess
{
    public partial class DriverRepository : IDriverRepository
    {
        private readonly string appify_connectionstring;

        public DriverRepository(IConfiguration config)
        {
            this.appify_connectionstring = config["ConnectionStrings:appify.connectionstring"].ToString();
        }

        public List<AvailableDriver> GetAvailableDrivers(int? vehicleGroupId)
        {
            List<AvailableDriver> items = new List<AvailableDriver>();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(
                    con,
                    dbroutine.DBStoredProc.LISTAVAILABLEDRIVERS,
                    vehicleGroupId.HasValue ? (object)vehicleGroupId.Value : DBNull.Value);
                items = DataTableHelper.ConvertDataTable<AvailableDriver>(ds.Tables[0]);
            }
            return items;
        }
    }
}
