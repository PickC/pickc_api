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
    public partial class MemberThemeRepository : IMemberThemeRepository
    {
        private IConfiguration configuration;
        private string appify_connectionstring;

        public MemberThemeRepository(IConfiguration config)
        {
            this.configuration = config;
            this.appify_connectionstring = config["ConnectionStrings:appify.connectionstring"].ToString();
        }
        public bool Delete(long memberID, long templateID, long themeID)
        {

            var result = false;
            //DataTable dt = DataTableHelper.CreateDataTableFromObj(item);
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(dbroutine.DBStoredProc.DELETEMEMBERTHEME))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@MemberID", memberID);
                        cmd.Parameters.AddWithValue("@TemplateID", templateID);
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

        public MemberTheme Get(long memberID)
        {
            MemberTheme item = new MemberTheme();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, dbroutine.DBStoredProc.SELECTMEMBERTHEME, memberID);
                item = DataTableHelper.ConvertDataTable<MemberTheme>(ds.Tables[0]).FirstOrDefault();
            }
            return item;
        }

        public List<MemberTheme> ListAll()
        {
            List<MemberTheme> item = new List<MemberTheme>();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, dbroutine.DBStoredProc.LISTMEMBERTHEME);
                item = DataTableHelper.ConvertDataTable<MemberTheme>(ds.Tables[0]);
            }
            return item;
        }

        public MemberThemeHeader Save(MemberThemeHeader item)
        {
            var result = false;
            //DataTable dt = DataTableHelper.CreateDataTableFromObj(item);
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(dbroutine.DBStoredProc.SAVEMEMBERTHEME))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;

                        cmd.Parameters.AddWithValue("@MemberID", item.MemberID);
                        cmd.Parameters.AddWithValue("@TemplateID", item.TemplateID);
                        cmd.Parameters.AddWithValue("@ThemeID", item.ThemeID);
                        cmd.Parameters.AddWithValue("@CreatedBy", item.CreatedBy);
                        cmd.Parameters.AddWithValue("@ModifiedBy", item.ModifiedBy);
                        cmd.Parameters.AddWithValue("@IsActive", item.IsActive);

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

        public List<TemplateThemesMember> ListAllThemesByTemplate(long templateID)
        {
            List<TemplateThemesMember> item = new List<TemplateThemesMember>();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, dbroutine.DBStoredProc.LISTTHEMEBYTEMPLATE, templateID);
                item = DataTableHelper.ConvertDataTable<TemplateThemesMember>(ds.Tables[0]);
            }
            return item;
        }
    }
}
