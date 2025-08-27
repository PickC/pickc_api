using appify.Business.Contract;
using appify.DataAccess.Contract;
using appify.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace appify.Business
{
    public partial class OrderBusiness:IOrderBusiness
    {
        private IOrderHeaderRepository orderRepository;
        private IOrderDetailRepository orderDetailRepository;

        public OrderBusiness(IOrderHeaderRepository orderRepository, IOrderDetailRepository orderDetailRepository)
        {
            this.orderRepository = orderRepository;
            this.orderDetailRepository = orderDetailRepository; 
        }

        public bool Delete(long orderID)
        {
            throw new NotImplementedException();
        }

        public bool UpdateOrderStatus(Int64 orderID, short orderStatus, string remarks) { 
            return orderRepository.UpdateOrderStatus(orderID, orderStatus, remarks);
        }

        public bool DeleteItem(long orderID)
        {
            throw new NotImplementedException();
        }
        public Order Get(long orderID)
        {
            throw new NotImplementedException();
        }

        public CustomerOrder GetCustomerOrder(long orderID)
        {
            CustomerOrder orderItem = orderRepository.GetCustomerOrder(orderID);

            List<OrderDetail> items = new List<OrderDetail>();


            if (orderItem != null)
            {
                     items = new List<OrderDetail>();

                orderItem.items = orderDetailRepository.List(orderID);
                 

            }

            return orderItem;
            
        }
        public CustomerOrderNew GetCustomerOrderNew(long orderID)
        {
            CustomerOrderNew orderItem = orderRepository.GetCustomerOrderNew(orderID);
            List<OrderDetailNew> item = new List<OrderDetailNew>();
            if (orderItem != null)
            {
                item = new List<OrderDetailNew>();
                orderItem.items = orderDetailRepository.ListNew(orderID);
            }

            return orderItem;
        }

        public OrderDetail GetItem(long orderID)
        {
            throw new NotImplementedException();
        }

        public OrderUpdateDetail GetOrderUpdateDetail(long orderID)
        {
            return orderRepository.GetOrderUpdateDetail(orderID);
        }

        public string GetOrderStatus(long orderID)
        {
            return orderRepository.GetOrderStatus(orderID);
        }

        public List<CustomerOrder> List(long sellerID)
        {
            List<CustomerOrder> orders = orderRepository.List(sellerID);

            List<OrderDetail> items = new List<OrderDetail>();


            if (orders?.Any() == true)
            {
                foreach (var vo in orders)
                {
                    items = new List<OrderDetail>();

                    vo.items = orderDetailRepository.List(vo.OrderID);
                }


            }

            return orders;
        }
        public List<OrderList> OrderList(long userID, short userType)
        {
            return orderRepository.OrderList(userID, userType);
        }
        public Task<List<OrderList>> OrderListPageView(OrderSearch itemData)
        {
            return orderRepository.OrderListPageView(itemData);
        }
        public List<CustomerOrderSummary> CustomerSummaryList(long sellerID, string OrderStatus, short PageNo, short Rows)
        {
            List<CustomerOrderSummary> orders = orderRepository.CustomerSummaryList(sellerID, OrderStatus, PageNo, Rows);
            return orders;
        }


        public List<VendorOrder> ListByVendor(long vendorID, string OrderStatus, short PageNo, short Rows) { 
        
            List<VendorOrder> vendorOrders = new List<VendorOrder>();

            List<OrderDetail> orderItems = new List<OrderDetail>();


            vendorOrders = orderRepository.ListByVendor(vendorID, OrderStatus, PageNo, Rows);

            if (vendorOrders?.Any()==true)
            {
                foreach (var vo in vendorOrders)
                {
                    orderItems = new List<OrderDetail>();
                    
                    vo.items = orderDetailRepository.List(vo.OrderID);
                }

                
            }

            return vendorOrders;
        }

        public List<VendorOrderNew> ListByVendorNew(long vendorID, string OrderStatus, short PageNo, short Rows)
        {

            List<VendorOrderNew> vendorOrders = new List<VendorOrderNew>();

            List<OrderDetailNew> orderItems = new List<OrderDetailNew>();


            vendorOrders = orderRepository.ListByVendorNew(vendorID, OrderStatus, PageNo, Rows);

            if (vendorOrders?.Any() == true)
            {
                foreach (var vo in vendorOrders)
                {
                    orderItems = new List<OrderDetailNew>();

                    vo.items = orderDetailRepository.ListNew(vo.OrderID);
                }


            }

            return vendorOrders;
        }

        public List<VendorOrder> GetByVendorDetail(long vendorID, long OrderID)
        {

            List<VendorOrder> vendorOrders = new List<VendorOrder>();

            List<OrderDetail> orderItems = new List<OrderDetail>();


            vendorOrders = orderRepository.GetByVendorDetail(vendorID, OrderID);

            if (vendorOrders?.Any() == true)
            {
                foreach (var vo in vendorOrders)
                {
                    orderItems = new List<OrderDetail>();

                    vo.items = orderDetailRepository.List(vo.OrderID);
                }


            }

            return vendorOrders;
        }

        public List<OrderDetail> ListItems(long sellerID)
        {
            throw new NotImplementedException();
        }

        public OrderHeader Save(Order item)
        {
            OrderHeader orderHeader = new OrderHeader
            {
             OrderID = item.OrderID,
             OrderNo = "", //string.Format("PO{0}{1}",item.VendorID.ToString(),DateTime.Now.ToString("ssyyyyyMMddHHmm")),
             OrderDate = item.OrderDate,
             VendorID = item.VendorID,
             MemberID = item.MemberID,
             AddressID = item.AddressID,
             OrderAmount = item.OrderAmount,
             DiscountAmount= item.DiscountAmount,
             TaxAmount= item.TaxAmount,
             TotalAmount=item.TotalAmount,
             Currency = item.Currency,
             PaidAmount = item.PaidAmount,
             Remarks=item.Remarks,
             DeliveryInstruction = item.DeliveryInstruction,
             PaymentType = item.PaymentType,
             DeliveryCost = item.DeliveryCost,
             ReceiverName = item.ReceiverName,
             ReceiverMobileNo = item.ReceiverMobileNo,
             DeliveryChannel = item.DeliveryChannel,
             DeliveryChannelDescription = item.DeliveryChannelDescription

            };


             var orderItem = orderRepository.Save(orderHeader);

            if (item.items?.Any()==true)
            {
                foreach (var dt in item.items)
                {
                    dt.OrderID = orderItem.OrderID;
                    orderDetailRepository.Save(dt);
                }
            }

            // Create Invoice Data

            Invoice newInvoice = new Invoice();

            newInvoice.OrderID = orderItem.OrderID;
            newInvoice.MemberID = orderItem.MemberID;
            newInvoice.SellerID = orderItem.VendorID;
            newInvoice.InvoiceAmount = orderItem.OrderAmount;
            newInvoice.TaxAmount = orderItem.TaxAmount;
            newInvoice.TotalAmount = orderItem.TotalAmount;





            return orderItem;
        }

        public bool SaveItem(OrderItem item)
        {
            throw new NotImplementedException();
        }


        public bool UpdateOrderPickup(Int64 orderID, decimal weight, decimal length, decimal width, decimal height) {
            return orderRepository.UpdateOrderPickup(orderID, weight, length, width, height);
        
        }

        public OrderHeaderDelivery GetOrderForDelivery(Int64 orderID) { 
        
            var orderHeader = new OrderHeaderDelivery();

            orderHeader = orderRepository.GetOrderForDelivery(orderID);

            if (orderHeader !=null)
            {
                orderHeader.order_items = orderDetailRepository.GetOrderItemForDelivery(orderID);
            }

            return orderHeader;

        }


        public bool UpdateOrderAWB(Int64 orderID,string courierRefID, string shipmentID, string awb) { 
        
            return orderRepository.UpdateOrderAWB(orderID, courierRefID,shipmentID, awb);
        }

        public OrderTrackingDetails GetOrderTrackingDetails(Int64 orderID) {
            return orderRepository.GetOrderTrackingDetails(orderID);
        }
        public Int64 UpdateOrderTrackingStatus(OrderTrackingUpdate item)
        {
            return orderRepository.UpdateOrderTrackingStatus(item);
        }
        public Int64 UpdateDelhiveryOrderTrackingStatus(OrderTrackingUpdateDelhivery item)
        {
            return orderRepository.UpdateDelhiveryOrderTrackingStatus(item);
        }
        public bool OrderPaymentSave(OrderPayment item)
        {
            return orderRepository.OrderPaymentSave(item);
        }
        public List<DailyOrderSummary> GetDailyOrderSummary()
        {
            return orderRepository.GetDailyOrderSummary();
        }
        public List<EmailConfig> GetAlertHeader()
        {
            return orderRepository.GetAlertHeader();
        }
        public bool StockUpdate(long orderID, short OrderStatus)
        {
            return orderRepository.StockUpdate(orderID, OrderStatus);
        }

        public OrderData? GetOrderDataForAuditLog(long orderID) => orderRepository.GetOrderDataForAuditLog(orderID);
        public VendorEnabledServices GetVendorServices(long orderID)
        {
            return orderRepository.GetVendorServices(orderID);
        }
    }
}
