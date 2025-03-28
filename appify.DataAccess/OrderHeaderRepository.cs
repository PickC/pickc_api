using appify.DataAccess.Contract;
using appify.models;
using appify.utility;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appify.DataAccess
{
    public partial class OrderHeaderRepository : IOrderHeaderRepository
    {
        private IConfiguration configuration;
        private string appify_connectionstring;
        public OrderHeaderRepository(IConfiguration config)
        {
            this.configuration = config;
            this.appify_connectionstring = config["ConnectionStrings:appify.connectionstring"].ToString();
        }

        public bool Delete(short orderID)
        {
            var result = false;
            //DataTable dt = DataTableHelper.CreateDataTableFromObj(item);
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(dbroutine.DBStoredProc.DELETEORDERHEADER))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@OrderID", orderID);


                        con.Open();
                        result = Convert.ToBoolean(cmd.ExecuteNonQuery());

                        con.Close();
                    }

                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return result;
        }


        public bool UpdateOrderStatus(Int64 orderID, short orderStatus, string remarks) {
            var result = false;
            //DataTable dt = DataTableHelper.CreateDataTableFromObj(item);
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(dbroutine.DBStoredProc.ORDERSTATUSUPDATE))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@OrderID", orderID);
                        cmd.Parameters.AddWithValue("@OrderStatus", orderStatus);
                        cmd.Parameters.AddWithValue("@Remarks", remarks);


                        con.Open();
                        result = Convert.ToBoolean(cmd.ExecuteNonQuery());

                        con.Close();
                    }

                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return result;

        }

        public OrderHeader Get(short orderID)
        {
            OrderHeader item = new OrderHeader();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.SELECTORDERHEADER, orderID);
            item = DataTableHelper.ConvertDataTable<OrderHeader>(ds.Tables[0]).FirstOrDefault();

            return item;
        }
        public OrderUpdateDetail GetOrderUpdateDetail(long orderID)
        {
            OrderUpdateDetail item = new OrderUpdateDetail();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.SELECTORDERUPDATEDETAIL, orderID);
            item = DataTableHelper.ConvertDataTable<OrderUpdateDetail>(ds.Tables[0]).FirstOrDefault();

            return item;
        }

        public OrderHeaderDelivery GetOrderForDelivery(Int64 orderID) {

            OrderHeaderDelivery item = new OrderHeaderDelivery();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.ORDERDELIVERYHEADER, orderID);
            item = DataTableHelper.ConvertDataTable<OrderHeaderDelivery>(ds.Tables[0]).FirstOrDefault();

            return item;

        }


        public OrderTrackingDetails GetOrderTrackingDetails(Int64 orderID) {

            OrderTrackingDetails item = new OrderTrackingDetails();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.ORDERDELIVERYTRACKINGDETAILS, orderID);
            item = DataTableHelper.ConvertDataTable<OrderTrackingDetails>(ds.Tables[0]).FirstOrDefault();

            return item;


        }

        public CustomerOrder GetCustomerOrder(long orderID)
        {
            CustomerOrder items = new CustomerOrder();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.SELECTORDERHEADERBYORDERID, orderID);
            items = DataTableHelper.ConvertDataTable<CustomerOrder>(ds.Tables[0]).FirstOrDefault();

            return items;
        }
        public CustomerOrderNew GetCustomerOrderNew(long orderID)
        {
            CustomerOrderNew items = new CustomerOrderNew();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.SELECTORDERSBYORDERID, orderID);
            items = DataTableHelper.ConvertDataTable<CustomerOrderNew>(ds.Tables[0]).FirstOrDefault();

            return items;
        }
        public List<CustomerOrder> List(long sellerID)
        {
            List<CustomerOrder> items = new List<CustomerOrder>();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.LISTORDERHEADERBYSELLER, sellerID);
            items = DataTableHelper.ConvertDataTable<CustomerOrder>(ds.Tables[0]);

            return items;
        }
        public List<OrderList> OrderList(long userID, short userType)
        {
            List<OrderList> items = new List<OrderList>();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.LISTORDER, userID, userType);
            items = DataTableHelper.ConvertDataTable<OrderList>(ds.Tables[0]);

            return items;
        }

        public List<DailyOrderSummary> GetDailyOrderSummary() {
            List<DailyOrderSummary> items = new List<DailyOrderSummary>();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.DAILYORDERSUMMARY);
            items = DataTableHelper.ConvertDataTable<DailyOrderSummary>(ds.Tables[0]);

            return items;


        }
        public List<CustomerOrderSummary> CustomerSummaryList(long sellerID, string OrderStatus, short PageNo, short Rows)
        {
            List<CustomerOrderSummary> items = new List<CustomerOrderSummary>();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.LISTORDERBYCUSTOMER, sellerID, OrderStatus, PageNo, Rows);
            items = DataTableHelper.ConvertDataTable<CustomerOrderSummary>(ds.Tables[0]);

            return items;
        }
        

        public List<VendorOrder> ListByVendor(long vendorID, string OrderStatus, short PageNo, short Rows)
        {
            List<VendorOrder> items = new List<VendorOrder>();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.LISTORDERBYVENDOR, vendorID, OrderStatus, PageNo, Rows);
            items = DataTableHelper.ConvertDataTable<VendorOrder>(ds.Tables[0]);

            return items;
        }

        public List<VendorOrderNew> ListByVendorNew(long vendorID, string OrderStatus, short PageNo, short Rows)
        {
            List<VendorOrderNew> items = new List<VendorOrderNew>();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.LISTORDERBYVENDOR, vendorID, OrderStatus, PageNo, Rows);
            items = DataTableHelper.ConvertDataTable<VendorOrderNew>(ds.Tables[0]);

            return items;
        }

        public List<VendorOrder> GetByVendorDetail(long vendorID, long OrderID)
        {
            List<VendorOrder> items = new List<VendorOrder>();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.ORDERBYVENDORDETAIL, vendorID, OrderID);
            items = DataTableHelper.ConvertDataTable<VendorOrder>(ds.Tables[0]);

            return items;
        }

        public OrderHeader Save(OrderHeader item)
        {
            var result = false;
            //DataTable dt = DataTableHelper.CreateDataTableFromObj(item);
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(dbroutine.DBStoredProc.SAVEORDERHEADER))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;

                        cmd.Parameters.AddWithValue("@OrderID", item.OrderID);
                        cmd.Parameters.AddWithValue("@OrderNo", item.OrderNo);
                        cmd.Parameters.AddWithValue("@OrderDate", item.OrderDate);
                        cmd.Parameters.AddWithValue("@VendorID", item.VendorID);
                        cmd.Parameters.AddWithValue("@MemberID", item.MemberID);
                        cmd.Parameters.AddWithValue("@AddressID", item.AddressID);
                        cmd.Parameters.AddWithValue("@OrderAmount", item.OrderAmount);
                        cmd.Parameters.AddWithValue("@DiscountAmount", item.DiscountAmount);
                        cmd.Parameters.AddWithValue("@TaxAmount", item.TaxAmount);
                        cmd.Parameters.AddWithValue("@TotalAmount", item.TotalAmount);
                        cmd.Parameters.AddWithValue("@Currency", item.Currency);
                        cmd.Parameters.AddWithValue("@PaidAmount", item.PaidAmount);
                        cmd.Parameters.AddWithValue("@Remarks", item.Remarks);
                        cmd.Parameters.AddWithValue("@DeliveryInstruction", item.DeliveryInstruction);
                        cmd.Parameters.AddWithValue("@DeliveryCost", item.DeliveryCost);
                        cmd.Parameters.AddWithValue("@PaymentType", item.PaymentType);
                        cmd.Parameters.AddWithValue("@ReceiverName", item.ReceiverName);
                        cmd.Parameters.AddWithValue("@ReceiverMobileNo", item.ReceiverMobileNo);
                        cmd.Parameters.AddWithValue("@DeliveryChannel", item.DeliveryChannel);



                        //Add the output parameter to the command object
                        SqlParameter outPutParameter = new SqlParameter();
                        outPutParameter.ParameterName = "@NewOrderID";
                        outPutParameter.SqlDbType = System.Data.SqlDbType.BigInt;
                        outPutParameter.Direction = System.Data.ParameterDirection.Output;
                        cmd.Parameters.Add(outPutParameter);

                        con.Open();
                        result = Convert.ToBoolean(cmd.ExecuteNonQuery());

                        item.OrderID = Convert.ToInt64(outPutParameter.Value);

                        con.Close();
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return item;
        }


        public bool UpdateOrderPickup(Int64 orderID, decimal weight, decimal length, decimal width, decimal height) {
            var result = false;
            //DataTable dt = DataTableHelper.CreateDataTableFromObj(item);
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(dbroutine.DBStoredProc.UPDATEORDERPICKUP))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@OrderID", orderID);
                        cmd.Parameters.AddWithValue("@TotalWeight", weight);
                        cmd.Parameters.AddWithValue("@Length", length);
                        cmd.Parameters.AddWithValue("@Width", width);
                        cmd.Parameters.AddWithValue("@Height", height);

                        con.Open();
                        result = Convert.ToBoolean(cmd.ExecuteNonQuery());

                        con.Close();
                    }

                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return result;

        }



        public bool UpdateOrderAWB(Int64 orderID, string courierRefID, string shipmentID, string awb)
        {
            var result = false;
            //DataTable dt = DataTableHelper.CreateDataTableFromObj(item);
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(dbroutine.DBStoredProc.ORDERUPDATEAWB))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@OrderID", orderID);
                        cmd.Parameters.AddWithValue("@CourierRefID", courierRefID);
                        cmd.Parameters.AddWithValue("@ShipmentID", shipmentID);
                        cmd.Parameters.AddWithValue("@AWB", awb);

                        con.Open();
                        result = Convert.ToBoolean(cmd.ExecuteNonQuery());

                        con.Close();
                    }

                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return result;

        }

        public Int64 UpdateOrderTrackingStatus(OrderTrackingUpdate item)
        {
            var result = false;
            Int64 OrderID = 0;
            //DataTable dt = DataTableHelper.CreateDataTableFromObj(item);
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(dbroutine.DBStoredProc.UPDATEORDERTRACKINGSTATUS))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@OrderNo", item.OrderNo);
                        cmd.Parameters.AddWithValue("@OrderStatus", item.OrderStatus);
                        cmd.Parameters.AddWithValue("@Remarks", item.Remarks);
                        cmd.Parameters.AddWithValue("@CourierRefID", item.CourierRefID);
                        cmd.Parameters.AddWithValue("@ShipmentID", item.ShipmentID);
                        cmd.Parameters.AddWithValue("@AWB", item.AWB);
                        cmd.Parameters.AddWithValue("@DeliveredOn", item.DeliveredOn);
                        cmd.Parameters.AddWithValue("@CourierName", item.CourierName);
                        cmd.Parameters.AddWithValue("@TrackURL", item.TrackURL);

                        //Add the output parameter to the command object
                        SqlParameter outPutParameter = new SqlParameter();
                        outPutParameter.ParameterName = "@NewOrderID";
                        outPutParameter.SqlDbType = System.Data.SqlDbType.BigInt;
                        outPutParameter.Direction = System.Data.ParameterDirection.Output;
                        cmd.Parameters.Add(outPutParameter);
                        con.Open();
                        result = Convert.ToBoolean(cmd.ExecuteNonQuery());
                        if (outPutParameter.Value != null && outPutParameter.Value != "" && outPutParameter.Value != System.DBNull.Value)
                            OrderID = Convert.ToInt64(outPutParameter.Value);

                        con.Close();
                    }

                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return OrderID;

        }

        public Int64 UpdateDelhiveryOrderTrackingStatus(OrderTrackingUpdateDelhivery item)
        {
            var result = false;
            Int64 OrderID = 0;
            //DataTable dt = DataTableHelper.CreateDataTableFromObj(item);
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(dbroutine.DBStoredProc.UPDATEORDERTRACKINGSTATUSDELHIVERY))
                    { 
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@AWB", item.AWB);
                        cmd.Parameters.AddWithValue("@Status", item.Status);
                        cmd.Parameters.AddWithValue("@StatusType", item.StatusType);
                        cmd.Parameters.AddWithValue("@Instructions", item.Instructions);
                        cmd.Parameters.AddWithValue("@ReferenceNo", item.ReferenceNo);
                        cmd.Parameters.AddWithValue("@StatusDateTime", item.StatusDateTime);

                        //Add the output parameter to the command object
                        SqlParameter outPutParameter = new SqlParameter();
                        outPutParameter.ParameterName = "@NewOrderID";
                        outPutParameter.SqlDbType = System.Data.SqlDbType.BigInt;
                        outPutParameter.Direction = System.Data.ParameterDirection.Output;
                        cmd.Parameters.Add(outPutParameter);
                        con.Open();
                        result = Convert.ToBoolean(cmd.ExecuteNonQuery());
                        if (outPutParameter.Value != null && outPutParameter.Value != "" && outPutParameter.Value != System.DBNull.Value)
                            OrderID = Convert.ToInt64(outPutParameter.Value);

                        con.Close();
                    }

                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return OrderID;

        }

        public bool OrderPaymentSave(OrderPayment item)
        {
            var result = false;
            //DataTable dt = DataTableHelper.CreateDataTableFromObj(item);
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(dbroutine.DBStoredProc.SAVEORDERPAYMENT))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@PaymentID", item.PaymentID);
                        cmd.Parameters.AddWithValue("@PaymentDate", item.PaymentDate);
                        cmd.Parameters.AddWithValue("@OrderID", item.OrderID);
                        cmd.Parameters.AddWithValue("@EventName", item.EventName);
                        cmd.Parameters.AddWithValue("@PaymentAmount", item.PaymentAmount);
                        cmd.Parameters.AddWithValue("@OrderReferenceNo", item.OrderReferenceNo);
                        cmd.Parameters.AddWithValue("@PaymentReferenceNo", item.PaymentReferenceNo);
                        cmd.Parameters.AddWithValue("@PaymentMode", item.PaymentMode);
                        cmd.Parameters.AddWithValue("@LookupCode", item.LookupCode);

                        //Add the output parameter to the command object
                        SqlParameter outPutParameter = new SqlParameter();
                        outPutParameter.ParameterName = "@RETURNSTATUS";
                        outPutParameter.SqlDbType = System.Data.SqlDbType.BigInt;
                        outPutParameter.Direction = System.Data.ParameterDirection.Output;
                        cmd.Parameters.Add(outPutParameter);

                        con.Open();
                        result = Convert.ToBoolean(cmd.ExecuteNonQuery());
                        if (outPutParameter.Value != null && outPutParameter.Value != "" && outPutParameter.Value != System.DBNull.Value)
                            result = Convert.ToBoolean(outPutParameter.Value);
                        con.Close();

                    }

                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return result;

        }
        //public List<DailyOrderSummary> GetDailyOrderSummary()
        //{
        //    List<DailyOrderSummary> items = new List<DailyOrderSummary>();
        //    DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.DAILYORDERSUMMARY);
        //    items = DataTableHelper.ConvertDataTable<DailyOrderSummary>(ds.Tables[0]);

        //    return items;

        //}
        public List<EmailConfig> GetAlertHeader()
        {
            List<EmailConfig> items = new List<EmailConfig>();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.EMAILSERVERALERT);
            items = DataTableHelper.ConvertDataTable<EmailConfig>(ds.Tables[0]);

            return items;
        }
        public bool StockUpdate(long orderID, short OrderStatus)
        {
            var result = false;
            //DataTable dt = DataTableHelper.CreateDataTableFromObj(item);
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(dbroutine.DBStoredProc.STOCKUPDATE))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@OrderID", orderID);
                        cmd.Parameters.AddWithValue("@OrderStatus", OrderStatus);

                        con.Open();
                        result = Convert.ToBoolean(cmd.ExecuteNonQuery());

                        con.Close();
                    }

                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return result;
        }
    }
}
