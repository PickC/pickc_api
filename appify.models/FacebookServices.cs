using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appify.models
{
    public partial class MetaProduct
    {
        public string CatalogID { get; set; }
        public string RetailerID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public string ImageUrl { get; set; }
        public string Brand { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; }
        public string Availability { get; set; }
    }

    public class EventRequest
    {
        public string EventName { get; set; }
        public string EventId { get; set; }       // optional, we generate if missing
        public IEnumerable<string> ContentIds { get; set; }
        public object[] Contents { get; set; }    // flexible contents structure
        public decimal? Value { get; set; }
        public string Currency { get; set; }
        public string Fbp { get; set; }
        public string Fbc { get; set; }
        // PII should not come raw from client ideally. If you must accept it, hash here.
        public string Email { get; set; }
        public string Phone { get; set; }
        // optional: client IP and user agent if available
    }
}
