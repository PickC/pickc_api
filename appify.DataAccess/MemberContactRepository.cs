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
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace appify.DataAccess
{
    public partial class MemberContactRepository : IMemberContactRepository
    {

        private IConfiguration configuration;
        private string appify_connectionstring;

        public MemberContactRepository(IConfiguration config)
        {
            this.configuration = config;
            this.appify_connectionstring = config["ConnectionStrings:appify.connectionstring"].ToString();
        }
        public bool Delete(long memberID, string mobileNo)
        {
            var result = false;
            //DataTable dt = DataTableHelper.CreateDataTableFromObj(item);
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(dbroutine.DBStoredProc.DELETEMEMBERCONTACT))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@MemberID", memberID);
                        cmd.Parameters.AddWithValue("@MobileNo", mobileNo);



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

        public MemberContact Get(long memberID, string mobileNo)
        {
            MemberContact item = new MemberContact();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, dbroutine.DBStoredProc.SELECTMEMBERCONTACT, memberID, mobileNo);
                item = DataTableHelper.ConvertDataTable<MemberContact>(ds.Tables[0]).FirstOrDefault();
            }
            return item;
        }

        public List<MemberContact> List(long memberID)
        {
            List<MemberContact> item = new List<MemberContact>();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, dbroutine.DBStoredProc.LISTMEMBERCONTACT, memberID);
                item = DataTableHelper.ConvertDataTable<MemberContact>(ds.Tables[0]);
            }
            return item;
        }
        public bool BulkSave(List<MemberContact> items)
        {
            MemberContact returnItem = new MemberContact();
            var result = false;
            try
            {
                foreach (var item in items)
                {
                    returnItem = Save(item);
                    result = true;
                }
            }
            catch (Exception ex)
            {
                returnItem = new MemberContact();
                result = false;
            }

            return result;
        }

        public MemberContact Save(MemberContact item)
        {
            var result = false;
            //DataTable dt = DataTableHelper.CreateDataTableFromObj(item);
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(dbroutine.DBStoredProc.SAVEMEMBERCONTACT))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;


                        cmd.Parameters.AddWithValue("@MemberID", item.MemberID);
                        cmd.Parameters.AddWithValue("@MobileNo", item.MobileNo);
                        cmd.Parameters.AddWithValue("@ContactName", item.ContactName);
                        cmd.Parameters.AddWithValue("@EmailID", item.EmailID);



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

            return item;
        }
    }
}
