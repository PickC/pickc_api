/*
 * Company: AppifyRetail.
 * Author: Gurjeet
 * Version: 1.1
 * Date: 2024-09-01
 * Description:
*/
using appify.DataAccess.Contract;
using appify.models;
using appify.utility;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace appify.DataAccess
{
    public partial class OrderDetailRepository : IOrderDetailRepository
    {
        private IConfiguration configuration;
        private string appify_connectionstring;

        public OrderDetailRepository(IConfiguration config)
        {
            this.configuration = config;
            this.appify_connectionstring = config["ConnectionStrings:appify.connectionstring"].ToString();
        }

        public bool Delete(short itemID, Int64 orderID)
        {
            var result = false;
            //DataTable dt = DataTableHelper.CreateDataTableFromObj(item);
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(dbroutine.DBStoredProc.DELETEORDERDETAIL))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@ItemID", itemID);
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

        public OrderDetail Get(short itemID, Int64 orderID)
        {
            throw new NotImplementedException();
        }

        public List<OrderDetail> List(Int64 orderID)
        {
            List<OrderDetail> items = new List<OrderDetail>();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.LISTORDERDETAIL, orderID);
            items = DataTableHelper.ConvertDataTable<OrderDetail>(ds.Tables[0]);

            return items;
        }

        public List<OrderDetailNew> ListNew(Int64 orderID)
        {
            List<OrderDetailNew> items = new List<OrderDetailNew>();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.LISTORDERDETAIL, orderID);
            items = DataTableHelper.ConvertDataTable<OrderDetailNew>(ds.Tables[0]);

            return items;
        }

        public List<OrderDetailDelivery> GetOrderItemForDelivery(Int64 orderID) {

            List<OrderDetailDelivery> items = new List<OrderDetailDelivery>();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.ORDERDELIVERYDETAILS, orderID);
            items = DataTableHelper.ConvertDataTable<OrderDetailDelivery>(ds.Tables[0]);

            return items;

        }

        public bool Save(OrderItem item)
        {
            var result = false;
            //DataTable dt = DataTableHelper.CreateDataTableFromObj(item);
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(dbroutine.DBStoredProc.SAVEORDERDETAIL))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;

                        cmd.Parameters.AddWithValue("@ItemID", item.ItemID);
                        cmd.Parameters.AddWithValue("@OrderID", item.OrderID);
                        cmd.Parameters.AddWithValue("@ProductID", item.ProductID);
                        cmd.Parameters.AddWithValue("@SellerID", item.SellerID);
                        cmd.Parameters.AddWithValue("@Quantity", item.Quantity);
                        cmd.Parameters.AddWithValue("@UnitPrice", item.UnitPrice);
                        cmd.Parameters.AddWithValue("@DiscountType", item.DiscountType);
                        cmd.Parameters.AddWithValue("DiscountAmount", item.DiscountAmount);
                        cmd.Parameters.AddWithValue("@SellingPrice", item.SellingPrice);
                        cmd.Parameters.AddWithValue("@PriceID", item.PriceID);



                        ////Add the output parameter to the command object
                        //SqlParameter outPutParameter = new SqlParameter();
                        //outPutParameter.ParameterName = "@NewProductID";
                        //outPutParameter.SqlDbType = System.Data.SqlDbType.BigInt;
                        //outPutParameter.Direction = System.Data.ParameterDirection.Output;
                        //cmd.Parameters.Add(outPutParameter);

                        con.Open();
                        result = Convert.ToBoolean(cmd.ExecuteNonQuery());

                        //item.OrderID = Convert.ToInt64(outPutParameter.Value);

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
