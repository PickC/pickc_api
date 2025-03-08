using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appify.models
{
    public partial class Address
    {
        public Int64 AddressID { get; set; }

        public Int64 LinkID { get; set; }

        public Int16 AddressType { get; set; }

        public string? HouseNo { get; set; }
        public string? Address1 { get; set; }

        public string? Address2 { get; set; }

        public string? Landmark { get; set; }
        public string? City { get; set; }

        public string? ZipCode { get; set; }

        public string? AlternateNo { get; set; }

        public string? State { get; set; }

        public string? Country { get; set; }

        public decimal Latitude { get; set; }

        public decimal Longitude { get; set; }

        public bool IsDefault { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime ModifiedOn { get; set; }

        public string LocationID { get; set; }
    }

    public partial class AddressObj
    {
        public string name { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public string pin { get; set; }
        public string return_address { get; set; }
        public string return_pin { get; set; }
        public string return_city { get; set; }
        public string return_state { get; set; }
        public string return_country { get; set; }

    }
}
