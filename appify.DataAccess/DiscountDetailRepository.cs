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
    public partial class DiscountDetailRepository : IDiscountDetailRepository
    {
        private IConfiguration  _configuration;
        private string appify_connectionstring;
        public DiscountDetailRepository(IConfiguration configuration) { 
            this._configuration = configuration;
            this.appify_connectionstring = configuration["ConnectionStrings:appify.connectionstring"].ToString();
        }

        public DiscountDetail Get(long DiscountID, long ProductID)
        {
            DiscountDetail item = new DiscountDetail();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.SELECTDISCOUNTDETAIL, DiscountID, ProductID);
            item = DataTableHelper.ConvertDataTable<DiscountDetail>(ds.Tables[0]).FirstOrDefault();

            return item;
        }

        public List<DiscountDetail> GetAll(long DiscountID, long ProductID)
        {
            List<DiscountDetail> item = new List<DiscountDetail>();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.LISTDISCOUNTDETAIL, DiscountID, ProductID);
            item = DataTableHelper.ConvertDataTable<DiscountDetail>(ds.Tables[0]);
            return item;
        }

        public bool Remove(long DiscountID, long ProductID)
        {
            var result = false;
            try
            {
                using(SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using(SqlCommand cmd = new SqlCommand(dbroutine.DBStoredProc.DELETEDISCOUNTDETAIL))
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

        public DiscountDetail Save(DiscountDetail item)
        {
            var result = false;

            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(dbroutine.DBStoredProc.SAVEDISCOUNTDETAIL))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;

                        cmd.Parameters.AddWithValue("@DiscountID", item.DiscountID);
                        cmd.Parameters.AddWithValue("@ProductID", item.ProductID);
                        cmd.Parameters.AddWithValue("@IsActive", item.IsActive);

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
            catch (Exception ex)
            {
                throw ex;
            }
            return item;
        }
    }
}
