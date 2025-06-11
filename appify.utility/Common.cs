using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json;
using System.Net.Http;
using appify.models;
using Microsoft.AspNetCore.Http;
using appify.Business.Contract;
using appify.Business;
using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;

namespace appify.utility
{
    public static class Common
    {
        public static string ConvertObjectToJson(object obj)
        {
            // Serialize the object to JSON
            string json = JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented);
            return json;
        }

        public static T ConvertJsonToObject<T>(string json)
        {
            // Deserialize JSON to object
            T obj = JsonConvert.DeserializeObject<T>(json);
            return obj;
        }

        public static readonly string SecretKey = "App1fyd3v3l0p3r";

        public static readonly string IMAGECLASSIFIER_URL = "https://appify-image-classifier.azurewebsites.net/classify-image/";

        public static readonly int IMAGE_SIZE = 5 * 1024 * 1024; /// 5 MB

        public static readonly string PhonePayGateWayURL = "https://api-preprod.phonepe.com/apis/pg-sandbox";

        public static readonly string OneDelhiveryCreateURL = "https://track.delhivery.com/api/backend/clientwarehouse/create/";
        public static readonly string OneDelhiveryEditURL = "https://track.delhivery.com/api/backend/clientwarehouse/edit/";

        /// <summary>
        /// Shiprocket Tracking API URL
        /// </summary>
        public static readonly string ShipRocketTrackingURL = "https://apiv2.shiprocket.in/v1/external/courier/track";

        public static readonly string ShiproketDeliveryTrackingURL = "https://shiprocket.co/tracking/";

        public static readonly string OneDelhiveryToken = "9c0afe4ffc2f6ffd70fa4d1ee3a652f7f033f81c";

        public static readonly string OneDelhiveryShipmentCost = "https://track.delhivery.com/api/kinko/v1/invoice/charges/.json";

        public static readonly string OnDeliveryPincodeService = "https://track.delhivery.com/c/api/pin-codes/json";

        public static readonly string RazorPayCreateAccount = "https://api.razorpay.com/v2/accounts";

        public static readonly string RazorpayPaymentTransfers = "https://api.razorpay.com/v1/payments/PAYMENT_ID/transfers";

        public static readonly string RazorPayKey = "rzp_test_OVkzHWQC4WRAMj";
        public static readonly string RazorPaySecret = "f0RBriXVQMJ5dwxphwDGlskH";

        //public static readonly string RazorPayKey = "rzp_live_uSkTSnmcPZUeVA";
        //public static readonly string RazorPaySecret = "nU2y93R7YEJv4QgWuKt3eptV";

        public static EventLogs UpdateEventLogs(string eventType, HttpRequest request, string url, Object? inputData, Object? outputData,
                                                string? eventLogStatus)
        {


            EventLogs eventLog;
            try
            {
                Int64 VendorID = request.Headers["VendorID"].Count > 0 ? Int64.Parse(request.Headers["VendorID"]) : 0;
                Int64 CustomerID = request.Headers["CustomerID"].Count > 0 ? Int64.Parse(request.Headers["CustomerID"]) : 0;
                string IPAddress = request.Headers["IPAddress"].Count > 0 ? request.Headers["IPAddress"] : "Not Found";
                string AppName = request.Headers["AppName"].Count > 0 ? request.Headers["AppName"] : "Not Found";
                string module = request.Headers["module"].Count > 0 ? request.Headers["module"] : "Not Found";
                string Version = request.Headers["appify-version"].Count > 0 ? request.Headers["appify-version"] : "Not Found";

                eventLog = new EventLogs
                {
                    EventType = eventType,
                    VendorID = VendorID,
                    CustomerID = CustomerID,
                    Source = url,
                    Module = module,
                    IPAddress = IPAddress,
                    EventLog = eventLogStatus,
                    InputJSON = Common.ConvertObjectToJson(inputData),
                    OutputJSON = Common.ConvertObjectToJson(outputData),
                    EventTime = DateTime.Now,
                    AppName = AppName,
                    Version = Version
                };

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return eventLog;
        }

        public static async Task<bool> UpdateEventLogsNew(string eventType, HttpRequest request, string url, Object? inputData, Object? outputData,
                                        string? eventLogStatus, IEventLogBusiness eventLogBusiness)
        {

            bool result = false;
            EventLogs eventLog;
            try
            {
                Int64 VendorID = request.Headers["VendorID"].Count > 0 ? Int64.Parse(request.Headers["VendorID"]) : 0;
                Int64 CustomerID = request.Headers["CustomerID"].Count > 0 ? Int64.Parse(request.Headers["CustomerID"]) : 0;
                string IPAddress = request.Headers["IPAddress"].Count > 0 ? request.Headers["IPAddress"] : "Not Found";
                string AppName = request.Headers["AppName"].Count > 0 ? request.Headers["AppName"] : "Not Found";
                string module = request.Headers["module"].Count > 0 ? request.Headers["module"] : "Not Found";
                string Version = request.Headers["appify-version"].Count > 0 ? request.Headers["appify-version"] : "Not Found";

                eventLog = new EventLogs
                {
                    EventType = eventType,
                    VendorID = VendorID,
                    CustomerID = CustomerID,
                    Source = url,
                    Module = module,
                    IPAddress = IPAddress,
                    EventLog = eventLogStatus,
                    InputJSON = Common.ConvertObjectToJson(inputData),
                    OutputJSON = Common.ConvertObjectToJson(outputData),
                    EventTime = DateTime.Now,
                    AppName = AppName,
                    Version = Version
                };

                result = eventLogBusiness.eventLogAdd(eventLog);


            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public static string CheckIPAddress(HttpContext httpContext, string[] allowedCountries)
        {
            var result = "";
            var ipAddress = "";
            try
            {
                ipAddress = httpContext.Connection.RemoteIpAddress?.ToString();
                if (ipAddress != null)
                {
                    if (ipAddress == "::1")
                        ipAddress = "127.0.0.1";   // ::1 is an IPV6 lookback address when testing in local host!

                    var ip2location = new IP2Location.Component();
                    var location = ip2location.IPQuery(ipAddress);
                    if (!allowedCountries.Contains(location.CountryShort))
                    {
                        result = "Access Denied! for country " + location.CountryLong;
                    }
                    else
                    {
                        result = "Access allowed";
                    }
                }
                else
                {
                    result = "Invalid IP Address";
                }
            }
            catch (Exception ex)
            {
                result = ex.ToString();
            }
            return result + "-" + Convert.ToString(ipAddress);
        }



        public static string GenerateOTP(string secretKey)
        {
            //
            var OTPSecretKey = secretKey;

            var password = "";
            Random r = new Random();
            int keyLength = OTPSecretKey.Length;
            for (var i = 0; i < 6; i++)
            {
                password += OTPSecretKey[r.Next(0, keyLength)];
            }
            return password;
        }

        //public static string GenerateRandomPassword(int length = 12)
        //{
        //    const string validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789#$%&@";
        //    var random = new Random();

        //    return new string(
        //        Enumerable.Repeat(validChars, length)
        //                  .Select(s => s[random.Next(s.Length)])
        //                  .ToArray()
        //    );
        //}


        public static string GenerateRandomPassword(int length = 12)
        {
            //if (length < 8)
            //    throw new ArgumentException("Password length must be at least 8 characters", nameof(length));

            const string lowerCase = "abcdefghkmnpqrstuvwxyz";
            const string upperCase = "ABCDEFGHJKLMNPQRSTUVWXYZ";
            const string digits = "0123456789";
            const string specialChars = "!#%&*@?";

            // Ensure at least one character from each group
            var passwordChars = new List<char>
            {
                lowerCase[RandomNumberGenerator.GetInt32(lowerCase.Length)],
                upperCase[RandomNumberGenerator.GetInt32(upperCase.Length)],
                digits[RandomNumberGenerator.GetInt32(digits.Length)],
                specialChars[RandomNumberGenerator.GetInt32(specialChars.Length)]
            };

            // Combine all valid characters
            var allValidChars = lowerCase + upperCase + digits + specialChars;

            // Fill the rest with random characters
            for (int i = passwordChars.Count; i < length; i++)
            {
                passwordChars.Add(allValidChars[RandomNumberGenerator.GetInt32(allValidChars.Length)]);
            }

            // Shuffle the result to avoid predictable patterns
            for (int i = 0; i < passwordChars.Count; i++)
            {
                int randomIndex = RandomNumberGenerator.GetInt32(passwordChars.Count);
                (passwordChars[i], passwordChars[randomIndex]) = (passwordChars[randomIndex], passwordChars[i]);
            }

            return new string(passwordChars.ToArray());
        }






        public static async Task<string?> GetExpectedDeliveryDateAsync(string awbNumber)
        {
            using var client = new HttpClient();
            var token = OneDelhiveryToken;
            client.DefaultRequestHeaders.Add("Authorization", $"Token {token}");

            var response = await client.GetAsync($"https://track.delhivery.com/api/v1/packages/json/?waybill={awbNumber}");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var data = JObject.Parse(json);

            var edd = data["ShipmentData"]?[0]?["Shipment"]?["ExpectedDeliveryDate"]?.ToString();
            return edd;
        }
    }
    public class TokenObject
    {

        public long UserID { get; set; }
        public string DeviceID { get; set; }
        // public string Role {  get; set; }

    }

}
