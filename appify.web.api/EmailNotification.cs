/*
 * Company: AppifyRetail.
 * Author: Gurjeet
 * Version: 1.1
 * Date: 2024-09-01
 * Description:
*/
using appify.models;
using appify.utility;
using System.Net.Mail;
using System.Net;
using System.Text;
using FirebaseAdmin.Messaging;
using appify.Business.Contract;
using System.IO;
using System;
using System.Reflection.PortableExecutable;
using appify.Business;
namespace appify.web.api
{
    public class EmailNotification
    {
        public readonly IEventLogBusiness eventLogBusiness;

        public static bool SendEmail(Notifications notifications, IFormFile file=null)
        {
            bool result = false;
            try
            {
                string filename = file.FileName.ToString();
                string fromMail = NotificationConfig.GMAIL_ID_FROM;
                string fromPassword = NotificationConfig.GMAIL_PASSWORD_FROM;

                MailMessage message = new MailMessage();
                message.From = new MailAddress(fromMail);
                message.Subject = notifications.EmailSubject;
                message.To.Add(new MailAddress(notifications.ToEmail));
                if (notifications.ToEmailCC != null && notifications.ToEmailCC != "")
                {
                    //message.CC.Add(notifications.ToEmailCC);
                }
                if (notifications.ToEmailBCC != null && notifications.ToEmailBCC != "")
                {
                    //message.Bcc.Add(notifications.ToEmailBCC);
                }
                if (file != null)
                {
                    byte[] data;
                    using (var br = new BinaryReader(file.OpenReadStream()))
                        data = br.ReadBytes((int)file.OpenReadStream().Length);
                    var stream = new MemoryStream(data);
                    message.Attachments.Add(new Attachment(stream, filename));

                }

                message.Body = notifications.EmailBody;
                message.IsBodyHtml = true;
                message.SubjectEncoding = Encoding.UTF8;
                message.BodyEncoding = Encoding.UTF8;

                var smtpClient = new SmtpClient(NotificationConfig.SMTPCLIENT)
                {
                    Port = NotificationConfig.PORT,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new NetworkCredential(fromMail, fromPassword),
                    EnableSsl = true,
                };

                smtpClient.EnableSsl = true;


                //var alternativeView = new AlternateView(Body, new System.Net.Mime.ContentType("text/html"));
                //string? emailBody = alternativeView.ToString();
                //emailBody = emailBody.Replace("{{Product}}", "Appify");
                //alternativeView = emailBody;
                //message.AlternateViews.Add(alternativeView);


                //smtpClient.SendMailAsync(message);
                
                smtpClient.Send(message);
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
                    //ToEmailCC = NotificationConfig.TO_BCC,
                    //ToEmailBCC = NotificationConfig.TO_CC,
                    EmailSubject = emailNotificationTemplate.Subject.Replace("{{order_number}}", getEmailNotificationHeader[0].OrderNo.ToString()),
                    EmailTemplateURL = emailNotificationTemplate.TemplateURL,
                    ToEmail = getEmailNotificationHeader[0].EmailID
                };
                ////notifications.ToEmail = "gurjeet@appi-fy.ai";////sharma@appi-fy.ai

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
                    mailbody = mailbody.Replace("{{vendor_name}}", getEmailNotificationHeader[0].AppName.ToString())
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
                    mailbody = mailbody.Replace("{{vendor_app_name}}", getEmailNotificationHeader[0].AppName.ToString())
                                       .Replace("{{order_number}}", getEmailNotificationHeader[0].OrderNo.ToString());
                }
                if (TemplateID == 1007) ////Order Confirmed by Vendor to Customer
                {
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
                    mailbody = mailbody.Replace("{{vendor_app_name}}", getEmailNotificationHeader[0].AppName.ToString())
                        .Replace("{{order_number}}", getEmailNotificationHeader[0].OrderNo.ToString())
                        .Replace("{{product_name}}", getEmailNotificationHeader[0].ProductName.ToString());
                }
                if (TemplateID == 1010) ////Order has been cancelled by vendor to update customer
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
                        .Replace("{{customer_number}}", getEmailNotificationHeader[0].CustomerMobileNo.ToString())
                        .Replace("{{order_number}}", getEmailNotificationHeader[0].OrderNo.ToString())
                        .Replace("{{vendor_app_name}}", getEmailNotificationHeader[0].AppName.ToString())
                        .Replace("{{vendor_number}}", getEmailNotificationHeader[0].MobileNo.ToString())
                        .Replace("{{vendor_reason}}", getEmailNotificationHeader[0].Remarks.ToString());
                }
                if (TemplateID == 1012) ////Order has been cancelled by customer to update Vendor
                {
                    mailbody = mailbody.Replace("{{product_name}}", getEmailNotificationHeader[0].ProductName.ToString())
                        .Replace("{{customer_name}}", getEmailNotificationHeader[0].CustomerName.ToString())
                        .Replace("{{order_number}}", getEmailNotificationHeader[0].OrderNo.ToString())
                        .Replace("{{vendor_app_name}}", getEmailNotificationHeader[0].AppName.ToString())
                        .Replace("{{cancel_reason}}", getEmailNotificationHeader[0].Remarks.ToString());
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
                        .Replace("{{customer_number}}", getEmailNotificationHeader[0].CustomerMobileNo.ToString())
                        .Replace("{{order_number}}", getEmailNotificationHeader[0].OrderNo.ToString())
                        .Replace("{{vendor_app_name}}", getEmailNotificationHeader[0].AppName.ToString())
                        .Replace("{{vendor_number}}", getEmailNotificationHeader[0].MobileNo.ToString())
                        .Replace("{{customer_reason}}", getEmailNotificationHeader[0].Remarks.ToString());
                }
                if(TemplateID == 1016) //// Order has been Shipped intimate Vendor about it
                {
                    mailbody = mailbody.Replace("{{product_name}}", getEmailNotificationHeader[0].ProductName.ToString())
                        .Replace("{{vendor_app_name}}", getEmailNotificationHeader[0].AppName.ToString())
                        .Replace("{{order_number}}", getEmailNotificationHeader[0].OrderNo.ToString())
                        .Replace("{{customer_name}}", getEmailNotificationHeader[0].CustomerName.ToString())
                        .Replace("{{carrier_name}}", getEmailNotificationHeader[0].DeliveryChannel.ToString())
                        .Replace("{{tracking_number}}", getEmailNotificationHeader[0].TrackingNumber.ToString())
                        .Replace("{{delivery_date}}", getEmailNotificationHeader[0].DeliveredOn.ToString());
                }
                if (TemplateID == 1017) //// Order has been Shipped intimate Customer about it
                {
                    mailbody = mailbody.Replace("{{name}}", getEmailNotificationHeader[0].FirstName.ToString())
                        .Replace("{{vendor_app_name}}", getEmailNotificationHeader[0].AppName.ToString())
                        .Replace("{{order_number}}", getEmailNotificationHeader[0].OrderNo.ToString())
                        .Replace("{{carrier_name}}", getEmailNotificationHeader[0].DeliveryChannel.ToString())
                        .Replace("{{tracking_number}}", getEmailNotificationHeader[0].TrackingNumber.ToString())
                        .Replace("{{delivery_date}}", getEmailNotificationHeader[0].DeliveredOn.ToString())
                        .Replace("{{tracking_link}}", getEmailNotificationHeader[0].TrackURL.ToString());
                }
                if (TemplateID == 1018) //// Order has been Shipped intimate Opps Team about it
                {
                    notifications.EmailSubject = notifications.EmailSubject.Replace("{{vendor_name}}", getEmailNotificationHeader[0].AppName.ToString()).Replace("{{order_number}}", getEmailNotificationHeader[0].OrderNo.ToString());
                    mailbody = mailbody.Replace("{{product_name}}", getEmailNotificationHeader[0].ProductName.ToString())
                        .Replace("{{vendor_app_name}}", getEmailNotificationHeader[0].AppName.ToString())
                        .Replace("{{order_number}}", getEmailNotificationHeader[0].OrderNo.ToString())
                        .Replace("{{customer_name}}", getEmailNotificationHeader[0].CustomerName.ToString())
                        .Replace("{{customer_number}}", getEmailNotificationHeader[0].CustomerMobileNo.ToString())
                        .Replace("{{carrier_name}}", getEmailNotificationHeader[0].DeliveryChannel.ToString())
                        .Replace("{{tracking_number}}", getEmailNotificationHeader[0].TrackingNumber.ToString())
                        .Replace("{{delivery_date}}", getEmailNotificationHeader[0].DeliveredOn.ToString());
                }
                if (TemplateID == 1019) //// Out for Delivery intimate Customer about it
                {
                    mailbody = mailbody.Replace("{{name}}", getEmailNotificationHeader[0].FirstName.ToString())
                        .Replace("{{order_number}}", getEmailNotificationHeader[0].OrderNo.ToString());
                }
                if (TemplateID == 1020) //// Order has been delivered intimate Vendor about it
                {
                    mailbody = mailbody.Replace("{{product_name}}", getEmailNotificationHeader[0].ProductName.ToString())
                        .Replace("{{vendor_app_name}}", getEmailNotificationHeader[0].AppName.ToString())
                        .Replace("{{order_number}}", getEmailNotificationHeader[0].OrderNo.ToString())
                        .Replace("{{customer_name}}", getEmailNotificationHeader[0].CustomerName.ToString())
                        .Replace("{{customer_email}}", getEmailNotificationHeader[0].CustomerEmail.ToString())
                        .Replace("{{customer_number}}", getEmailNotificationHeader[0].CustomerMobileNo.ToString())
                        .Replace("{{order_price}}", getEmailNotificationHeader[0].TotalAmount.ToString())
                        .Replace("{{delivery_date}}", getEmailNotificationHeader[0].DeliveredOn.ToString());
                }
                if (TemplateID == 1021) //// Order has been delivered intimate Customer about it
                {
                    mailbody = mailbody.Replace("{{name}}", getEmailNotificationHeader[0].FirstName.ToString())
                        .Replace("{{vendor_app_name}}", getEmailNotificationHeader[0].AppName.ToString())
                        .Replace("{{order_number}}", getEmailNotificationHeader[0].OrderNo.ToString());
                }
                if (TemplateID == 1022) //// Order has been delivered intimate Opps Team about it
                {
                    mailbody = mailbody.Replace("{{product_name}}", getEmailNotificationHeader[0].ProductName.ToString())
                        .Replace("{{vendor_app_name}}", getEmailNotificationHeader[0].AppName.ToString())
                        .Replace("{{order_number}}", getEmailNotificationHeader[0].OrderNo.ToString())
                        .Replace("{{customer_name}}", getEmailNotificationHeader[0].CustomerName.ToString())
                        .Replace("{{customer_email}}", getEmailNotificationHeader[0].CustomerEmail.ToString())
                        .Replace("{{customer_number}}", getEmailNotificationHeader[0].CustomerMobileNo.ToString())
                        .Replace("{{order_price}}", getEmailNotificationHeader[0].TotalAmount.ToString())
                        .Replace("{{delivery_date}}", getEmailNotificationHeader[0].DeliveredOn.ToString());
                }
                if (TemplateID == 1023) //// Order has been delayed intimate Vendor about it
                {
                    mailbody = mailbody.Replace("{{product_name}}", getEmailNotificationHeader[0].ProductName.ToString())
                        .Replace("{{vendor_app_name}}", getEmailNotificationHeader[0].AppName.ToString())
                        .Replace("{{order_number}}", getEmailNotificationHeader[0].OrderNo.ToString())
                        .Replace("{{customer_name}}", getEmailNotificationHeader[0].CustomerName.ToString())
                        .Replace("{{estimated_delivery_date}}", getEmailNotificationHeader[0].DeliveredOn.ToString())
                        .Replace("{{new_estimated_delivery_date}}", getEmailNotificationHeader[0].DeliveredOn.ToString());
                }
                if (TemplateID == 1024) //// Order has been delayed intimate Customer about it
                {
                    mailbody = mailbody.Replace("{{product_name}}", getEmailNotificationHeader[0].ProductName.ToString())
                        .Replace("{{name}}", getEmailNotificationHeader[0].FirstName.ToString())
                        .Replace("{{vendor_app_name}}", getEmailNotificationHeader[0].AppName.ToString())
                        .Replace("{{order_number}}", getEmailNotificationHeader[0].OrderNo.ToString())
                        .Replace("{{estimated_delivery_date}}", getEmailNotificationHeader[0].DeliveredOn.ToString())
                        .Replace("{{new_estimated_delivery_date}}", getEmailNotificationHeader[0].DeliveredOn.ToString());
                }
                if (TemplateID == 1025) //// Order has been delayed intimate Opps Team about it
                {
                    mailbody = mailbody.Replace("{{product_name}}", getEmailNotificationHeader[0].ProductName.ToString())
                        .Replace("{{vendor_app_name}}", getEmailNotificationHeader[0].AppName.ToString())
                        .Replace("{{order_number}}", getEmailNotificationHeader[0].OrderNo.ToString())
                        .Replace("{{customer_name}}", getEmailNotificationHeader[0].CustomerName.ToString())
                        .Replace("{{customer_number}}", getEmailNotificationHeader[0].CustomerMobileNo.ToString())
                        .Replace("{{estimated_delivery_date}}", getEmailNotificationHeader[0].DeliveredOn.ToString())
                        .Replace("{{new_estimated_delivery_date}}", getEmailNotificationHeader[0].DeliveredOn.ToString());
                }
                notifications.EmailBody = mailbody;
                if(notifications.ToEmail=="")
                {
                    notifications.ToEmail = NotificationConfig.To_OPPSTeam;
                }
                
                if (notifications.ToEmail!="")
                {
                    SendEmailNew(notifications, notificationBusiness);
                }

                result = true;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public static bool SendEmailNew(Notifications notifications, INotificationBusiness notificationBusiness)
        {
            bool result = false;
            try
            {
                List<EmailConfig> EmailSetting = notificationBusiness.GetEmailConfig();
                var gmailFrom = EmailSetting.Where(x => x.SettingKey == "EMAILUSERID").FirstOrDefault().SettingValue.ToString();
                var gmailPass = EmailSetting.Where(x => x.SettingKey == "EMAILPASSWORD").FirstOrDefault().SettingValue.ToString();
                var gmailClient = EmailSetting.Where(x => x.SettingKey == "EMAILCLIENT").FirstOrDefault().SettingValue.ToString();
                var gmailPort = EmailSetting.Where(x => x.SettingKey == "EMAILPORT").FirstOrDefault().SettingValue.ToString();
                var gmailToBCC = EmailSetting.Where(x => x.SettingKey == "EMAILTOBCC").FirstOrDefault().SettingValue.ToString();
                var gmailToCC = EmailSetting.Where(x => x.SettingKey == "EMAILTOCC").FirstOrDefault().SettingValue.ToString();
                var gmailToOPPS = EmailSetting.Where(x => x.SettingKey == "EMAILTOOPPS").FirstOrDefault().SettingValue.ToString();
                var isToBCC = Convert.ToBoolean(EmailSetting.Where(x => x.SettingKey == "ISTOBCC").FirstOrDefault().SettingValue.ToString());
                var isToCC = Convert.ToBoolean(EmailSetting.Where(x => x.SettingKey == "ISTOCC").FirstOrDefault().SettingValue.ToString());
                var isToOPPS = Convert.ToBoolean(EmailSetting.Where(x => x.SettingKey == "ISTOOPPS").FirstOrDefault().SettingValue.ToString());

                string fromMail = gmailFrom;
                string fromPassword = gmailPass;

                MailMessage message = new MailMessage();
                message.From = new MailAddress(fromMail);
                message.Subject = notifications.EmailSubject;
                message.To.Add(new MailAddress(notifications.ToEmail));
                if(isToCC== true)
                {
                    if (gmailToCC != null && gmailToCC != "")
                    {
                        message.CC.Add(gmailToCC);
                    }
                }
                if (isToBCC == true)
                {
                    if (gmailToBCC != null && gmailToBCC != "")
                    {
                        message.Bcc.Add(gmailToBCC);
                    }
                }
                message.Body = notifications.EmailBody;
                message.IsBodyHtml = true;
                message.SubjectEncoding = Encoding.UTF8;
                message.BodyEncoding = Encoding.UTF8;

                var smtpClient = new SmtpClient(gmailClient)
                {
                    Port = Convert.ToInt16(gmailPort),
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new NetworkCredential(fromMail, fromPassword),
                    EnableSsl = true,
                };

                smtpClient.EnableSsl = true;


                //var alternativeView = new AlternateView(Body, new System.Net.Mime.ContentType("text/html"));
                //string? emailBody = alternativeView.ToString();
                //emailBody = emailBody.Replace("{{Product}}", "Appify");
                //alternativeView = emailBody;
                //message.AlternateViews.Add(alternativeView);


                //smtpClient.SendMailAsync(message);

                smtpClient.Send(message);
                result = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        public static bool SendEmailAlert(ParamDownTimeAlert ItemData, Int64 TemplateID, string ToEmail, INotificationBusiness notificationBusiness)
        {
            bool result = false;
            try
            {
                string mailbody = string.Empty;
                EmailNotificationTemplate emailNotificationTemplate = notificationBusiness.GetEmailNotificationTemplate(TemplateID);
                Notifications notifications = new Notifications
                {
                    EmailSubject = emailNotificationTemplate.Subject.Replace("{{api_name}}", ItemData.Service.ToString()),
                    EmailTemplateURL = emailNotificationTemplate.TemplateURL,
                    ToEmail = ToEmail
                };
                string path = notifications.EmailTemplateURL;
                using (StreamReader reader = new StreamReader(path))
                {
                    mailbody = reader.ReadToEnd();
                }
                if (TemplateID == 1026) ////Server Down Alert
                {
                    mailbody = mailbody.Replace("{{api_name}}", ItemData.Service.ToString());
                }
                notifications.EmailBody = mailbody;

                List<EmailConfig> EmailSetting = notificationBusiness.GetEmailConfig();
                var gmailFrom = EmailSetting.Where(x => x.SettingKey == "EMAILUSERID").FirstOrDefault().SettingValue.ToString();
                var gmailPass = EmailSetting.Where(x => x.SettingKey == "EMAILPASSWORD").FirstOrDefault().SettingValue.ToString();
                var gmailClient = EmailSetting.Where(x => x.SettingKey == "EMAILCLIENT").FirstOrDefault().SettingValue.ToString();
                var gmailPort = EmailSetting.Where(x => x.SettingKey == "EMAILPORT").FirstOrDefault().SettingValue.ToString();

                string fromMail = gmailFrom;
                string fromPassword = gmailPass;

                MailMessage message = new MailMessage();
                message.From = new MailAddress(fromMail);
                message.Subject = notifications.EmailSubject;
                message.To.Add(new MailAddress(notifications.ToEmail));

                message.Body = notifications.EmailBody;
                message.IsBodyHtml = true;
                message.SubjectEncoding = Encoding.UTF8;
                message.BodyEncoding = Encoding.UTF8;

                var smtpClient = new SmtpClient(gmailClient)
                {
                    Port = Convert.ToInt16(gmailPort),
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new NetworkCredential(fromMail, fromPassword),
                    EnableSsl = true,
                };

                smtpClient.EnableSsl = true;

                smtpClient.Send(message);
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
