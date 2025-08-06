using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appify.models
{
    public class EventLogs
    {
        public Int64 EventID { get; set; }
        public string EventType { get; set; }
        public Int64 VendorID { get; set; }
        public Int64 CustomerID { get; set; }
        public string Source { get; set; }
        public string Module { get; set; }
        public string IPAddress { get; set; }
        public string EventLog { get; set; }
        public string InputJSON { get; set; }
        public string OutputJSON { get; set; }
        public DateTime EventTime { get; set; }
        public string AppName { get; set; }
        public string Version { get; set; }
    }
}
