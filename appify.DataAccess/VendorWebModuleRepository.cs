using appify.DataAccess.Contract;
using appify.models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using appify.utility;

namespace appify.DataAccess
{
    public class VendorWebModuleRepository : IVendorWebModuleRepository
    {
        private IConfiguration configuration;
        private string appify_connectionstring;

        public const string SELECTVENDORUSER = "[Operation].[usp_MemberUserSelect]";
        public const string LISTVENDORUSER = "[Operation].[usp_MemberUserListByVendor]";
        public const string SAVEVENDORUSER = "[Operation].[usp_MemberUserSave]";
        public const string UPDATEVEVENDORUSER = "[Operation].[usp_MemberUserStatusUpdate]";
        public VendorWebModuleRepository(IConfiguration config)
        {
            this.configuration = config;
            this.appify_connectionstring = config["ConnectionStrings:appify.connectionstring"].ToString();
        }

        public MemberUser SaveVendorUser(MemberUser item)
        {
            var result = false;
            //DataTable dt = DataTableHelper.CreateDataTableFromObj(item);
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(SAVEVENDORUSER))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;

                        cmd.Parameters.AddWithValue("@UserID", item.UserID);
                        cmd.Parameters.AddWithValue("@VendorID", item.@VendorID);
                        cmd.Parameters.AddWithValue("@MemberType", item.@MemberType);
                        cmd.Parameters.AddWithValue("@FirstName", item.@FirstName);
                        cmd.Parameters.AddWithValue("@LastName", item.@LastName);
                        cmd.Parameters.AddWithValue("@MobileNo", item.@MobileNo);
                        cmd.Parameters.AddWithValue("@Createdby", item.@Createdby);
                        cmd.Parameters.AddWithValue("@CreatedOn", item.@CreatedOn);
                        cmd.Parameters.AddWithValue("@ModifiedBy", item.@ModifiedBy);
                        cmd.Parameters.AddWithValue("@ModifiedOn", item.@ModifiedOn);
                        cmd.Parameters.AddWithValue("@IsActive", item.IsActive);

                        SqlParameter outPutParameter = new SqlParameter();
                        outPutParameter.ParameterName = "@NewUserID";
                        outPutParameter.SqlDbType = System.Data.SqlDbType.BigInt;
                        outPutParameter.Direction = System.Data.ParameterDirection.Output;
                        cmd.Parameters.Add(outPutParameter);


                        con.Open();
                        result = Convert.ToBoolean(cmd.ExecuteNonQuery());


                        item.UserID = Convert.ToInt64(outPutParameter.Value);
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

        public List<MemberUserLite> GetVendorUserList(long VendorID)
        {
            List<MemberUserLite> items = new List<MemberUserLite>();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, LISTVENDORUSER, VendorID);
                items = DataTableHelper.ConvertDataTable<MemberUserLite>(ds.Tables[0]);
            }
            return items;
        }

        public MemberUserLite GetVendorUser(long UserID)
        {
            MemberUserLite item = new MemberUserLite();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, SELECTVENDORUSER, UserID);
                item = DataTableHelper.ConvertDataTable<MemberUserLite>(ds.Tables[0]).FirstOrDefault();
            }
            return item;
        }

        public bool UpdateVendorUser(long UserID, bool IsActive)
        {
            var result = false;
            //DataTable dt = DataTableHelper.CreateDataTableFromObj(item);
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(UPDATEVEVENDORUSER))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@UserID", UserID);
                        cmd.Parameters.AddWithValue("@IsActive", IsActive);


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
