using appify.DataAccess.Contract;
using appify.models;
using appify.utility;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;


namespace appify.DataAccess
{
    public partial class ProductLogRepository : IProductLogRepository
    {
        private IConfiguration configuration;
        private string appify_connectionstring;

        public const string SELECTPRODUCTLOG = "[Audit].[usp_ProductLogSelect]";
        public const string LISTPRODUCTLOG = "[Audit].[usp_ProductLogList]";
        public const string SAVEPRODUCTLOG = "[Audit].[usp_ProductLogSave]";
        public const string REMOVEPRODUCTLOG = "[Audit].[usp_ProductLogDelete]";

        public ProductLogRepository(IConfiguration config)
        {
            this.configuration = config;
            this.appify_connectionstring = config["ConnectionStrings:appify.connectionstring"].ToString();
        }

        public IProductAuditLog GetProductLog(Int64 auditID)
        {
            ProductAuditLog item = new ProductAuditLog();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, SELECTPRODUCTLOG, auditID);
                item = DataTableHelper.ConvertDataTable<ProductAuditLog>(ds.Tables[0]).FirstOrDefault();
                con.Close();
            }

            return (IProductAuditLog)item;
        }

        public async Task<IEnumerable<IProductAuditLog>> ListProductLog(Int64 productID)
        {
            List<IProductAuditLog> items = new List<IProductAuditLog>();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, LISTPRODUCTLOG,productID);
                var concreteItems = DataTableHelper.ConvertDataTable<ProductAuditLog>(ds.Tables[0]);
                con.Close();

                items.AddRange(concreteItems);
            }

            return items;

        }
        public async Task<bool> SaveProductLog(IProductAuditLog itemData)
        {

            var result = false;
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(SAVEPRODUCTLOG))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@AuditID", itemData.AuditId);
                        cmd.Parameters.AddWithValue("@ProductID", itemData.ProductID);
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

        public bool DeleteProductLog(Int64 auditID)
        {

            var result = false;
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(REMOVEPRODUCTLOG))
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
