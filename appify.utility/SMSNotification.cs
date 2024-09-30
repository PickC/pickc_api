using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace appify.utility
{
    public static class SMSNotification
    {
        public static string SendSMSNotification(string MobileNo, string OTPCode)
        {
            string myURI = "https://api.bulksms.com/v1/messages";
            //string myData = "{ \"to\": \"+919810722979\", \"body\": \"Hello World!\"}";
            string myData = "{\"to\": \"+91" + MobileNo+ "\", \"body\":\"" + OTPCode + " is your verification code.\"}";
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
