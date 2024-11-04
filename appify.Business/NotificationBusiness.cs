using appify.DataAccess.Contract;
using appify.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using appify.Business.Contract;
using System.Net.Http;
using appify.utility;
namespace appify.Business
{
    public partial class NotificationBusiness : INotificationBusiness
    {
        ///private readonly IWebHostEnvironment _webHostEnvironment;

        private INotificationRepository repository;
        public NotificationBusiness(INotificationRepository repository) { 
            this.repository = repository;
        }

        public object Server { get; private set; }

        public List<PushNotificationMessage> GetNotificationByVendor(long VendorID)
        {
            return repository.GetNotificationByVendor(VendorID);
        }
        public List<PushNotificationMessage> GetNotificationByUser(long CustomerID)
        {
            return repository.GetNotificationByUser(CustomerID);
        }

        public List<PushNotificationMessage> GetNotificationByVendor(long VendorID, short PageNo, short Rows)
        {
            return repository.GetNotificationByVendor(VendorID, PageNo, Rows);
        }
        public List<PushNotificationMessage> GetNotificationByUser(long CustomerID, short PageNo, short Rows)
        {
            return repository.GetNotificationByUser(CustomerID, PageNo, Rows);
        }

        public NotificationTemplate GetNotificationTemplate(long TemplateID)
        {
            return repository.GetNotificationTemplate(TemplateID);
        }
        public SMSNotificationTemplate GetSMSNotificationTemplate(long TemplateID)
        {
            return repository.GetSMSNotificationTemplate(TemplateID);
        }
        public bool IsReadNotification(long NotificationID)
        {
            return repository.IsReadNotification(NotificationID);
        }
        public bool addNotificationMessage(PushNotificationMessage pushNotification)
        {
            return repository.addNotificationMessage(pushNotification);
        }

        public string unReadCountNotification(long UserID)
        {
            return repository.unReadCountNotification(UserID);
        }
        public VendorDetails GetVendorDetails(long VendorID, long OrderID)
        {
            return repository.GetVendorDetails(VendorID, OrderID);
        }
        public EmailNotificationTemplate GetEmailNotificationTemplate(long TemplateID)
        {
            return repository.GetEmailNotificationTemplate(TemplateID);
        }
        public List<EmailNotificationHeader> GetMemberDetails(long MemberID, long OrderID)
        {
            return repository.GetMemberDetails(MemberID, OrderID);
        }
        public SMSSystemConfigSetting GetSMSSystemConfig()
        {
            return repository.GetSMSSystemConfig();
        }
        public List<SMSConfig> GetSMSConfig()
        {
            return repository.GetSMSConfig();
        }
        public List<EmailConfig> GetEmailConfig()
        {
            return repository.GetEmailConfig();
        }
    }
}
