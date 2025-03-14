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
    public class ThemeMasterRepository : IThemeMasterRepository
    {

        private IConfiguration configuration;
        private string appify_connectionstring;

        public ThemeMasterRepository(IConfiguration config)
        {
            this.configuration = config;
            this.appify_connectionstring = config["ConnectionStrings:appify.connectionstring"].ToString();
        }
        public bool Delete(long themeID)
        {
            var result = false;
            //DataTable dt = DataTableHelper.CreateDataTableFromObj(item);
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(dbroutine.DBStoredProc.DELETETHEME))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@ThemeID", themeID);


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

        public ThemeMaster Get(long themeID)
        {
            ThemeMaster item = new ThemeMaster();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.SELECTTHEME, themeID);
            item = DataTableHelper.ConvertDataTable<ThemeMaster>(ds.Tables[0]).FirstOrDefault();

            return item;
        }

        public List<ThemeMaster> ListAll()
        {
            List<ThemeMaster> item = new List<ThemeMaster>();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.LISTTHEME);
            item = DataTableHelper.ConvertDataTable<ThemeMaster>(ds.Tables[0]);

            return item;
        }

        public ThemeMaster Save(ThemeMaster item)
        {
            var result = false;
            //DataTable dt = DataTableHelper.CreateDataTableFromObj(item);
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(dbroutine.DBStoredProc.SAVETHEME))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;

                        cmd.Parameters.AddWithValue("@ThemeID", item.ThemeID);
                        cmd.Parameters.AddWithValue("@PrimaryColor", item.PrimaryColor);
                        cmd.Parameters.AddWithValue("@PrimaryLightColor", item.PrimaryLightColor);
                        cmd.Parameters.AddWithValue("@BackgroundBoxColor", item.BackgroundBoxColor);
                        cmd.Parameters.AddWithValue("@TextColor", item.TextColor);
                        cmd.Parameters.AddWithValue("@SecondaryColor", item.SecondaryColor);
                        cmd.Parameters.AddWithValue("@ScaffoldBgColor", item.ScaffoldBgColor);
                        cmd.Parameters.AddWithValue("@IsDark", item.IsDark);


                        //Add the output parameter to the command object
                        SqlParameter outPutParameter = new SqlParameter();
                        outPutParameter.ParameterName = "@NewThemeID";
                        outPutParameter.SqlDbType = System.Data.SqlDbType.BigInt;
                        outPutParameter.Direction = System.Data.ParameterDirection.Output;
                        cmd.Parameters.Add(outPutParameter);

                        con.Open();
                        result = Convert.ToBoolean(cmd.ExecuteNonQuery());

                        item.ThemeID = Convert.ToInt64(outPutParameter.Value);


                         

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
