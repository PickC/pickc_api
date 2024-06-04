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
    public partial class LookUpRepository : ILookUpRepository
    {
        private IConfiguration configuration;
        private string appify_connectionstring;


        public LookUpRepository(IConfiguration config)
        {
            this.configuration = config;
            this.appify_connectionstring = config["ConnectionStrings:appify.connectionstring"].ToString();
        }
        public bool DeleteLookUp(short lookupID)
        {
            var result = false;
            //DataTable dt = DataTableHelper.CreateDataTableFromObj(item);
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(dbroutine.DBStoredProc.DELETELOOKUP))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@LookupID", lookupID);


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

        public List<Lookup> GetAllList()
        {
            List<Lookup> item = new List<Lookup>();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.LISTLOOKUP);
            item = DataTableHelper.ConvertDataTable<Lookup>(ds.Tables[0]);

            return item;
        }

        public List<SystemConfigSetting> GetSystemConfigurationSettings(string SettingKey)
        {
            List<SystemConfigSetting> item = new List<SystemConfigSetting>();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.LISTSYSTEMCONFIGSETTING, SettingKey);
            item = DataTableHelper.ConvertDataTable<SystemConfigSetting>(ds.Tables[0]);

            return item;
        }

        public List<Lookup> GetList(string category)
        {
            List<Lookup> items = new List<Lookup>();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.LISTLOOKUPBYCATEGORY, category);
            items = DataTableHelper.ConvertDataTable<Lookup>(ds.Tables[0]);

            return items;
        }
        public List<Lookup> GetList(string category,string userID)
        {
            List<Lookup> items = new List<Lookup>();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.LISTLOOKUPBYMEMBERCATEGORY, category,userID);
            items = DataTableHelper.ConvertDataTable<Lookup>(ds.Tables[0]);

            return items;
        }

        public Lookup GetLookUp(string lookupCode,string category)
        {
            Lookup item = new Lookup();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.SELECTLOOKUP,lookupCode, category);
            item = DataTableHelper.ConvertDataTable<Lookup>(ds.Tables[0]).FirstOrDefault();

            return item;
        }

        public Lookup GetLookUp(short lookupID)
        {
            throw new NotImplementedException();
        }

        public bool HasLookUp(short lookupID)
        {
            throw new NotImplementedException();
        }

        public bool SaveLookUp(Lookup lookup)
        {
            var result = false;
            //DataTable dt = DataTableHelper.CreateDataTableFromObj(item);
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(dbroutine.DBStoredProc.SAVELOOKUP))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;

                        cmd.Parameters.AddWithValue("@LookupID",  lookup.LookupID);
                        cmd.Parameters.AddWithValue("@LookupCode",  lookup.LookupCode);
                        cmd.Parameters.AddWithValue("@LookupDescription", lookup.LookupDescription);
                        cmd.Parameters.AddWithValue("@LookupCategory", lookup.LookupCategory);
                        cmd.Parameters.AddWithValue("@MappingCode",  lookup.MappingCode);
                        cmd.Parameters.AddWithValue("@CreatedBy",  lookup.CreatedBy);
                        cmd.Parameters.AddWithValue("@ModifiedBy", lookup.ModifiedBy);


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
