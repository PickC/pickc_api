using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appify.models
{
    public class InvoiceDetail
    {
        public Int16 ItemID { get; set; }

        public Int64 InvoiceID { get; set; }

        public Int64 ProductID { get; set; }

        public Int32 Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public Int16 DiscountType { get; set; }

        public decimal DiscountAmount { get; set; }
        public decimal CGST { get; set; }
        public decimal SGST { get; set; }
        public decimal IGST { get; set; }

        public decimal TaxAmount { get; set; }

        public decimal SellingAmount { get; set; }
        public decimal SellingPrice { get; set; }
    }
}
