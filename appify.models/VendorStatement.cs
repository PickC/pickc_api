using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appify.models
{
    public partial class VendorStatement
    {
        public long VendorID { get; set; }
        public string VendorName { get; set; }
        public string Address { get; set; }
        public string EmailID { get; set; }
        public string ReportPeriod { get; set; }
        public string SettlementDate { get; set; }
        public string PaymentReference { get; set; }
        public string AccountNumber { get; set; }
        public List<VendorStatementData> Orders { get; set; }
    }

    public partial class VendorStatementData
    {
        public Int64 OrderID { get; set; }
        public string OrderNo { get; set; }
        public string OrderDate { get; set; }
        public string OrderStatus { get; set; }
        public decimal OrderAmount{ get; set; }
        public decimal DeliveryCost { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TransactionCharges { get; set; }
        public decimal TransactionChargesGST { get; set; }
        public decimal TotalGST { get; set; }
        public decimal AppifyCommission { get; set; }
        public decimal CommissionGST { get; set; }
        public decimal TCS { get; set; }
        public decimal PayOut { get; set; }
    }

}
