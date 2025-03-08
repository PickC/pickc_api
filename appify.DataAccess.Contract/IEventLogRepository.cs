using appify.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appify.DataAccess.Contract
{
    public interface IEventLogRepository
    {
        public bool eventLogAdd(EventLogs eventLog);
        public bool eventLogRemove(long EventID);
        public EventLogs eventLogGet(long EventID);
        public List<EventLogs> eventLogList();
        public List<EventLogs> eventLogListByVendor(long VendorID);
        public List<EventLogs> eventLogListByCustomer(long CustomerID);
    }
}
