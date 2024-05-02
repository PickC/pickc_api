using appify.Business.Contract;
using appify.models;
using appify.DataAccess.Contract;
using appify.models;
using appify.utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appify.Business
{
    public class EventLogBusiness : IEventLogBusiness
    {
        private IEventLogRepository repository;
        public EventLogBusiness(IEventLogRepository repository)
        {
            this.repository = repository;
        }
        public bool eventLogAdd(EventLogs eventLog)
        {
            return repository.eventLogAdd(eventLog);
        }
        public bool eventLogRemove(long EventID)
        {
            return repository.eventLogRemove(EventID);
        }
        public EventLogs eventLogGet(long EventID)
        {
            return repository.eventLogGet(EventID);
        }
        public List<EventLogs> eventLogList()
        {
            return repository.eventLogList();
        }
        public List<EventLogs> eventLogListByVendor(long VendorID)
        {
            return repository.eventLogListByVendor(VendorID);
        }
        public List<EventLogs> eventLogListByCustomer(long CustomerID)
        {
            return repository.eventLogListByCustomer(CustomerID);
        }
    }
}
