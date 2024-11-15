using appify.DataAccess.Contract;
using appify.models;
using appify.utility;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appify.DataAccess
{
    public partial class DiscountHeaderRepository : IDiscountHeaderRepository
    {
        private IConfiguration _configuration;
        private string appify_connectionstring;
        public DiscountHeaderRepository(IConfiguration configuration) { 
        this._configuration = configuration;
            this.appify_connectionstring = configuration["ConnectionStrings:appify.connectionstring"].ToString();
        }

        public DiscountHeader Get(long DiscountID)
        {
            DiscountHeader item = new DiscountHeader();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.SELECTDISCOUNTHEADER, DiscountID);
            item = DataTableHelper.ConvertDataTable<DiscountHeader>(ds.Tables[0]).FirstOrDefault();

            return item;
        }
        public OrderDiscount GetDiscount(long DiscountID)
        {
            OrderDiscount item = new OrderDiscount();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.SELECTORDERDISCOUNT, DiscountID);
            item = DataTableHelper.ConvertDataTable<OrderDiscount>(ds.Tables[0]).FirstOrDefault();

            return item;
        }
        public List<OrderDiscountDetail> GetOrderDiscountByVendor(Int64 VendorID)
        {
            List<OrderDiscountDetail> item = new List<OrderDiscountDetail>();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.GETORDERDISCOUNTSBYVENDOR, VendorID);
            item = DataTableHelper.ConvertDataTable<OrderDiscountDetail>(ds.Tables[0]);

            return item;
        }
        public List<DiscountHeader> GetAll()
        {
            List<DiscountHeader> item = new List<DiscountHeader>();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.LISTDISCOUNTHEADER);
            item = DataTableHelper.ConvertDataTable<DiscountHeader>(ds.Tables[0]);

            return item;
        }
        public List<OrderDiscount> GetDiscountByVendor(Int64 VendorID)
        {
            List<OrderDiscount> item = new List<OrderDiscount>();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.LISTORDERDISCOUNTBYVENDOR, VendorID);
            item = DataTableHelper.ConvertDataTable<OrderDiscount>(ds.Tables[0]);

            return item;
        }
        public List<OrderDiscount> GetDiscountListbyVendorRows(long VendorID, int pageNo, int rows)
        {
            List<OrderDiscount> item = new List<OrderDiscount>();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.LISTORDERDISCOUNTBYVENDORPAGEVIEW, VendorID, pageNo, rows);
            item = DataTableHelper.ConvertDataTable<OrderDiscount>(ds.Tables[0]);

            return item;
        }
        public Int64 GetDiscountCount(Int64 VendorID)
        {
            Int64 item = new Int64();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.ROWCOUNTORDERDISCOUNT, VendorID);
            item = Convert.ToInt64(ds.Tables[0].Rows[0][0].ToString());

            return item;
        }
        public List<ProductDiscount> ListByVendor(long vendorID)
        {
            List<ProductDiscount> items = new List<ProductDiscount>();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.LISTDISCOUNTBYVENDOR, vendorID);
            items = DataTableHelper.ConvertDataTable<ProductDiscount>(ds.Tables[0]);

            return items;
        }


        public List<ProductDiscountList> ListByProduct(long productID)
        {
            List<ProductDiscountList> items = new List<ProductDiscountList>();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.LISTVENDORPRODUCTDISCOUNTS, productID);
            items = DataTableHelper.ConvertDataTable<ProductDiscountList>(ds.Tables[0]);

            return items;
        }
        

        public bool Remove(long DiscountID, long ProductID)
        {
            var result = false;
            try
            {
                using(SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using(SqlCommand cmd = new SqlCommand(dbroutine.DBStoredProc.DELETEDISCOUNTHEADER))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@DiscountID", DiscountID);
                        cmd.Parameters.AddWithValue("@ProductID", ProductID);

                        con.Open();
                        result = Convert.ToBoolean(cmd.ExecuteNonQuery());

                        con.Close();
                    }
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public DiscountHeader Save(DiscountHeader item)
        {
            var result = false;

            try
            {
                using(SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using(SqlCommand cmd = new SqlCommand(dbroutine.DBStoredProc.SAVEDISCOUNTHEADER))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;

                        cmd.Parameters.AddWithValue("@DiscountID", item.DiscountID); 
                        cmd.Parameters.AddWithValue("@ProductID", item.ProductID); 
                        cmd.Parameters.AddWithValue("@DiscountType", item.DiscountType);
                        cmd.Parameters.AddWithValue("@DiscountValue", item.DiscountValue);
                        cmd.Parameters.AddWithValue("@EffectiveDate", item.EffectiveDate);
                        cmd.Parameters.AddWithValue("@ExpiryDate", item.ExpiryDate);
                        cmd.Parameters.AddWithValue("@CreatedBy", item.CreatedBy);
                        cmd.Parameters.AddWithValue("@ModifiedBy", item.ModifiedBy);

                        SqlParameter outPutParameter = new SqlParameter();
                        outPutParameter.ParameterName = "@NewDiscountID";
                        outPutParameter.SqlDbType = SqlDbType.BigInt;
                        outPutParameter.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(outPutParameter);

                        con.Open();
                        result = Convert.ToBoolean(cmd.ExecuteNonQuery());
                        item.IsActive = true;
                        item.DiscountID = Convert.ToInt64(outPutParameter.Value);
                        con.Close();
                    }
                }

            }
            catch(Exception ex)
            {
                throw ex;
            }

            return item;
        }

        public OrderDiscount DiscountSave(OrderDiscount item)
        {
            var result = false;

            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(dbroutine.DBStoredProc.SAVEORDERDISCOUNT))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;

                        cmd.Parameters.AddWithValue("@DiscountID", item.DiscountID);
                        cmd.Parameters.AddWithValue("@VendorID", item.VendorID);
                        cmd.Parameters.AddWithValue("@UOM", item.UOM);
                        cmd.Parameters.AddWithValue("@Qty", item.Qty);
                        cmd.Parameters.AddWithValue("@EffectiveDate", item.EffectiveDate);
                        cmd.Parameters.AddWithValue("@ExpiryDate", item.ExpiryDate);
                        cmd.Parameters.AddWithValue("@DiscountType", item.DiscountType);
                        cmd.Parameters.AddWithValue("@DiscountAmount", item.DiscountAmount);
                        cmd.Parameters.AddWithValue("@CreatedBy", item.CreatedBy);
                        cmd.Parameters.AddWithValue("@ModifiedBy", item.ModifiedBy);

                        SqlParameter outPutParameter = new SqlParameter();
                        outPutParameter.ParameterName = "@NewDiscountID";
                        outPutParameter.SqlDbType = SqlDbType.BigInt;
                        outPutParameter.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(outPutParameter);

                        con.Open();
                        result = Convert.ToBoolean(cmd.ExecuteNonQuery());
                        item.IsActive = true;
                        item.DiscountID = Convert.ToInt64(outPutParameter.Value);
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
        public bool DiscountRemove(long DiscountID)
        {
            var result = false;
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(dbroutine.DBStoredProc.REMOVEORDERDISCOUNT))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@DiscountID", DiscountID);

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
