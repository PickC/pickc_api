using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appify.models
{
    public class InvoiceHeader
    {
        public Int64 InvoiceID { get; set; }

        public string InvoiceNo { get; set; }

        public DateTime InvoiceDate { get; set; }

        public Int64 OrderID { get; set; }

        public Int64 MemberID { get; set; }

        public Int64 SellerID { get; set; }

        public decimal InvoiceAmount { get; set; }

        public decimal TaxAmount { get; set; }

        public decimal TotalAmount { get; set; }

        public DateTime CreatedOn { get; set; }

        public bool IsCancel { get; set; }

        public DateTime CancelledOn { get; set; }

        public string CancelRemarks { get; set; }
    }


    public class Invoice : InvoiceHeader
    {


        public List<InvoiceDetail> items { get; set; }

    }



    public class InvoiceReport
    {

        public InvoiceReport()
        {
            InvoiceItems = new List<InvoiceItemReport>();
        }
        public string InvoiceNo { get; set; }
        public string OrderNo { get; set; }
        public DateTime OrderDate { get; set; }
        public Int64 SellerID { get; set; }
        public Int64 MemberID { get; set; }
        public decimal InvoiceAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal? RoundOffAmount { get; set; }
        public string CompanyName { get; set; }
        public string CompanyAddress1 { get; set; }
        public string CompanyAddress2 { get; set; }
        public string CompanyState { get; set; }
        public string CompanyZipCode { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress1 { get; set; }
        public string CustomerAddress2 { get; set; }
        public string CustomerState { get; set; }
        public string CustomerZipCode { get; set; }

        public decimal DeliveryCost { get; set; }
        public decimal DeliveryGST { get; set; }
        public decimal DeliveryGSTPercent { get; set; }

        public string? SellerGSTIN { get; set; }
        public string? SellerPAN { get; set; }
        public string? MemberGSTIN { get; set; }
        public string? MemberPAN { get; set; }

        public string? PaymentType { get; set; }

        public string? PaymentReference { get; set; }

        public string? ReceiverName { get; set; }
        public string? ReceiverMobileNo { get; set; }


        public decimal? SubTotal { get; set; }
        public decimal? GrandTotal { get; set; }


        public List<InvoiceItemReport> InvoiceItems { get; set; }

    }


    public class InvoiceItemReport
    {

        public string ProductID { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal SellingAmount { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public string Brand { get; set; }


        public decimal DiscountAmount { get; set; }
        public decimal CGST { get; set; }
        public decimal SGST { get; set; }
        public decimal IGST { get; set; }

        public decimal? CGSTPercent { get; set; }
        public decimal? SGSTPercent { get; set; }
        public decimal? IGSTPercent { get; set; }

        public string? HSNCode { get; set; }

        public string DiscountTypeDescription { get; set; }

    }

}
