using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appify.utility
{
    public class NotificationConfig
    {

        ////Email Notification Configuration
        ///
        public const string GMAIL_ID_FROM = "noreply@appi-fy.ai";
        public const string GMAIL_PASSWORD_FROM = "cmprfzsbtadpfwgk";
        public const string SMTPCLIENT = "smtp.gmail.com";
        public const Int16 PORT = 587;
        public const string TO_BCC = "";
        public const string TO_CC = "";
        ////// Email Templates Files Path
        ///
        public const string ORDER_SAVE_EMAIL_TEMPLATE_URL = "wwwroot/EmailTemplates/welcome.html";




        /////// Email Subject Lines
        ///
        public const string ORDER_EMAIL_SUBJECT = "Your Order has been successfully placed";



        ////// FCM Body Messages
        ///
        public const string ORDER_FCM_SUBJECT = "Your Order has been successfully placed";

    }
}
