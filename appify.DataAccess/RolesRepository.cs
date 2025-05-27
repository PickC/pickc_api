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

        public bool Delete(short roleID, short userID)
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
                        cmd.Parameters.AddWithValue("@RoleID", roleID);
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

        public Roles Get(short roleID)
        {
            Roles item = new Roles();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, dbroutine.DBStoredProc.SELECTROLE, roleID);
                item = DataTableHelper.ConvertDataTable<Roles>(ds.Tables[0]).FirstOrDefault();
            }
            return item;
        }

        public long GetRolesCount()
        {

            Int64 item = new Int64();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, dbroutine.DBStoredProc.ROWCOUNTROLE);
                item = Convert.ToInt64(ds.Tables[0].Rows[0][0].ToString());
            }
            return item;
        }

        public List<Roles> ListAll(string roleCode,string roleDescription)
        {
            List<Roles> item = new List<Roles>();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, dbroutine.DBStoredProc.LISTROLE, roleCode, roleDescription);
                item = DataTableHelper.ConvertDataTable<Roles>(ds.Tables[0]);
            }
            return item;
        }

        public List<Roles> ListbyPageView(int pageNo, int rows)
        {
            List<Roles> item = new List<Roles>();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, dbroutine.DBStoredProc.PAGEVIEWLISTROLE, pageNo, rows);
                item = DataTableHelper.ConvertDataTable<Roles>(ds.Tables[0]);
            }
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

                        cmd.Parameters.AddWithValue("@RoleID", item.RoleID);
                        cmd.Parameters.AddWithValue("@RoleCode", item.RoleCode);
                        cmd.Parameters.AddWithValue("@RoleDescription", item.RoleDescription);
                        cmd.Parameters.AddWithValue("@CreatedBy", item.CreatedBy);
                        cmd.Parameters.AddWithValue("@ModifiedBy", item.ModifiedBy);

                        //Add the output parameter to the command object
                        SqlParameter outPutParameter = new SqlParameter();
                        outPutParameter.ParameterName = "@NewRoleID";
                        outPutParameter.SqlDbType = System.Data.SqlDbType.SmallInt;
                        outPutParameter.Direction = System.Data.ParameterDirection.Output;
                        cmd.Parameters.Add(outPutParameter);

                        con.Open();
                        result = Convert.ToBoolean(cmd.ExecuteNonQuery());

                        if (outPutParameter.Value != null && outPutParameter.Value != "" && outPutParameter.Value != System.DBNull.Value)
                            item.RoleID = Convert.ToInt16(outPutParameter.Value);
                        else
                            item.RoleID = 0;

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
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, dbroutine.DBStoredProc.ROLEACCESSTYPE, LookupCategory);
                item = DataTableHelper.ConvertDataTable<RolesAccessType>(ds.Tables[0]);
            }
            return item;
        }
    }
}
