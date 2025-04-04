using appify.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appify.DataAccess.Contract
{
    public interface INotificationRepository
    {
        public Notifications SendEmail(Notifications notifications);

        public List<PushNotificationMessage> GetNotificationByVendor(long VendorID);
        public List<PushNotificationMessage> GetNotificationByUser(long CustomerID);

        //the below function is used for pagination API version : 1.1
        public List<PushNotificationMessage> GetNotificationByVendor(long VendorID, short PageNo, short Rows);
        public List<PushNotificationMessage> GetNotificationByUser(long CustomerID, short PageNo, short Rows);


        public NotificationTemplate GetNotificationTemplate(long TemplateID);
        public SMSNotificationTemplate GetSMSNotificationTemplate(long TemplateID);
        public bool IsReadNotification(long NotificationID);
        public bool addNotificationMessage(PushNotificationMessage pushNotification);
        public string unReadCountNotification(long UserID);
        public VendorDetails GetVendorDetails(long VendorID, long OrderID);
        public EmailNotificationTemplate GetEmailNotificationTemplate(long TemplateID);
        public List<EmailNotificationHeader> GetMemberDetails(long MemberID, long OrderID);
        public SMSSystemConfigSetting GetSMSSystemConfig();
        public List<SMSConfig> GetSMSConfig();
        public List<EmailConfig> GetEmailConfig();
        public List<EmailConfig> GetAlertHeader();
        public bool UpdateSMSAlert(bool smsalert, bool smsalertemail);
        public List<EmailUserHeader> GetUserDetails(string EmailID,bool isAcceptedUsers=true);
    }
}
