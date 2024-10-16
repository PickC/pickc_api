using appify.Business.Contract;
using appify.models;
using FirebaseAdmin.Messaging;
using System.Net;
using System.Text;

namespace appify.web.api
{
    public class SMSNotification
    {
        public SMSNotification()
        {

        }
        public static async Task<string> SendSMSNotificationMessage(Int64 TemplateID, Int64 MemberID, Int64 OrderID, string replaceTitle, INotificationBusiness notificationBusiness, string OTPValue="")
        {
            string responseBody = "";
            try
            {
                string MobileNo = string.Empty;
                SMSNotificationModel notificationModel = new SMSNotificationModel();
                //////SMSNotification Template
                SMSNotificationTemplate notificationTemplate = notificationBusiness.GetSMSNotificationTemplate(TemplateID);
                if(MemberID == 0)
                {
                    if (TemplateID == 1016) ////Send Verification OTP
                    {
                        MobileNo = replaceTitle;
                        notificationModel.Title = notificationTemplate.MessageTitle.ToString();
                        notificationModel.Body = notificationTemplate.MessageBody.Replace("{#var#}", OTPValue).ToString();
                    }
                }
                else if (MemberID != 0) ////// Send OTP Condition
                {
                    List<EmailNotificationHeader> getEmailNotificationHeader = notificationBusiness.GetMemberDetails(MemberID, OrderID);

                    notificationModel.Title = notificationTemplate.MessageTitle.Replace(replaceTitle, getEmailNotificationHeader[0].FirstName).Trim();
                

                    if (TemplateID == 1000) ////Welcome Signup for Vendor
                    {
                        notificationModel.Body = notificationTemplate.MessageBody.Replace("{{vendor_name}}", getEmailNotificationHeader[0].FirstName.ToString());
                    }
                    else if (TemplateID == 1017) ////Welcome Signup for Customer
                    {
                        notificationModel.Body = notificationTemplate.MessageBody.Replace("{{vendor_app_name}}", getEmailNotificationHeader[0].AppName.ToString());
                    }
                    else if (TemplateID == 1008) ////Order Status Change
                    {
                        notificationModel.Body = notificationTemplate.MessageBody.Replace("#<order No>", getEmailNotificationHeader[0].OrderNo.Replace("OD", "")).Trim()
                            .Replace("<Delivery Date>", getEmailNotificationHeader[0].DeliveredOn.ToString()).Trim()
                            .Replace("<Tracking Link>", getEmailNotificationHeader[0].TrackURL.ToString()).Trim();
                    }
                    else if (TemplateID == 1012) ////Refund Processed
                    {
                        notificationModel.Body = notificationTemplate.MessageBody.Replace("#<order No>", getEmailNotificationHeader[0].OrderNo.Replace("OD","")).Trim()
                           .Replace("<date range>", "").Trim();
                    }
                    else if (TemplateID == 1013) ////Order Received
                    {
                        notificationModel.Body = notificationTemplate.MessageBody.Replace("#<order No>", getEmailNotificationHeader[0].OrderNo.Replace("OD", "")).Trim()
                       .Replace("<Tracking Link>", "").Trim();
                    }
                    else if (TemplateID == 1015) ////Back-in-Stock Notification
                    {
                        notificationModel.Body = notificationTemplate.MessageBody.Replace("<product page link>", "").Trim();
                    }
                    else
                    {
                        notificationModel.Body = notificationTemplate.MessageBody.Replace("#<order No>", getEmailNotificationHeader[0].OrderNo.Replace("OD", "")).Trim();
                    }
                    MobileNo = getEmailNotificationHeader[0].MobileNo.ToString();
                }
                List<SMSConfig> SMSSettings = notificationBusiness.GetSMSConfig();
                var userID = SMSSettings.Where(x => x.SettingKey == "SMSUSERID").FirstOrDefault().SettingValue.ToString();
                var password = SMSSettings.Where(x => x.SettingKey == "SMSPASSWORD").FirstOrDefault().SettingValue.ToString();
                var sender = SMSSettings.Where(x => x.SettingKey == "SMSHEADER").FirstOrDefault().SettingValue.ToString();
                var peid = SMSSettings.Where(x => x.SettingKey == "SMSENTITYID").FirstOrDefault().SettingValue.ToString();

                using (var client = new HttpClient())
                {

                    var BaseUri = new Uri(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("BULKSMSCredentials:url").Value);
                    var parameters = new Dictionary<string, string>();
                    parameters["userid"] = userID;
                    parameters["password"] = password;
                    parameters["sender"] = sender;
                    parameters["mobileno"] = MobileNo;
                    parameters["msg"] = notificationModel.Title + ' ' + notificationModel.Body;
                    parameters["peid"] = peid;
                    parameters["tpid"] = notificationTemplate.SMSTemplateID.ToString();

                    var response = await client.PostAsync(BaseUri, new FormUrlEncodedContent(parameters));

                    if (response.IsSuccessStatusCode)
                    {
                        responseBody = await response.Content.ReadAsStringAsync();
                    }
                    else
                    {
                        responseBody = response.StatusCode.ToString();
                    }

                }
                //responseBody = true;

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return responseBody;
        }

        public static string SendSMSNotification(string MobileNo, string OTPCode)
        {
            string myURI = "https://api.bulksms.com/v1/messages";
            //string myData = "{ \"to\": \"+919810722979\", \"body\": \"Hello World!\"}";
            string myData = "{\"to\": \"+91" + MobileNo + "\", \"body\":\"" + OTPCode + " is your verification code.\"}";
            //string myData = "{\"to\": \"+91" + MobileNo + "\", \"body\":\"" + OTPCode + " is your verification code.\", \"routingGroup\":\"ECONOMY\"}";
            ///////////// BULKSMS - USERNAME & PASSWORD

            // This URL is used for sending messages
            string result = string.Empty;

            // change these values to match your own account
            string myUsername = "appifydeveloper";
            string myPassword = "App1fyd3v3l0p#r";

            // the details of the message we want to send
            //string myData = "{to: \"+919810722979\", body:\"Hello Mr. Smith!\"}";

            // build the request based on the supplied settings
            var request = WebRequest.Create(myURI);

            // supply the credentials
            request.Credentials = new NetworkCredential(myUsername, myPassword);
            request.PreAuthenticate = true;
            // we want to use HTTP POST
            request.Method = "POST";
            // for this API, the type must always be JSON
            request.ContentType = "application/json";

            // Here we use Unicode encoding, but ASCIIEncoding would also work
            var encoding = new UnicodeEncoding();
            var encodedData = encoding.GetBytes(myData);

            // Write the data to the request stream
            var stream = request.GetRequestStream();
            stream.Write(encodedData, 0, encodedData.Length);
            stream.Close();

            // try ... catch to handle errors nicely
            try
            {
                // make the call to the API
                var response = request.GetResponse();

                // read the response and print it to the console
                var reader = new StreamReader(response.GetResponseStream());
                result = reader.ReadToEnd();
            }
            catch (WebException ex)
            {
                // show the general message
                result = "An error occurred:" + ex.Message;

                // print the detail that comes with the error
                var reader = new StreamReader(ex.Response.GetResponseStream());
                result = "Error details:" + reader.ReadToEnd();
            }
            return result;
        }
    }
}
