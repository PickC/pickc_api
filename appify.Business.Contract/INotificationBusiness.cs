using appify.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appify.Business.Contract
{
    public interface INotificationBusiness
    {
        public Notifications SendEmail(Notifications notifications); 
        public List<PushNotificationMessage> GetNotificationByVendor(long VendorID);

        public List<PushNotificationMessage> GetNotificationByUser(long CustomerID);
        public NotificationTemplate GetNotificationTemplate(long TemplateID);
        public bool IsReadNotification(long NotificationID);
        public bool addNotificationMessage(PushNotificationMessage pushNotification);
        public string unReadCountNotification(long UserID);
        public VendorDetails GetVendorDetails(long VendorID, long OrderID);
    }
}
