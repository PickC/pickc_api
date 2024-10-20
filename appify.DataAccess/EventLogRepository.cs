using appify.DataAccess.Contract;
using appify.models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using appify.utility;
using System.Data;
namespace appify.DataAccess
{
    public partial class EventLogRepository : IEventLogRepository
    {
        private IConfiguration configuration;
        private string appify_connectionstring;

        public EventLogRepository(IConfiguration config)
        {
            this.configuration = config;
            this.appify_connectionstring = config["ConnectionStrings:appify.connectionstring"].ToString();
        }

        public bool eventLogAdd(EventLogs eventLog)
        {

            var result = true;
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(dbroutine.DBStoredProc.SAVEEVENTLOG))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@EventID", eventLog.EventID);
                        cmd.Parameters.AddWithValue("@EventType", eventLog.EventType);
                        cmd.Parameters.AddWithValue("@VendorID", eventLog.VendorID);
                        cmd.Parameters.AddWithValue("@CustomerID", eventLog.CustomerID);
                        cmd.Parameters.AddWithValue("@Source", eventLog.Source);
                        cmd.Parameters.AddWithValue("@Module", eventLog.Module);
                        cmd.Parameters.AddWithValue("@IPAddress", eventLog.IPAddress);
                        cmd.Parameters.AddWithValue("@EventLog", eventLog.EventLog);
                        cmd.Parameters.AddWithValue("@InputJSON", eventLog.InputJSON);
                        cmd.Parameters.AddWithValue("@OutputJSON", eventLog.OutputJSON);
                        cmd.Parameters.AddWithValue("@EventTime", eventLog.EventTime);
                        cmd.Parameters.AddWithValue("@AppName", eventLog.AppName);
                        cmd.Parameters.AddWithValue("@Version", eventLog.Version);

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

        public bool eventLogRemove(long EventID)
        {

            var result = false;
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(dbroutine.DBStoredProc.DELETEEVENTLOG))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@EventID", EventID);


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

        public EventLogs eventLogGet(long EventID)
        {
            EventLogs item = new EventLogs();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.SELECTEVENTLOG, EventID);
            item = DataTableHelper.ConvertDataTable<EventLogs>(ds.Tables[0]).FirstOrDefault();

            return item;
        }

        public List<EventLogs> eventLogList()
        {
            List<EventLogs> items = new List<EventLogs>();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.LISTEVENTLOG);
            items = DataTableHelper.ConvertDataTable<EventLogs>(ds.Tables[0]);

            return items;
        }

        public List<EventLogs> eventLogListByVendor(long VendorID)
        {
            List<EventLogs> item = new List<EventLogs>();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.LISTEVENTLOGBYVENDOR, VendorID);
            item = DataTableHelper.ConvertDataTable<EventLogs>(ds.Tables[0]);
            return item;
        }

        public List<EventLogs> eventLogListByCustomer(long CustomerID)
        {
            List<EventLogs> item = new List<EventLogs>();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.LISTEVENTLOGBYCUSTOMER, CustomerID);
            item = DataTableHelper.ConvertDataTable<EventLogs>(ds.Tables[0]);
            return item;
        }
    }
}
