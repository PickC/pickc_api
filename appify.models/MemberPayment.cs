using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appify.models
{
    public partial class VendorPayment
    {
        public Int64 PaymentID {  get; set; }
        public Int64 VendorID { get; set; }
        public Int32 SubscriptionType { get; set; }
        public decimal PaymentAmount { get; set; }
        public decimal TaxAmount {  get; set; }
        public decimal TotalAmount { get; set; }
        public string ReferenceNo { get; set; }
        public Int32 PaymentStatus {  get; set; }
        public DateTime CreatedOn { get; set; }
        public string name { get; set; }
        public string receipt {  get; set; }
    }
    public partial class VendorPaymentStatus
    {
        public Int64 PaymentID { get; set; }
        public Int64 VendorID { get; set; }
        public string ReferenceNo { get; set; }
    }
}
