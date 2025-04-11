using appify.DataAccess.Contract;
using appify.models;
using appify.utility;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace appify.DataAccess
{
    public class RoleRightsRepository : IRoleRightsRepository
    {
        private IConfiguration _configuration;
        private string appify_connectionstring;

        public RoleRightsRepository(IConfiguration configuration)
        {
            this._configuration = configuration;
            this.appify_connectionstring = configuration["ConnectionStrings:appify.connectionstring"].ToString();
        }

        public bool Delete(short roleID, short securableID)
        {
            var result = false;
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(dbroutine.DBStoredProc.DELETEROLERIGHT))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@RoleID", roleID);
                        cmd.Parameters.AddWithValue("@SecurableID", securableID);

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

        public RoleRights Get(short roleID, short securableID)
        {
            RoleRights item = new RoleRights();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, dbroutine.DBStoredProc.SELECTROLERIGHT, roleID, securableID);
                item = DataTableHelper.ConvertDataTable<RoleRights>(ds.Tables[0]).FirstOrDefault();
            }
            return item;
        }
         
        public List<RoleRights> ListAll(short roleID)
        {
            List<RoleRights> item = new List<RoleRights>();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, dbroutine.DBStoredProc.LISTROLERIGHT, roleID);
                item = DataTableHelper.ConvertDataTable<RoleRights>(ds.Tables[0]);
            }
            return item;
        }

        public bool Save(RoleRights item)
        {
            var result = false;

            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(dbroutine.DBStoredProc.SAVEROLERIGHT))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;

                        cmd.Parameters.AddWithValue("@RoleID", item.RoleID);
                        cmd.Parameters.AddWithValue("@SecurableID", item.SecurableID);
                        cmd.Parameters.AddWithValue("@IsAdd", item.IsAdd);
                        cmd.Parameters.AddWithValue("@IsEdit", item.IsEdit);
                        cmd.Parameters.AddWithValue("@IsView", item.IsView);
                        cmd.Parameters.AddWithValue("@IsDownload", item.IsDownload);
                        cmd.Parameters.AddWithValue("@IsDelete", item.IsDelete);
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

            return result;
        } 
    }
}
