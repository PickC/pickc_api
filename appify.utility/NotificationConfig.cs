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

        public const string GMAIL_ID_FROM = "noreply@appi-fy.ai";
        public const string GMAIL_PASSWORD_FROM = "App1fyn0r3ply@2023";
        public const string SMTPCLIENT = "smtp.gmail.com";
        public const Int16 PORT = 587;
        public const string TO_BCC = "";//"ragsarma@gmail.com";//"aj@appi-fy.ai";  ////// On production Aj Email will be there
        public const string TO_CC = "";//"neella@appi-fy.ai";//"shipments@appi-fy.ai"; ///// On Production Shipments Email will be there
        public const string To_OPPSTeam = "support@appi-fy.ai";
        ////// Email Templates Files Path

    }
}
