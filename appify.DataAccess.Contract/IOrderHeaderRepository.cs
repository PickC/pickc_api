using appify.models;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appify.DataAccess.Contract
{
    public interface IOrderHeaderRepository
    {

        public OrderHeader Save(OrderHeader item);
        public bool Delete(short orderID);

        public OrderHeader Get(short orderID);
        public List<CustomerOrder> List(long sellerID);

        public  CustomerOrder GetCustomerOrder(long orderID);
        public List<CustomerOrderSummary> CustomerSummaryList(long sellerID, string OrderStatus, short PageNo, short Rows);

        public bool UpdateOrderStatus(Int64 orderID, short orderStatus, string remarks);

        public List<VendorOrder> ListByVendor(long vendorID, string OrderStatus, short PageNo, short Rows);
        public List<VendorOrderNew> ListByVendorNew(long vendorID, string OrderStatus, short PageNo, short Rows);
        public List<VendorOrder> GetByVendorDetail(long vendorID, long OrderID);
        public OrderUpdateDetail GetOrderUpdateDetail(long orderID);
        public bool UpdateOrderPickup(Int64 orderID, decimal weight, decimal length, decimal width, decimal height);
        public OrderHeaderDelivery GetOrderForDelivery(Int64 orderID);

        public bool UpdateOrderAWB(Int64 orderID, string CourierRefID, string shipmentID, string awb);
        public OrderTrackingDetails GetOrderTrackingDetails(Int64 orderID);
        public Int64 UpdateOrderTrackingStatus(OrderTrackingUpdate item);
        public Int64 UpdateDelhiveryOrderTrackingStatus(OrderTrackingUpdateDelhivery item);
        public bool OrderPaymentSave(OrderPayment item);

        public List<DailyOrderSummary> GetDailyOrderSummary();
    }
}
