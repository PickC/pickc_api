using appify.DataAccess.Contract;
using appify.models;
using appify.utility;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;


namespace appify.DataAccess
{
    public partial class OrderLogRepository : IOrderLogRepository
    {
        private IConfiguration configuration;
        private string appify_connectionstring;

        public const string SELECTORDERLOG = "[Audit].[usp_OrderLogSelect]";
        public const string LISTORDERLOG = "[Audit].[usp_OrderLogList]";
        public const string SAVEORDERLOG = "[Audit].[usp_OrderLogSave]";
        public const string REMOVEORDERLOG = "[Audit].[usp_OrderLogDelete]";

        public OrderLogRepository(IConfiguration config)
        {
            this.configuration = config;
            this.appify_connectionstring = config["ConnectionStrings:appify.connectionstring"].ToString();
        }

        public IOrderAuditLog GetOrderLog(Int64 auditID)
        {
            OrderAuditLog item = new OrderAuditLog();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, SELECTORDERLOG, auditID);
                item = DataTableHelper.ConvertDataTable<OrderAuditLog>(ds.Tables[0]).FirstOrDefault();
                con.Close();
            }

            return item;
        }

        public List<IOrderAuditLog> ListOrderLog(Int64 orderID)
        {
            var items = new List<IOrderAuditLog>();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, LISTORDERLOG,orderID);
                var ConcreteItems = DataTableHelper.ConvertDataTable<OrderAuditLog>(ds.Tables[0]);
                con.Close();

                items.AddRange(ConcreteItems);
            }

            return items;

        }
        public async Task<bool> SaveOrderLog(IOrderAuditLog itemData)
        {

            var result = false;
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(SAVEORDERLOG))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@AuditID", itemData.AuditId);
                        cmd.Parameters.AddWithValue("@OrderID", itemData.OrderID);
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

        public bool DeleteOrderLog(Int64 auditID)
        {

            var result = false;
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(REMOVEORDERLOG))
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
