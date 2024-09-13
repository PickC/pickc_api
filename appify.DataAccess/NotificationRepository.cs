using appify.DataAccess.Contract;
using appify.models;
using appify.utility;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace appify.DataAccess
{
    public partial class NotificationRepository : INotificationRepository
    {
        private IConfiguration configuration;
        private string appify_connectionstring;
        public NotificationRepository(IConfiguration config)
        {
            this.configuration = config;
            this.appify_connectionstring = config["ConnectionStrings:appify.connectionstring"].ToString();
        }
        public Notifications SendEmail(Notifications notifications)
        {
            throw new NotImplementedException();
        }

        public List<PushNotificationMessage> GetNotificationByVendor(long VendorID)
        {
            List<PushNotificationMessage> items = new List<PushNotificationMessage>();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.LISTNOTIFICATIONBYVENDOR, VendorID);
            items = DataTableHelper.ConvertDataTable<PushNotificationMessage>(ds.Tables[0]);

            return items;
        }
        public List<PushNotificationMessage> GetNotificationByUser(long CustomerID)
        {
            List<PushNotificationMessage> items = new List<PushNotificationMessage>();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.LISTNOTIFICATIONBYCUSTOMER, CustomerID);
            items = DataTableHelper.ConvertDataTable<PushNotificationMessage>(ds.Tables[0]);

            return items;
        }

        public List<PushNotificationMessage> GetNotificationByVendor(long VendorID, short PageNo, short Rows)
        {
            List<PushNotificationMessage> items = new List<PushNotificationMessage>();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.LISTNOTIFICATIONBYVENDORPAGEVIEW, VendorID, PageNo, Rows);
            items = DataTableHelper.ConvertDataTable<PushNotificationMessage>(ds.Tables[0]);

            return items;
        }
        public List<PushNotificationMessage> GetNotificationByUser(long CustomerID, short PageNo, short Rows)
        {
            List<PushNotificationMessage> items = new List<PushNotificationMessage>();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.LISTNOTIFICATIONBYCUSTOMERPAGEVIEW, CustomerID, PageNo, Rows);
            items = DataTableHelper.ConvertDataTable<PushNotificationMessage>(ds.Tables[0]);

            return items;
        }


        public NotificationTemplate GetNotificationTemplate(long TemplateID)
        {
            NotificationTemplate items = new NotificationTemplate();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.SELECTNOTIFICATIONTEMPLATE, TemplateID);
            items = DataTableHelper.ConvertDataTable<NotificationTemplate>(ds.Tables[0]).FirstOrDefault();

            return items;
        }

        public bool IsReadNotification(long NotificationID)
        {

            var result = false;
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(dbroutine.DBStoredProc.ISREADNOtifICATION))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@NotificationID", NotificationID);


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
        public bool addNotificationMessage(PushNotificationMessage pushNotification)
        {

            var result = false;
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(dbroutine.DBStoredProc.SAVENOTIFICATION))
                    {
                        cmd.CommandType = CommandType.StoredProcedure; 
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@NotificationID", 0);
                        cmd.Parameters.AddWithValue("@OrderID", pushNotification.OrderID);
                        cmd.Parameters.AddWithValue("@SenderID", pushNotification.SenderID);
                        cmd.Parameters.AddWithValue("@ReceiverID", pushNotification.ReceiverID);
                        cmd.Parameters.AddWithValue("@NotificationTitle", pushNotification.NotificationTitle);
                        cmd.Parameters.AddWithValue("@NotificationMessage", pushNotification.NotificationMessage);
                        cmd.Parameters.AddWithValue("@NotificationEvent", 0);

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

        public string unReadCountNotification(long UserID)
        {

            string result = "";
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(dbroutine.DBStoredProc.UNREADNOTIFICATION))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@UserID", UserID);


                        con.Open();
                        result = Convert.ToString(cmd.ExecuteScalar());

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

        public VendorDetails GetVendorDetails(long VendorID, long OrderID)
        {
            VendorDetails items = new VendorDetails();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.GETVENDORDETAILS, VendorID, OrderID);
            items = DataTableHelper.ConvertDataTable<VendorDetails>(ds.Tables[0]).FirstOrDefault();

            return items;
        }
        public EmailNotificationTemplate GetEmailNotificationTemplate(long TemplateID)
        {
            EmailNotificationTemplate items = new EmailNotificationTemplate();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.SELECTEMAILNOTIFICATIONTEMPLATE, TemplateID);
            items = DataTableHelper.ConvertDataTable<EmailNotificationTemplate>(ds.Tables[0]).FirstOrDefault();

            return items;
        }
        public List<EmailNotificationHeader> GetMemberDetails(long VendorID, long OrderID)
        {
            List<EmailNotificationHeader> items = new List<EmailNotificationHeader>();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.GETMEMBERDETAILS, VendorID, OrderID);
            items = DataTableHelper.ConvertDataTable<EmailNotificationHeader>(ds.Tables[0]);

            return items;
        }
    }
}
