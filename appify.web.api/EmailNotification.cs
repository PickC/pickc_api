using appify.models;
using appify.utility;
using System.Net.Mail;
using System.Net;
using System.Text;
using FirebaseAdmin.Messaging;
using appify.Business.Contract;
using System.IO;

namespace appify.web.api
{
    public class EmailNotification
    {
        public EmailNotification()
        {
            
        }

        public static bool SendEmail(Notifications notifications)
        {
            bool result = false;
            try
            {
                string fromMail = NotificationConfig.GMAIL_ID_FROM;
                string fromPassword = NotificationConfig.GMAIL_PASSWORD_FROM;

                MailMessage message = new MailMessage();
                message.From = new MailAddress(fromMail);
                message.Subject = notifications.EmailSubject;
                message.To.Add(new MailAddress(notifications.ToEmail));
                if (notifications.ToEmailCC != null && notifications.ToEmailCC != "")
                {
                    message.CC.Add(notifications.ToEmailCC);
                }
                if (notifications.ToEmailBCC != null && notifications.ToEmailBCC != "")
                {
                    message.Bcc.Add(notifications.ToEmailBCC);
                }

                message.Body = notifications.EmailBody;
                message.IsBodyHtml = true;
                message.SubjectEncoding = Encoding.UTF8;
                message.BodyEncoding = Encoding.UTF8;

                var smtpClient = new SmtpClient(NotificationConfig.SMTPCLIENT)
                {
                    Port = NotificationConfig.PORT,
                    Credentials = new NetworkCredential(fromMail, fromPassword),
                    EnableSsl = true,
                };

                smtpClient.EnableSsl = true;


                //var alternativeView = new AlternateView(Body, new System.Net.Mime.ContentType("text/html"));
                //string? emailBody = alternativeView.ToString();
                //emailBody = emailBody.Replace("{{Product}}", "Appify");
                //alternativeView = emailBody;
                //message.AlternateViews.Add(alternativeView);


                smtpClient.SendMailAsync(message);
                result = true;
                ////smtpClient.Send(message);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public static bool SendEmailNotification(Int64 TemplateID, Int64 MemberID, Int64 OrderID, INotificationBusiness notificationBusiness)
        {
            bool result = false;
            try
            {
                //TemplateID = 1010;
                int OrderQuantity = 0;
                EmailNotificationTemplate emailNotificationTemplate = notificationBusiness.GetEmailNotificationTemplate(TemplateID);
                List<EmailNotificationHeader> getEmailNotificationHeader = notificationBusiness.GetMemberDetails(MemberID, OrderID);
                foreach(var item in getEmailNotificationHeader)
                {
                    OrderQuantity += item.OrderQuantity;
                }

                Notifications notifications = new Notifications
                {
                    ToEmailCC = NotificationConfig.TO_BCC,
                    ToEmailBCC = NotificationConfig.TO_CC,
                    EmailSubject = emailNotificationTemplate.Subject.Replace("{{order_number}}", getEmailNotificationHeader[0].OrderNo.ToString()),
                    EmailTemplateURL = emailNotificationTemplate.TemplateURL,
                    ToEmail = getEmailNotificationHeader[0].EmailID
                };
                //notifications.ToEmail = "nkolweb@gmail.com";

                string path = notifications.EmailTemplateURL;
                string mailbody = string.Empty;
                using (StreamReader reader = new StreamReader(path))
                {
                    mailbody = reader.ReadToEnd();
                }
                if (TemplateID == 1000) ////Welcome Signup
                {
                    mailbody = mailbody.Replace("{{name}}", getEmailNotificationHeader[0].FirstName=="" ? "John" : getEmailNotificationHeader[0].FirstName);
                }
                if (TemplateID == 1001) ////Order Placement To Customer
                {
                    notifications.EmailSubject = notifications.EmailSubject.Replace("{{vendor_app_name}}", getEmailNotificationHeader[0].AppName.ToString());
                    mailbody = mailbody.Replace("{{name}}", getEmailNotificationHeader[0].FirstName.ToString())
                        .Replace("{{order_number}}", getEmailNotificationHeader[0].OrderNo.ToString())
                        .Replace("{{order_date}}", getEmailNotificationHeader[0].OrderDate.ToString())
                        .Replace("{{order_quantity}}", OrderQuantity.ToString())
                        .Replace("{{order_price}}",getEmailNotificationHeader[0].TotalAmount.ToString())
                        .Replace("{{delivery_address}}", getEmailNotificationHeader[0].shipping_address.ToString())
                        .Replace("{{vendor_app_name}}", getEmailNotificationHeader[0].AppName.ToString());
                }
                else if (TemplateID == 1002) ////Order Placement To Vendor
                {
                    mailbody = mailbody.Replace("{{vendor_name}}", getEmailNotificationHeader[0].FirstName.ToString())
                        .Replace("{{order_number}}", getEmailNotificationHeader[0].OrderNo.ToString())
                        .Replace("{{order_date}}", getEmailNotificationHeader[0].OrderDate.ToString())
                        .Replace("{{order_quantity}}", OrderQuantity.ToString())
                        .Replace("{{order_price}}", getEmailNotificationHeader[0].TotalAmount.ToString())
                        .Replace("{{delivery_address}}", getEmailNotificationHeader[0].shipping_address.ToString());
                }
                else if (TemplateID == 1003) ////Order Confirm
                {
                    mailbody = mailbody.Replace("{{name}}", getEmailNotificationHeader[0].FirstName.ToString())
                        .Replace("{{order_number}}", getEmailNotificationHeader[0].OrderNo.ToString())
                        .Replace("{{order_date}}", getEmailNotificationHeader[0].OrderDate.ToString())
                        .Replace("{{order_total}}", getEmailNotificationHeader[0].TotalAmount.ToString())
                        .Replace("{{tracking_number}}", getEmailNotificationHeader[0].TrackingNumber.ToString())
                        .Replace("{{delivery_date}}", getEmailNotificationHeader[0].DeliveredOn.ToString())
                        .Replace("{{vendor_app_name}}", getEmailNotificationHeader[0].AppName.ToString());

                }
                else if (TemplateID == 1004) ////Shipping Delivery Updates
                {
                    mailbody = mailbody.Replace("{{name}}", getEmailNotificationHeader[0].FirstName.ToString())
                        .Replace("{{order_number}}", getEmailNotificationHeader[0].OrderNo.ToString())
                        .Replace("{{carrier_name}}", getEmailNotificationHeader[0].CourierName.ToString())
                        .Replace("{{tracking_number}}", getEmailNotificationHeader[0].TrackingNumber.ToString())
                        .Replace("{{estimated_delivery_date}}", getEmailNotificationHeader[0].DeliveredOn.ToString())
                        .Replace("{{tracking_link}}", getEmailNotificationHeader[0].TrackURL.ToString())
                        .Replace("{{vendor_app_name}}", getEmailNotificationHeader[0].AppName.ToString());
                }
                else if (TemplateID == 1005) ////Delivery Updates
                {
                    mailbody = mailbody.Replace("{{name}}", getEmailNotificationHeader[0].FirstName.ToString())
                        .Replace("{{order_number}}", getEmailNotificationHeader[0].OrderNo.ToString())
                        .Replace("{{vendor_app_name}}", getEmailNotificationHeader[0].AppName.ToString());
                }
                else if (TemplateID == 1006) ////Delivery Confirmation
                {
                    mailbody = mailbody.Replace("{{name}}", getEmailNotificationHeader[0].FirstName.ToString())
                        .Replace("{{order_number}}", getEmailNotificationHeader[0].OrderNo.ToString())
                        .Replace("{{vendor_app_name}}", getEmailNotificationHeader[0].AppName.ToString());
                }
                else if (TemplateID == 1007) ////Delayed Shipment Notification
                {
                    mailbody = mailbody.Replace("{{name}}", getEmailNotificationHeader[0].FirstName.ToString())
                        .Replace("{{order_number}}", getEmailNotificationHeader[0].OrderNo.ToString())
                        .Replace("{{vendor_app_name}}", getEmailNotificationHeader[0].AppName.ToString());
                }
                else if (TemplateID == 1010) ////Order Cancellation Customer
                {
                    mailbody = mailbody.Replace("{{name}}", getEmailNotificationHeader[0].FirstName.ToString())
                        .Replace("{{order_number}}", getEmailNotificationHeader[0].OrderNo.ToString())
                        .Replace("{{vendor_app_name}}", getEmailNotificationHeader[0].AppName.ToString());
                }
                else if (TemplateID == 1011) ////Order Cancellation Vendor
                {
                    mailbody = mailbody.Replace("{{name}}", getEmailNotificationHeader[0].FirstName.ToString())
                        .Replace("{{order_number}}", getEmailNotificationHeader[0].OrderNo.ToString());
                }
                notifications.EmailBody = mailbody;
                if(notifications.ToEmail=="")
                {
                    notifications.ToEmail = "support@appi-fy.ai";
                }
                
                //if (notifications.ToEmail!="")
                //{
                    SendEmail(notifications);
               // }

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
