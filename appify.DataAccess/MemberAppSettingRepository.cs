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
    public partial class MemberAppSettingRepository : IMemberAppSettingRepository
    {

        private IConfiguration configuration;
        private string appify_connectionstring;

        public MemberAppSettingRepository(IConfiguration config)
        {
            this.configuration = config;
            this.appify_connectionstring = config["ConnectionStrings:appify.connectionstring"].ToString();
        }
        public bool DeleteAppSetting(long userID,string appName)
        {
            var result = false;
            //DataTable dt = DataTableHelper.CreateDataTableFromObj(item);
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(dbroutine.DBStoredProc.DELETEMEMBERAPPSETTING))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@UserID", userID);


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

        public MemberAppSetting GetAppSetting(long userID, string appName)
        {
            MemberAppSetting item = new MemberAppSetting();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.SELECTMEMBERAPPSETTING, userID);
            item = DataTableHelper.ConvertDataTable<MemberAppSetting>(ds.Tables[0]).FirstOrDefault();

            return item;
        }

        public List<MemberAppSetting> GetAppSettingList(long userID)
        {
            List<MemberAppSetting> items = new List<MemberAppSetting>();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.LISTMEMBERAPPSETTING, userID);
            items = DataTableHelper.ConvertDataTable<MemberAppSetting>(ds.Tables[0]);

            return items;
        }

        public bool saveAppSetting(MemberAppSetting item)
        {
            var result = false;
            //DataTable dt = DataTableHelper.CreateDataTableFromObj(item);
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(dbroutine.DBStoredProc.SAVEMEMBERAPPSETTING))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@UserID", item.UserID);
                        cmd.Parameters.AddWithValue("@AppName", item.AppName);
                        cmd.Parameters.AddWithValue("@AppName1", item.AppName1);
                        cmd.Parameters.AddWithValue("@AppName2", item.AppName2);
                        cmd.Parameters.AddWithValue("@ShortDescription", item.ShortDescription);
                        cmd.Parameters.AddWithValue("@Description", item.Description);
                        cmd.Parameters.AddWithValue("@Logo", item.Logo);
                        cmd.Parameters.AddWithValue("@PlayStoreID", item.PlayStoreID);
                        cmd.Parameters.AddWithValue("@AppStoreID", item.AppStoreID);
                        cmd.Parameters.AddWithValue("@AppIcon", item.AppIcon);

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
