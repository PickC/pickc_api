using appify.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appify.Business.Contract
{
    public interface IOrderBusiness
    {
        public OrderHeader Save(Order item);
        public bool Delete(long orderID);

        public Order Get(long orderID); 
        public List<CustomerOrder> List(long sellerID);
        public List<OrderList> OrderList(long userID, short userType);
        public  CustomerOrder GetCustomerOrder(long orderID);
        public CustomerOrderNew GetCustomerOrderNew(long orderID);
        public List<CustomerOrderSummary> CustomerSummaryList(long sellerID, string OrderStatus, short PageNo, short Rows);
        public bool UpdateOrderStatus(Int64 orderID, short orderStatus, string remarks);

        public OrderUpdateDetail GetOrderUpdateDetail(long orderID);
        public string GetOrderStatus(long orderID);
        public bool SaveItem(OrderItem item);
        public bool DeleteItem(long orderID);

        public OrderDetail GetItem(long orderID);
        public List<OrderDetail> ListItems(long sellerID);

        public List<VendorOrder> ListByVendor(long vendorID, string OrderStatus, short PageNo, short Rows);
        public List<VendorOrderNew> ListByVendorNew(long vendorID, string OrderStatus, short PageNo, short Rows);
        public List<VendorOrder> GetByVendorDetail(long vendorID, long OrderID);
        public bool UpdateOrderPickup(Int64 orderID, decimal weight, decimal length, decimal width, decimal height);

        public OrderHeaderDelivery GetOrderForDelivery(Int64 orderID);

        public bool UpdateOrderAWB(Int64 orderID,string courierRefID, string shipmentID, string awb);

        public OrderTrackingDetails GetOrderTrackingDetails(Int64 orderID);
        public Int64 UpdateOrderTrackingStatus(OrderTrackingUpdate item);
        public Int64 UpdateDelhiveryOrderTrackingStatus(OrderTrackingUpdateDelhivery item);
        public bool OrderPaymentSave(OrderPayment item);
        public List<DailyOrderSummary> GetDailyOrderSummary();
        public List<EmailConfig> GetAlertHeader();
        public bool StockUpdate(long orderID, short OrderStatus);
    }
}
