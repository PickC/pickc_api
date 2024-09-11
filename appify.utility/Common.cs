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

        public static readonly string RazorPayKey = "rzp_test_OVkzHWQC4WRAMj";
        public static readonly string RazorPaySecret = "f0RBriXVQMJ5dwxphwDGlskH";
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
                
                eventLog = new EventLogs
                {
                    EventType = eventType,
                    VendorID = VendorID,
                    CustomerID = CustomerID,
                    Source = url,
                    Module = AppName,
                    IPAddress = IPAddress,
                    EventLog = eventLogStatus,
                    InputJSON = Common.ConvertObjectToJson(inputData),
                    OutputJSON = Common.ConvertObjectToJson(outputData),
                    EventTime = DateTime.Now,
                    AppName = AppName
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

            bool result=false;
            EventLogs eventLog;
            try
            {
                Int64 VendorID = request.Headers["VendorID"].Count > 0 ? Int64.Parse(request.Headers["VendorID"]) : 0;
                Int64 CustomerID = request.Headers["CustomerID"].Count > 0 ? Int64.Parse(request.Headers["CustomerID"]) : 0;
                string IPAddress = request.Headers["IPAddress"].Count > 0 ? request.Headers["IPAddress"] : "Not Found";
                string AppName = request.Headers["AppName"].Count > 0 ? request.Headers["AppName"] : "Not Found";

                eventLog = new EventLogs
                {
                    EventType = eventType,
                    VendorID = VendorID,
                    CustomerID = CustomerID,
                    Source = url,
                    Module = AppName,
                    IPAddress = IPAddress,
                    EventLog = eventLogStatus,
                    InputJSON = Common.ConvertObjectToJson(inputData),
                    OutputJSON = Common.ConvertObjectToJson(outputData),
                    EventTime = DateTime.Now,
                    AppName = AppName
                };

                result = eventLogBusiness.eventLogAdd(eventLog);


            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
    }
}
