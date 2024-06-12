using System;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.Extensions;
using appify.models;
using appify.utility;
namespace appify.web.api
{
    public class SMSNotification
    {
        public SMSNotification()
        {
            
        }
        public static string SMSNotificationMessage()
        {
            string result = "";

            try
            {
                string accountSid = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("SMSNotification:accountSid").Value;
                string authToken = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("SMSNotification:authToken").Value;

                TwilioClient.Init(accountSid, authToken);
                var message = MessageResource.Create(
                    body: "Join Earth's mightiest heroes. Like Kevin Bacon.",
                    from: new Twilio.Types.PhoneNumber("+919885217825"),
                    to: new Twilio.Types.PhoneNumber("+919810722979")
                );
                
                result = message.Sid;
            }
            catch(Exception ex)
            {
                result = ex.Message.ToString();
            }
            return result;
        }
    }
}
