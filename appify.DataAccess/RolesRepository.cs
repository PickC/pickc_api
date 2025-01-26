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
using Azure;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace appify.DataAccess
{
    public  class RolesRepository : IRolesRepository
    {
        private IConfiguration _configuration;
        private string appify_connectionstring;

        public RolesRepository(IConfiguration configuration)
        {
            this._configuration = configuration;
            this.appify_connectionstring = configuration["ConnectionStrings:appify.connectionstring"].ToString();
        }

        public bool Delete(string roleCode, short userID)
        {
            var result = false;
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(dbroutine.DBStoredProc.DELETEROLE))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@RoleCode", roleCode);
                        cmd.Parameters.AddWithValue("@ModifiedBy", userID);

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

        public Roles Get(string roleCode)
        {
            Roles item = new Roles();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.SELECTROLE, roleCode);
            item = DataTableHelper.ConvertDataTable<Roles>(ds.Tables[0]).FirstOrDefault();

            return item;
        }

        public long GetRolesCount()
        {

            Int64 item = new Int64();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.ROWCOUNTROLE);
            item = Convert.ToInt64(ds.Tables[0].Rows[0][0].ToString());

            return item;
        }

        public List<Roles> ListAll()
        {
            List<Roles> item = new List<Roles>();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.LISTROLE);
            item = DataTableHelper.ConvertDataTable<Roles>(ds.Tables[0]);

            return item;
        }

        public List<Roles> ListbyPageView(int pageNo, int rows)
        {
            List<Roles> item = new List<Roles>();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.PAGEVIEWLISTROLE, pageNo, rows);
            item = DataTableHelper.ConvertDataTable<Roles>(ds.Tables[0]);

            return item;
        }

        public Roles Save(Roles item)
        {
            var result = false;

            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(dbroutine.DBStoredProc.SAVEROLE))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;

                        cmd.Parameters.AddWithValue("@RoleCode", item.RoleCode);
                        cmd.Parameters.AddWithValue("@RoleDescription", item.RoleDescription);
                        cmd.Parameters.AddWithValue("@CreatedBy", item.CreatedBy);
                        cmd.Parameters.AddWithValue("@ModifiedBy", item.ModifiedBy);

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
        public List<RolesAccessType> GetAccessType(string LookupCategory)
        {
            List<RolesAccessType> item = new List<RolesAccessType>();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.ROLEACCESSTYPE, LookupCategory);
            item = DataTableHelper.ConvertDataTable<RolesAccessType>(ds.Tables[0]);

            return item;
        }
    }
}
