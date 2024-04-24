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

        public List<DiscountHeader> GetAll(long DiscountID)
        {
            List<DiscountHeader> item = new List<DiscountHeader>();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.LISTDISCOUNTHEADER, DiscountID);
            item = DataTableHelper.ConvertDataTable<DiscountHeader>(ds.Tables[0]);

            return item;
        }


        public List<ProductDiscount> ListByVendor(long vendorID)
        {
            List<ProductDiscount> items = new List<ProductDiscount>();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.LISTDISCOUNTBYVENDOR, vendorID);
            items = DataTableHelper.ConvertDataTable<ProductDiscount>(ds.Tables[0]);

            return items;
        }


        public List<ProductDiscount> ListByProduct(long productID)
        {
            List<ProductDiscount> items = new List<ProductDiscount>();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.LISTVENDORPRODUCTDISCOUNTS, productID);
            items = DataTableHelper.ConvertDataTable<ProductDiscount>(ds.Tables[0]);

            return items;
        }
        

        public bool Remove(long DiscountID, long ModifiedBy)
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
                        cmd.Parameters.AddWithValue("@ModifiedBy", ModifiedBy);

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
                        cmd.Parameters.AddWithValue("@VendorID", item.VendorID); 
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
    }
}
