using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appify.models
{
    public partial class OrderDetail
    {
        public Int64 ItemID { get; set; }

        public Int64 OrderID { get; set; }

        public Int64 ProductID { get; set; }

        public Int64 SellerID { get; set; }

        public Int32 Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public Int16 DiscountType { get; set; }

        public decimal DiscountAmount { get; set; }

        public decimal SellingPrice { get; set; }

        public bool IsCancel { get; set; }

        public bool IsDelivered { get; set; }

        public Int64? DeliveryID { get; set; }

        public DateTime? DeliverDate { get; set; }

        public DateTime? CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public Int64? CancelBy { get; set; }

        public Int64? PriceID { get; set; }
        public string? Size { get; set; }
        public decimal? Price { get; set; }
        public decimal? Weight { get; set; }
        public string? ProductDescription { get; set; }
        public string? HSNCode { get; set; }
        public string? Color { get; set; }
        public string? ImageName { get; set; }
    }

    public partial class OrderDetailNew
    {
        public Int64 ItemID { get; set; }

        //public Int64 OrderID { get; set; }

        //public Int64 ProductID { get; set; }

        //public Int64 SellerID { get; set; }

        public Int32 Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        //public Int16 DiscountType { get; set; }

        //public decimal DiscountAmount { get; set; }

        public decimal SellingPrice { get; set; }

        //public bool IsCancel { get; set; }

        //public bool IsDelivered { get; set; }

        //public Int64? DeliveryID { get; set; }

        //public DateTime? DeliverDate { get; set; }

        //public DateTime? CreatedOn { get; set; }

        //public DateTime? ModifiedOn { get; set; }

        //public Int64? CancelBy { get; set; }

        public Int64? PriceID { get; set; }
        public string? Size { get; set; }
        public decimal? Price { get; set; }
        public decimal? Weight { get; set; }
        public string? ProductDescription { get; set; }
        public string? HSNCode { get; set; }
        public string? Color { get; set; }
        public string? ImageName { get; set; }
    }


    public partial class OrderItem
    {
        public Int64 ItemID { get; set; }

        public Int64 OrderID { get; set; }

        public Int64 ProductID { get; set; }

        public Int64 SellerID { get; set; }

        public Int32 Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public Int16 DiscountType { get; set; }

        public decimal DiscountAmount { get; set; }

        public decimal SellingPrice { get; set; }
        public Int64? PriceID { get; set; }

    }
}
