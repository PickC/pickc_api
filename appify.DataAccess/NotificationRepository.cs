/*
 * Company: AppifyRetail.
 * Author: Gurjeet
 * Version: 1.1
 * Date: 2024-09-01
 * Description:
*/
using appify.DataAccess.Contract;
using appify.models;
using appify.utility;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

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
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, dbroutine.DBStoredProc.LISTNOTIFICATIONBYVENDOR, VendorID);
                items = DataTableHelper.ConvertDataTable<PushNotificationMessage>(ds.Tables[0]);
            }
            return items;
        }
        public List<PushNotificationMessage> GetNotificationByUser(long CustomerID)
        {
            List<PushNotificationMessage> items = new List<PushNotificationMessage>();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, dbroutine.DBStoredProc.LISTNOTIFICATIONBYCUSTOMER, CustomerID);
                items = DataTableHelper.ConvertDataTable<PushNotificationMessage>(ds.Tables[0]);
            }
            return items;
        }

        public List<PushNotificationMessage> GetNotificationByVendor(long VendorID, short PageNo, short Rows)
        {
            List<PushNotificationMessage> items = new List<PushNotificationMessage>();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, dbroutine.DBStoredProc.LISTNOTIFICATIONBYVENDORPAGEVIEW, VendorID, PageNo, Rows);
                items = DataTableHelper.ConvertDataTable<PushNotificationMessage>(ds.Tables[0]);
            }
            return items;
        }
        public List<PushNotificationMessage> GetNotificationByUser(long CustomerID, short PageNo, short Rows)
        {
            List<PushNotificationMessage> items = new List<PushNotificationMessage>();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, dbroutine.DBStoredProc.LISTNOTIFICATIONBYCUSTOMERPAGEVIEW, CustomerID, PageNo, Rows);
                items = DataTableHelper.ConvertDataTable<PushNotificationMessage>(ds.Tables[0]);
            }
            return items;
        }


        public NotificationTemplate GetNotificationTemplate(long TemplateID)
        {
            NotificationTemplate items = new NotificationTemplate();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, dbroutine.DBStoredProc.SELECTNOTIFICATIONTEMPLATE, TemplateID);
                items = DataTableHelper.ConvertDataTable<NotificationTemplate>(ds.Tables[0]).FirstOrDefault();
            }
            return items;
        }
        public SMSNotificationTemplate GetSMSNotificationTemplate(long TemplateID)
        {
            SMSNotificationTemplate items = new SMSNotificationTemplate();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, dbroutine.DBStoredProc.SELECTSMSNOTIFICATIONTEMPLATE, TemplateID);
                items = DataTableHelper.ConvertDataTable<SMSNotificationTemplate>(ds.Tables[0]).FirstOrDefault();
            }
            return items;
        }

        public WhatsAppTemplate GetWhatsAppNotificationTemplate(long TemplateID)
        {
            WhatsAppTemplate items = new WhatsAppTemplate();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, dbroutine.DBStoredProc.SELECTWHATSAPPNOTIFICATIONTEMPLATE, TemplateID);
                items = DataTableHelper.ConvertDataTable<WhatsAppTemplate>(ds.Tables[0]).FirstOrDefault();
            }
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
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, dbroutine.DBStoredProc.GETVENDORDETAILS, VendorID, OrderID);
                items = DataTableHelper.ConvertDataTable<VendorDetails>(ds.Tables[0]).FirstOrDefault();
            }
            return items;
        }
        public EmailNotificationTemplate GetEmailNotificationTemplate(long TemplateID)
        {
            EmailNotificationTemplate items = new EmailNotificationTemplate();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, dbroutine.DBStoredProc.SELECTEMAILNOTIFICATIONTEMPLATE, TemplateID);
                items = DataTableHelper.ConvertDataTable<EmailNotificationTemplate>(ds.Tables[0]).FirstOrDefault();
            }
            return items;
        }
        public List<EmailNotificationHeader> GetMemberDetails(long VendorID, long OrderID)
        {
            List<EmailNotificationHeader> items = new List<EmailNotificationHeader>();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, dbroutine.DBStoredProc.GETMEMBERDETAILS, VendorID, OrderID);
                items = DataTableHelper.ConvertDataTable<EmailNotificationHeader>(ds.Tables[0]);
            }
            return items;
        }
        public SMSSystemConfigSetting GetSMSSystemConfig()
        {
            SMSSystemConfigSetting items = new SMSSystemConfigSetting();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, dbroutine.DBStoredProc.SELECTSMSCONFIGSETTING);
                items = DataTableHelper.ConvertDataTable<SMSSystemConfigSetting>(ds.Tables[0]).FirstOrDefault();
            }
            return items;
        }

        public List<SMSConfig> GetSMSConfig()
        {
            List<SMSConfig> items = new List<SMSConfig>();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, dbroutine.DBStoredProc.SELECTSMSCONFIGSETTING);
                items = DataTableHelper.ConvertDataTable<SMSConfig>(ds.Tables[0]).ToList();
            }
            return items;
        }
        public List<EmailConfig> GetEmailConfig()
        {
            List<EmailConfig> items = new List<EmailConfig>();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, dbroutine.DBStoredProc.SELECTEMAILCONFIGSETTING);
                items = DataTableHelper.ConvertDataTable<EmailConfig>(ds.Tables[0]).ToList();
            }
            return items;
        }
        public List<EmailConfig> GetAlertHeader()
        {
            List<EmailConfig> items = new List<EmailConfig>();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, dbroutine.DBStoredProc.EMAILSERVERALERT);
                items = DataTableHelper.ConvertDataTable<EmailConfig>(ds.Tables[0]);
            }
            return items;
        }
        public bool UpdateSMSAlert(bool smsalert, bool smsalertemail)
        {

            var result = false;
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(dbroutine.DBStoredProc.UPDATESMSALERT))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@SMSALert", smsalert);
                        cmd.Parameters.AddWithValue("@SMSAlertEMail", smsalertemail);


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
        public List<EmailUserHeader> GetUserDetails(string EmailID, bool isAcceptedUsers)
        {
            List<EmailUserHeader> items = new List<EmailUserHeader>();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, dbroutine.DBStoredProc.GETUSERDETAILS, EmailID, isAcceptedUsers);
                items = DataTableHelper.ConvertDataTable<EmailUserHeader>(ds.Tables[0]);
            }
            return items;
        }
    }
}
