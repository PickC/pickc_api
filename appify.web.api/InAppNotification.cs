using appify.Business.Contract;
using appify.models;

namespace appify.web.api
{
    public class InAppNotification
    {
        public static bool SendInAppNotification(Int64 TemplateID, Int64 VendorID, Int64 MemberID, Int64 OrderID, string replaceTitle, INotificationBusiness notificationBusiness)
        {
            bool result = false;
            try
            {
                NotificationModel notificationModel = new NotificationModel();
                bool IsVendor = false;
                //////Notification Template
                NotificationTemplate notificationTemplate = notificationBusiness.GetNotificationTemplate(TemplateID);
                if (VendorID == 0)
                {
                    VendorID = MemberID;
                    IsVendor = true;
                }

                VendorDetails vendorDetails = notificationBusiness.GetVendorDetails(MemberID, OrderID);
                notificationModel.PlatformType = vendorDetails.PlatformType;

                notificationModel.Title = notificationTemplate.MessageTitle.Replace(replaceTitle, vendorDetails.FirstName).Trim();
                if (TemplateID == 1008) ////Order Status Change //// Requested to Stop this message
                {
                    notificationModel.Body = notificationTemplate.MessageBody.Replace("#<order No>", vendorDetails.OrderNo).Trim()
                        .Replace("<Delivery Date>", vendorDetails.DeliveredOn.ToString()).Trim()
                        .Replace("<Tracking Link>", vendorDetails.TrackURL.ToString()).Trim();
                }
                else if (TemplateID == 1012) ////Refund Processed
                {
                    notificationModel.Body = notificationTemplate.MessageBody.Replace("#<order No>", vendorDetails.OrderNo).Trim()
                       .Replace("<date range>", "").Trim();
                }
                else if (TemplateID == 1013) ////Order Received //// Requested to Stop this message
                {
                    notificationModel.Body = notificationTemplate.MessageBody.Replace("#<order No>", vendorDetails.OrderNo).Trim()
                   .Replace("<Tracking Link>", "").Trim();
                }
                else if (TemplateID == 1015) ////Back-in-Stock Notification //// Requested to Stop this message
                {
                    notificationModel.Body = notificationTemplate.MessageBody.Replace("<product page link>", "").Trim();
                }
                else
                {
                    notificationModel.Body = notificationTemplate.MessageBody.Replace("#<order No>", vendorDetails.OrderNo).Trim();
                }

                if (IsVendor == true)
                {
                    VendorID = 0;
                }

                PushNotificationMessage pushNotificationMessage = new PushNotificationMessage
                { OrderID = OrderID, SenderID = VendorID, ReceiverID = MemberID, NotificationTitle = notificationModel.Title, NotificationMessage = notificationModel.Body };
                notificationBusiness.addNotificationMessage(pushNotificationMessage);

                result = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

    }
}
