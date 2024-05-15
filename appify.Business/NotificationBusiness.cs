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
        
        public Notifications SendEmail(Notifications notifications)
        {
            /// return this.repository.SendEmail(notifications);
            /// 
            try
            {
                string path = notifications.EmailTemplateURL;

                string fromMail = NotificationConfig.GMAIL_ID_FROM;
                string fromPassword = NotificationConfig.GMAIL_PASSWORD_FROM;

                MailMessage message = new MailMessage();
                message.From = new MailAddress(fromMail);
                message.Subject = notifications.EmailSubject;
                message.To.Add(new MailAddress(notifications.ToEmail));
                if (notifications.ToEmailCC != null && notifications.ToEmailCC!="")
                {
                    message.CC.Add(notifications.ToEmailCC);
                }
                if (notifications.ToEmailBCC != null && notifications.ToEmailBCC!="")
                {
                    message.Bcc.Add(notifications.ToEmailBCC);
                }
                //string Body = System.IO.File.ReadAllText(path);
                //string filepath = Server.MapPath('temp.html');
                string mailbody = string.Empty;
                using (StreamReader reader = new StreamReader(path))
                {
                    mailbody = reader.ReadToEnd();
                }

                mailbody = mailbody.Replace("{{Name}}", notifications.EmailTemplae_ReplaceName);
                message.Body = mailbody;
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

                ///smtpClient.Send(message);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return notifications;
        }

        public NotificationTemplate GetNotificationTemplate(long TemplateID)
        {
            return repository.GetNotificationTemplate(TemplateID);
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
    }
}
