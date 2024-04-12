using appify.DataAccess.Contract;
using appify.models;
using appify.utility;
using Microsoft.Data.SqlClient;
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

        public List<CustomerOrder> List(long sellerID)
        {
            List<CustomerOrder> items = new List<CustomerOrder>();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.LISTORDERHEADERBYSELLER, sellerID);
            items = DataTableHelper.ConvertDataTable<CustomerOrder>(ds.Tables[0]);

            return items;
        }

        public List<CustomerOrderSummary> CustomerSummaryList(long sellerID)
        {
            List<CustomerOrderSummary> items = new List<CustomerOrderSummary>();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.LISTORDERSUMMARYBYSELLER, sellerID);
            items = DataTableHelper.ConvertDataTable<CustomerOrderSummary>(ds.Tables[0]);

            return items;
        }
        

        public List<VendorOrder> ListByVendor(long vendorID)
        {
            List<VendorOrder> items = new List<VendorOrder>();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.LISTORDERBYVENDOR, vendorID);
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

        
    }
}
