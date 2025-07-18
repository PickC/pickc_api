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

        public TemplateMaster SaveTemplate(TemplateMaster item)
        {
            var result = false;
            //DataTable dt = DataTableHelper.CreateDataTableFromObj(item);
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(dbroutine.DBStoredProc.SAVETEMPLATE))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;

                        cmd.Parameters.AddWithValue("@TemplateID", item.TemplateID);
                        cmd.Parameters.AddWithValue("@Name", item.Name);
                        cmd.Parameters.AddWithValue("@Description", item.Description);
                        cmd.Parameters.AddWithValue("@Banner", item.Banner);
                        cmd.Parameters.AddWithValue("@Code", item.Code);
                        cmd.Parameters.AddWithValue("@IsActive", item.IsActive);

                        //Add the output parameter to the command object
                        SqlParameter outPutParameter = new SqlParameter();
                        outPutParameter.ParameterName = "@NewTemplateID";
                        outPutParameter.SqlDbType = System.Data.SqlDbType.BigInt;
                        outPutParameter.Direction = System.Data.ParameterDirection.Output;
                        cmd.Parameters.Add(outPutParameter);

                        con.Open();
                        result = Convert.ToBoolean(cmd.ExecuteNonQuery());

                        item.TemplateID = Convert.ToInt64(outPutParameter.Value);




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
        public bool DeleteTemplate(long templateID)
        {
            var result = false;
            //DataTable dt = DataTableHelper.CreateDataTableFromObj(item);
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(dbroutine.DBStoredProc.DELETETEMPLATE))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@TemplateID", templateID);


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
        public TemplateMaster GetTemplate(long templateID)
        {
            TemplateMaster item = new TemplateMaster();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, dbroutine.DBStoredProc.SELECTTEMPLATE, templateID);
                item = DataTableHelper.ConvertDataTable<TemplateMaster>(ds.Tables[0]).FirstOrDefault();
            }
            return item;
        }
        public List<TemplateMaster> ListAllTemplate()
        {
            List<TemplateMaster> item = new List<TemplateMaster>();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, dbroutine.DBStoredProc.LISTTEMPLATE);
                item = DataTableHelper.ConvertDataTable<TemplateMaster>(ds.Tables[0]);
            }
            return item;
        }
        public List<TemplatesMaster> ViewAllTemplateList()
        {
            List<TemplatesMaster> item = new List<TemplatesMaster>();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, dbroutine.DBStoredProc.VIEWALLTEMPLATES);
                item = DataTableHelper.ConvertDataTable<TemplatesMaster>(ds.Tables[0]);
            }
            return item;
        }
        public TemplateThemePages GetTemplateByTheme(long templateID, long themeID)
        {
            TemplateThemePages item = new TemplateThemePages();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, dbroutine.DBStoredProc.GetTEMPLATEBYTHEME, templateID, themeID);
                item = DataTableHelper.ConvertDataTable<TemplateThemePages>(ds.Tables[0]).FirstOrDefault();
            }
            return item;
        }
        public bool DeleteTheme(long themeID)
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

        public ThemeMaster GetTheme(long themeID)
        {
            ThemeMaster item = new ThemeMaster();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, dbroutine.DBStoredProc.SELECTTHEME, themeID);
                item = DataTableHelper.ConvertDataTable<ThemeMaster>(ds.Tables[0]).FirstOrDefault();
            }
            return item;
        }

        public List<ThemeMaster> ListAllTheme()
        {
            List<ThemeMaster> item = new List<ThemeMaster>();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, dbroutine.DBStoredProc.LISTTHEME);
                item = DataTableHelper.ConvertDataTable<ThemeMaster>(ds.Tables[0]);
            }
            return item;
        }
        public List<TemplateThemes> ListAllThemesByTemplate(long templateID)
        {
            List<TemplateThemes> item = new List<TemplateThemes>();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, dbroutine.DBStoredProc.LISTTHEMEBYTEMPLATE, templateID);
                item = DataTableHelper.ConvertDataTable<TemplateThemes>(ds.Tables[0]);
            }
            return item;
        }
        public ThemeMaster SaveTheme(ThemeMaster item)
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
                        cmd.Parameters.AddWithValue("@ThemeName", item.ThemeName);
                        cmd.Parameters.AddWithValue("@ThemeColor", item.ThemeColor);
                        cmd.Parameters.AddWithValue("@TemplateID", item.TemplateID);
                        cmd.Parameters.AddWithValue("@ThemeJSON", item.ThemeJSON);
                        cmd.Parameters.AddWithValue("@ThemePagesJSON", item.ThemePagesJSON);
                        cmd.Parameters.AddWithValue("@IsDark", item.IsDark);
                        cmd.Parameters.AddWithValue("@IsThemeAvailable", item.IsThemeAvailable);
                        cmd.Parameters.AddWithValue("@IsActive", item.IsActive);


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
