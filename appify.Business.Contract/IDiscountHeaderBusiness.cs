using appify.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appify.Business.Contract
{
    public interface IDiscountHeaderBusiness
    {
        public List<DiscountHeader> GetAll();
        public List<OrderDiscount> GetDiscountByVendor(Int64 VendorID);
        public List<OrderDiscount> GetDiscountListbyVendorRows(long vendorID, int pageNo, int rows);
        public DiscountHeader Get(Int64 DiscountID);
        public OrderDiscount GetDiscount(Int64 DiscountID);
        public Int64 GetDiscountCount(Int64 VendorID);
        public List<OrderDiscountDetail> GetOrderDiscountByVendor(Int64 VendorID);
        public DiscountHeader Save(DiscountHeader item);
        public OrderDiscount DiscountSave(OrderDiscount item);
        public bool Remove(Int64 DiscountID, Int64 ProductID);
        public bool DiscountRemove(Int64 DiscountID);
        public List<ProductDiscount> ListByVendor(long vendorID);
        public List<ProductDiscountList> ListByProduct(long productID);

    }
}
