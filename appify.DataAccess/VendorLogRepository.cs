using appify.DataAccess.Contract;
using appify.models;
using appify.utility;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;


namespace appify.DataAccess
{
    public partial class VendorLogRepository : IVendorLogRepository
    {
        private IConfiguration configuration;
        private string appify_connectionstring;

        public const string SELECTVENDORLOG = "[Audit].[usp_VendorLogSelect]";
        public const string LISTVENDORLOG = "[Audit].[usp_VendorLogList]";
        public const string SAVEVENDORLOG = "[Audit].[usp_VendorLogSave]";
        public const string REMOVEVENDORLOG = "[Audit].[usp_VendorLogDelete]";

        public VendorLogRepository(IConfiguration config)
        {
            this.configuration = config;
            this.appify_connectionstring = config["ConnectionStrings:appify.connectionstring"].ToString();
        }

        public IVendorAuditLog GetVendorLog(Int64 auditID)
        {
            VendorAuditLog item = new VendorAuditLog();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, SELECTVENDORLOG,auditID);
                item = DataTableHelper.ConvertDataTable<VendorAuditLog>(ds.Tables[0]).FirstOrDefault();
                con.Close();
            }

            return item;
        }

        public List<IVendorAuditLog> ListVendorLog(Int64 vendorID)
        {
            var items = new List<IVendorAuditLog>();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, LISTVENDORLOG,vendorID);
                var concreteItems = DataTableHelper.ConvertDataTable<VendorAuditLog>(ds.Tables[0]);
                con.Close();

                items.AddRange(concreteItems);

            }

            return items;

        }
        public async Task<bool> SaveVendorLog(IVendorAuditLog itemData)
        {

            var result = false;
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(SAVEVENDORLOG))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@AuditID", itemData.AuditId);
                        cmd.Parameters.AddWithValue("@VendorID", itemData.VendorID);
                        cmd.Parameters.AddWithValue("@EventType", itemData.EventType);
                        cmd.Parameters.AddWithValue("@ChangedBy", itemData.ChangedBy);
                        cmd.Parameters.AddWithValue("@PayLoad", itemData.Payload);
                        cmd.Parameters.AddWithValue("@Source", itemData.Source);
                        cmd.Parameters.AddWithValue("@IPAddress", itemData.IPAddress);

                        con.Open();
                        result = Convert.ToBoolean(cmd.ExecuteNonQuery());
                        con.Close();
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public bool DeleteVendorLog(Int64 auditID)
        {

            var result = false;
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(REMOVEVENDORLOG))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@AuditID", auditID);
                        con.Open();
                        result = Convert.ToBoolean(cmd.ExecuteNonQuery());

                        con.Close();
                    }

                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return result;
        }


    }
}
