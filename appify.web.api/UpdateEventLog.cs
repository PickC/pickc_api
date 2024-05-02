using appify.Business;
using appify.Business.Contract;
using appify.models;
using appify.utility;
using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appify.web.api
{
    public class UpdateEventLog 
    {
        public static EventLogs UpdateEventLogs(string eventType, HttpRequest request, string url, Object inputData, Object outputData,
                                                string eventLogStatus)
        {
            EventLogs eventLog = new EventLogs();
            try
            {
                Int64 VendorID = request.Headers["VendorID"].Count > 0 ? Int64.Parse(request.Headers["VendorID"]) : 0;
                Int64 CustomerID = request.Headers["CustomerID"].Count > 0 ? Int64.Parse(request.Headers["CustomerID"]) : 0;
                string IPAddress = request.Headers["IPAddress"].Count > 0 ? request.Headers["IPAddress"] : "Not Found";
                string AppName = request.Headers["AppName"].Count > 0 ? request.Headers["AppName"] : "Not Found";

                eventLog.EventType = eventType;
                eventLog.VendorID = VendorID;
                eventLog.CustomerID = CustomerID;
                eventLog.Source = url;
                eventLog.Module = AppName;
                eventLog.IPAddress = IPAddress;
                eventLog.EventLog = eventLogStatus;
                eventLog.InputJSON = Common.ConvertObjectToJson(inputData);
                eventLog.OutputJSON = Common.ConvertObjectToJson(outputData);
                eventLog.EventTime = DateTime.Now;
                eventLog.AppName = AppName;

            }
            catch(Exception ex)
            {
                throw ex;
            }
            return eventLog;
        }

    }
}
