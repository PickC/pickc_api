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
}
