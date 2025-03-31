using appify.DataAccess.Contract;
using appify.models;
using appify.utility;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace appify.DataAccess
{
    public class SecurablesRepository : ISecurablesRepository
    {
        private IConfiguration _configuration;
        private string appify_connectionstring;




        public SecurablesRepository(IConfiguration configuration) {
            this._configuration = configuration;
            this.appify_connectionstring = configuration["ConnectionStrings:appify.connectionstring"].ToString();
        }

        public bool Delete(Int32 SecurablesID)
        {
            var result = false;
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(dbroutine.DBStoredProc.REMOVESECURABLE))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@SecurableID", SecurablesID);

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

        public Securables Get(Int32 SecurablesID)
        {
            Securables item = new Securables();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.SELECTSECURABLE, SecurablesID);
            item = DataTableHelper.ConvertDataTable<Securables>(ds.Tables[0]).FirstOrDefault();

            return item;
        }

        public Int64 GetSecurablesCount()
        {

            Int64 item = new Int64();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.ROWCOUNTROLE);
            item = Convert.ToInt64(ds.Tables[0].Rows[0][0].ToString());

            return item;
        }

        public List<Securables> ListAll()
        {
            List<Securables> item = new List<Securables>();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.LISTSECURABLE);
            item = DataTableHelper.ConvertDataTable<Securables>(ds.Tables[0]);

            return item;
        }

        public Securables Save(Securables item)
        {
            var result = false;

            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(dbroutine.DBStoredProc.SAVESECURABLE))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;

                        cmd.Parameters.AddWithValue("@SecurableID", item.SecurableID);
                        cmd.Parameters.AddWithValue("@PageName", item.PageName);
                        cmd.Parameters.AddWithValue("@PageLink", item.PageLink);
                        cmd.Parameters.AddWithValue("@ParentID", item.ParentID);
                       


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



        //public bool SFDelete(Int32 FunctionID)
        //{
        //    var result = false;
        //    try
        //    {
        //        using (SqlConnection con = new SqlConnection(appify_connectionstring))
        //        {
        //            using (SqlCommand cmd = new SqlCommand(dbroutine.DBStoredProc.REMOVESECURABLEFUNCTION))
        //            {
        //                cmd.CommandType = CommandType.StoredProcedure;
        //                cmd.Connection = con;
        //                cmd.Parameters.AddWithValue("@FunctionID", FunctionID);

        //                con.Open();
        //                result = Convert.ToBoolean(cmd.ExecuteNonQuery());

        //                con.Close();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }

        //    return result;
        //}

        //public SecurablesFunction SFGet(Int32 FunctionID)
        //{
        //    SecurablesFunction item = new SecurablesFunction();
        //    DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.SELECTSECURABLEFUNCTION, FunctionID);
        //    item = DataTableHelper.ConvertDataTable<SecurablesFunction>(ds.Tables[0]).FirstOrDefault();

        //    return item;
        //}

        //public Int64 SFGetSecurablesCount()
        //{

        //    Int64 item = new Int64();
        //    DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.ROWCOUNTROLE);
        //    item = Convert.ToInt64(ds.Tables[0].Rows[0][0].ToString());

        //    return item;
        //}

        //public List<SecurablesFunction> SFList(Int32 SecurablesID)
        //{
        //    List<SecurablesFunction> item = new List<SecurablesFunction>();
        //    DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.LISTSECURABLEFUNCTION, SecurablesID);
        //    item = DataTableHelper.ConvertDataTable<SecurablesFunction>(ds.Tables[0]);

        //    return item;
        //}

        //public List<SecurablesFunction> SFListAll()
        //{
        //    List<SecurablesFunction> item = new List<SecurablesFunction>();
        //    DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.LISTALLSECURABLEFUNCTION);
        //    item = DataTableHelper.ConvertDataTable<SecurablesFunction>(ds.Tables[0]);

        //    return item;
        //}

        //public SecurablesFunction SFSave(SecurablesFunction item)
        //{
        //    var result = false;

        //    try
        //    {
        //        using (SqlConnection con = new SqlConnection(appify_connectionstring))
        //        {
        //            using (SqlCommand cmd = new SqlCommand(dbroutine.DBStoredProc.SAVESECURABLEFUNCTION))
        //            {
        //                cmd.CommandType = CommandType.StoredProcedure;
        //                cmd.Connection = con;

        //                cmd.Parameters.AddWithValue("@FunctionID", item.FunctionID);
        //                cmd.Parameters.AddWithValue("@SecurableID", item.SecurableID);
        //                cmd.Parameters.AddWithValue("@FunctionName", item.FunctionName);
        //                cmd.Parameters.AddWithValue("@AccessLevel", item.AccessLevel);

        //                //Add the output parameter to the command object
        //                SqlParameter outPutParameter = new SqlParameter();
        //                outPutParameter.ParameterName = "@NewFunctionID";
        //                outPutParameter.SqlDbType = System.Data.SqlDbType.SmallInt;
        //                outPutParameter.Direction = System.Data.ParameterDirection.Output;
        //                cmd.Parameters.Add(outPutParameter);

        //                con.Open();
        //                result = Convert.ToBoolean(cmd.ExecuteNonQuery());

        //                if (outPutParameter.Value != null && outPutParameter.Value != "" && outPutParameter.Value != System.DBNull.Value)
        //                    item.SecurableID = Convert.ToInt32(outPutParameter.Value);
        //                else
        //                    item.SecurableID = 0;

        //                con.Close();
        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }

        //    return item;
        //}

    }
}
