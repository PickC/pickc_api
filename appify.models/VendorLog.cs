using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace appify.models
{
	public partial class VendorLog
	{
		public VendorLog() { }


		public Int64  AuditID { get; set; }
		public Int64  VendorID { get; set; }
		public string  EventType { get; set; }
		public string  ChangedBy { get; set; }
		public DateTime  ChangedOn { get; set; }
		public string PayLoad { get; set; }
		public string  Source { get; set; }
		public string  IPAddress { get; set; }
	}
}




