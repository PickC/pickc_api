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
                
                //smtpClient.Send(message);
                result = true;
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
                notifications.ToEmail = "nkolweb@gmail.com";

                string path = notifications.EmailTemplateURL;
                string mailbody = string.Empty;
                using (StreamReader reader = new StreamReader(path))
                {
                    mailbody = reader.ReadToEnd();
                }
                if (TemplateID == 1000) ////Welcome Signup for Vendor
                {
                    mailbody = mailbody.Replace("{{vendor_name}}", getEmailNotificationHeader[0].FirstName.ToString());
                }
                if (TemplateID == 1001) ////Welcome Signup for Customer
                {
                    notifications.EmailSubject = notifications.EmailSubject.Replace("{{vendor_app_name}}", getEmailNotificationHeader[0].AppName.ToString());
                    mailbody = mailbody.Replace("{{name}}", getEmailNotificationHeader[0].FirstName.ToString())
                        .Replace("{{vendor_app_name}}", getEmailNotificationHeader[0].AppName.ToString())
                        .Replace("{{vendor_name}}", getEmailNotificationHeader[0].AppName.ToString());
                }
                if (TemplateID == 1002) ////Welcome Signup by Vendor for Opps Team
                {
                    notifications.ToEmail = NotificationConfig.To_OPPSTeam;
                    mailbody = mailbody.Replace("{{vendor_name}}", getEmailNotificationHeader[0].FirstName.ToString())
                        .Replace("{{vendor_number}}", getEmailNotificationHeader[0].MobileNo.ToString())
                        .Replace("{{vendor_email}}", getEmailNotificationHeader[0].EmailID.ToString())
                        .Replace("{{vendor_app_name}}", getEmailNotificationHeader[0].AppName.ToString());
                }
                if (TemplateID == 1003) ////Order Placement To Vendor
                {
                    notifications.EmailSubject = notifications.EmailSubject.Replace("{{vendor_app_name}}", getEmailNotificationHeader[0].AppName.ToString());
                    mailbody = mailbody.Replace("{{vendor_name}}", getEmailNotificationHeader[0].FirstName.ToString())
                        .Replace("{{order_number}}", getEmailNotificationHeader[0].OrderNo.ToString())
                        .Replace("{{order_date}}", getEmailNotificationHeader[0].OrderDate.ToString("dd/MM/yyyy hh:mm tt"))
                        .Replace("{{order_quantity}}", OrderQuantity.ToString())
                        .Replace("{{order_price}}",getEmailNotificationHeader[0].TotalAmount.ToString())
                        .Replace("{{delivery_address}}", getEmailNotificationHeader[0].shipping_address.ToString())
                        .Replace("{{vendor_app_name}}", getEmailNotificationHeader[0].AppName.ToString());
                }
                else if (TemplateID == 1004) ////Order Placement To Customer
                {
                    notifications.EmailSubject = notifications.EmailSubject.Replace("{{vendor_app_name}}", getEmailNotificationHeader[0].AppName.ToString());
                    mailbody = mailbody.Replace("{{name}}", getEmailNotificationHeader[0].FirstName.ToString())
                        .Replace("{{order_number}}", getEmailNotificationHeader[0].OrderNo.ToString())
                        .Replace("{{order_date}}", getEmailNotificationHeader[0].OrderDate.ToString("dd/MM/yyyy hh:mm tt"))
                        .Replace("{{order_quantity}}", OrderQuantity.ToString())
                        .Replace("{{order_price}}", getEmailNotificationHeader[0].TotalAmount.ToString())
                        .Replace("{{delivery_address}}", getEmailNotificationHeader[0].shipping_address.ToString())
                        .Replace("{{vendor_app_name}}", getEmailNotificationHeader[0].AppName.ToString());
                }
                if (TemplateID == 1005) ////Order Placement to Opps
                {
                    notifications.ToEmail = NotificationConfig.To_OPPSTeam;
                    notifications.EmailSubject = notifications.EmailSubject.Replace("{{vendor_app_name}}", getEmailNotificationHeader[0].AppName.ToString());
                    mailbody = mailbody.Replace("{{vendor_app_name}}", getEmailNotificationHeader[0].FirstName.ToString())
                        .Replace("{{order_number}}", getEmailNotificationHeader[0].OrderNo.ToString())
                        .Replace("{{order_date}}", getEmailNotificationHeader[0].OrderDate.ToString("dd/MM/yyyy hh:mm tt"))
                        .Replace("{{order_quantity}}", OrderQuantity.ToString())
                        .Replace("{{order_price}}", getEmailNotificationHeader[0].TotalAmount.ToString())
                        .Replace("{{customer_name}}", getEmailNotificationHeader[0].CustomerName.ToString())
                        .Replace("{{customer_email}}", getEmailNotificationHeader[0].CustomerEmail.ToString());
                }
                if (TemplateID == 1006) ////Order Confirmed by Vendor
                {
                    mailbody = mailbody.Replace("{{vendor_name}}", getEmailNotificationHeader[0].FirstName.ToString());
                }
                if (TemplateID == 1007) ////Order Confirmed by Vendor to Customer
                {
                    notifications.EmailSubject = notifications.EmailSubject.Replace("{{vendor_app_name}}", getEmailNotificationHeader[0].AppName.ToString());
                    mailbody = mailbody.Replace("{{name}}", getEmailNotificationHeader[0].FirstName.ToString())
                        .Replace("{{order_number}}", getEmailNotificationHeader[0].OrderNo.ToString())
                        .Replace("{{order_date}}", getEmailNotificationHeader[0].OrderDate.ToString("dd/MM/yyyy hh:mm tt"))
                        .Replace("{{order_quantity}}", OrderQuantity.ToString())
                        .Replace("{{delivery_address}}", getEmailNotificationHeader[0].shipping_address.ToString())
                        .Replace("{{order_price}}", getEmailNotificationHeader[0].TotalAmount.ToString())
                        .Replace("{{vendor_app_name}}", getEmailNotificationHeader[0].AppName.ToString());
                }
                if (TemplateID == 1008) ////Order Confirmed by Vendor to Opps
                {
                    notifications.ToEmail = NotificationConfig.To_OPPSTeam;
                    notifications.EmailSubject = notifications.EmailSubject.Replace("{{vendor_app_name}}", getEmailNotificationHeader[0].AppName.ToString());
                    mailbody = mailbody.Replace("{{vendor_name}}", getEmailNotificationHeader[0].FirstName.ToString())
                        .Replace("{{OrderDate}}", getEmailNotificationHeader[0].OrderDate.ToString())
                        .Replace("{{vendor_number}}", getEmailNotificationHeader[0].MobileNo.ToString())
                        .Replace("{{VendorWarehouseAddress}}", getEmailNotificationHeader[0].CompanyAddress1.ToString() +" " +getEmailNotificationHeader[0].CompanyAddress2.ToString());
                }
                if (TemplateID == 1009) ////Order has been cancelled by vendor
                {
                    mailbody = mailbody.Replace("{{vendor_name}}", getEmailNotificationHeader[0].FirstName.ToString())
                        .Replace("{{order_number}}", getEmailNotificationHeader[0].OrderNo.ToString())
                        .Replace("{{product_name}}", getEmailNotificationHeader[0].ProductName.ToString());
                }
                if (TemplateID == 1010) ////Order has been cancelled by vendor to cupdate customer
                {
                    mailbody = mailbody.Replace("{{vendor_app_name}}", getEmailNotificationHeader[0].AppName.ToString())
                        .Replace("{{order_number}}", getEmailNotificationHeader[0].OrderNo.ToString())
                        .Replace("{{name}}", getEmailNotificationHeader[0].FirstName.ToString());

                }
                if (TemplateID == 1011) ////Order has been cancelled by vendor to update Opps
                {
                    notifications.ToEmail = NotificationConfig.To_OPPSTeam;
                    notifications.EmailSubject = notifications.EmailSubject.Replace("{{vendor_app_name}}", getEmailNotificationHeader[0].AppName.ToString()).Replace("{{order_number}}", getEmailNotificationHeader[0].OrderNo.ToString());

                    mailbody = mailbody.Replace("{{product_name}}", getEmailNotificationHeader[0].ProductName.ToString())
                        .Replace("{{customer_name}}", getEmailNotificationHeader[0].CustomerName.ToString())
                        .Replace("{{customer_email}}", getEmailNotificationHeader[0].MobileNo.ToString())
                        .Replace("{{order_number}}", getEmailNotificationHeader[0].OrderNo.ToString())
                        .Replace("{{vendor_app_name}}", getEmailNotificationHeader[0].AppName.ToString())
                        .Replace("{{vendor_number}}", getEmailNotificationHeader[0].MobileNo.ToString())
                        .Replace("{{vendor_reason}}", getEmailNotificationHeader[0].Remarks.ToString());
                }
                if (TemplateID == 1012) ////Order has been cancelled by customer to update Vendor
                {
                    mailbody = mailbody.Replace("{{product_name}}", getEmailNotificationHeader[0].ProductName.ToString())
                        .Replace("{{customer_name}}", getEmailNotificationHeader[0].CustomerName.ToString())
                        .Replace("{{vendor_name}}", getEmailNotificationHeader[0].FirstName.ToString())
                        .Replace("{{order_number}}", getEmailNotificationHeader[0].OrderNo.ToString())
                        .Replace("{{vendor_app_name}}", getEmailNotificationHeader[0].AppName.ToString());
                }
                if (TemplateID == 1013) ////Order has been cancelled by Customer 
                {
                    mailbody = mailbody.Replace("{{product_name}}", getEmailNotificationHeader[0].ProductName.ToString())
                        .Replace("{{order_number}}", getEmailNotificationHeader[0].OrderNo.ToString())
                        .Replace("{{vendor_app_name}}", getEmailNotificationHeader[0].AppName.ToString())
                        .Replace("{{name}}", getEmailNotificationHeader[0].FirstName.ToString());
                }
                if (TemplateID == 1014) ////Order has been cancelled by Customer to update Opps
                {
                    notifications.ToEmail = NotificationConfig.To_OPPSTeam;
                    notifications.EmailSubject = notifications.EmailSubject.Replace("{{vendor_app_name}}", getEmailNotificationHeader[0].AppName.ToString()).Replace("{{order_number}}", getEmailNotificationHeader[0].OrderNo.ToString());

                    mailbody = mailbody.Replace("{{product_name}}", getEmailNotificationHeader[0].ProductName.ToString())
                        .Replace("{{customer_name}}", getEmailNotificationHeader[0].CustomerName.ToString())
                        .Replace("{{customer_email}}", getEmailNotificationHeader[0].MobileNo.ToString())
                        .Replace("{{order_number}}", getEmailNotificationHeader[0].OrderNo.ToString())
                        .Replace("{{vendor_app_name}}", getEmailNotificationHeader[0].AppName.ToString())
                        .Replace("{{vendor_number}}", getEmailNotificationHeader[0].MobileNo.ToString())
                        .Replace("{{customer_reason}}", getEmailNotificationHeader[0].Remarks.ToString());
                }
                notifications.EmailBody = mailbody;
                if(notifications.ToEmail=="")
                {
                    notifications.ToEmail = "support@appi-fy.ai";
                }
                
                if (notifications.ToEmail!="")
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
