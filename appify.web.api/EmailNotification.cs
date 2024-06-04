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
                EmailNotificationTemplate emailNotificationTemplate = notificationBusiness.GetEmailNotificationTemplate(TemplateID);
                EmailNotificationHeader getEmailNotificationHeader = notificationBusiness.GetMemberDetails(MemberID, OrderID);
                Notifications notifications = new Notifications
                {
                    ToEmailCC = NotificationConfig.TO_BCC,
                    ToEmailBCC = NotificationConfig.TO_CC,
                    EmailSubject = emailNotificationTemplate.Subject,
                    EmailTemplateURL = emailNotificationTemplate.TemplateURL,
                    ToEmail = getEmailNotificationHeader.EmailID
                };
                ////notifications.ToEmail = "nkolweb@gmail.com";

                string path = notifications.EmailTemplateURL;
                string mailbody = string.Empty;
                using (StreamReader reader = new StreamReader(path))
                {
                    mailbody = reader.ReadToEnd();
                }
                if (TemplateID == 1000) ////Welcome Signup
                {
                    mailbody = mailbody.Replace("{{name}}", getEmailNotificationHeader.FirstName);
                }
                if (TemplateID == 1001) ////Order Confirmation
                {
                    mailbody = mailbody.Replace("{{name}}", getEmailNotificationHeader.FirstName)
                        .Replace("{{order_number}}", getEmailNotificationHeader.OrderNo)
                        .Replace("{{order_date}}", getEmailNotificationHeader.OrderDate.ToString())
                        .Replace("{{order_total}}", Convert.ToString(getEmailNotificationHeader.TotalAmount))
                        //.Replace("{{tracking_number}}", getEmailNotificationHeader.TrackingNumber)
                        .Replace("{{delivery_date}}", getEmailNotificationHeader.DeliveredOn.ToString());
                }
                else if(TemplateID == 1002) ////Order Placement
                {
                    mailbody = mailbody.Replace("{{vendor_name}}", getEmailNotificationHeader.FirstName)
                        .Replace("{{product_name}}", getEmailNotificationHeader.ProductName)
                        .Replace("{{order_number}}", getEmailNotificationHeader.OrderNo)
                        .Replace("{{order_date}}", getEmailNotificationHeader.OrderDate.ToString())
                        .Replace("{{order_quantity}}", Convert.ToString(getEmailNotificationHeader.OrderQuantity))
                        .Replace("{{order_price}}", Convert.ToString(getEmailNotificationHeader.TotalAmount));
                        //.Replace("{{delivery_address}}", getEmailNotificationHeader.DeliveryChannelDescription)
                        //.Replace("{{delivery_date}}", getEmailNotificationHeader.DeliveredOn.ToString())
                        //.Replace("{{contact_email}}", getEmailNotificationHeader.DeliveredOn.ToString())
                        //.Replace("{{contact_phone}}", getEmailNotificationHeader.DeliveredOn.ToString())
                        //.Replace("{{your_name}}", getEmailNotificationHeader.DeliveredOn.ToString())
                        //.Replace("{{your_position}}", getEmailNotificationHeader.DeliveredOn.ToString());
                }
                notifications.EmailBody = mailbody;
                if(notifications.ToEmail!="")
                {
                    SendEmail(notifications);
                }

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
