using appify.DataAccess.Contract;
using appify.models;
using appify.utility;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace appify.DataAccess
{
    public partial class CategoryParameterRepository : ICategoryParameterRepository
    {
        private IConfiguration _configuration;
        private string appify_connectionstring;

        public CategoryParameterRepository(IConfiguration configuration)
        {
            this._configuration = configuration;
            this.appify_connectionstring = configuration["ConnectionStrings:appify.connectionstring"].ToString();
        }


        public bool Delete(long parameterID, long categoryID)
        {
            var result = false;
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(dbroutine.DBStoredProc.REMOVECATEGORYPARAMETER))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@ParameterID", parameterID);
                        cmd.Parameters.AddWithValue("@CategoryID", categoryID);

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

        public CategoryParameter Get(long parameterID, long categoryID)
        {
            CategoryParameter item = new CategoryParameter();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.SELECTCATEGORYPARAMETER, parameterID, categoryID);
            item = DataTableHelper.ConvertDataTable<CategoryParameter>(ds.Tables[0]).FirstOrDefault();

            return item;
        }

        public List<CategoryParameter> ListAll(long categoryID)
        {
            List<CategoryParameter> item = new List<CategoryParameter>();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.LISTCATEGORYPARAMETER, categoryID);
            item = DataTableHelper.ConvertDataTable<CategoryParameter>(ds.Tables[0]);

            return item;
        }

        public bool Save(CategoryParameter item)
        {
            var result = false;

            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(dbroutine.DBStoredProc.SAVECATEGORYPARAMETER))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;

                        cmd.Parameters.AddWithValue("@ParameterID", item.ParameterID);
                        cmd.Parameters.AddWithValue("@CategoryID", item.CategoryID);
                        cmd.Parameters.AddWithValue("@ParameterName", item.ParameterName);
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


    public partial class ParameterTypeRepository : IParameterTypeRepository
    {
        private IConfiguration _configuration;
        private string appify_connectionstring;

        public ParameterTypeRepository(IConfiguration configuration)
        {
            this._configuration = configuration;
            this.appify_connectionstring = configuration["ConnectionStrings:appify.connectionstring"].ToString();
        }


        public bool Delete(long parameterID, string parameterValue)
        {
            var result = false;
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(dbroutine.DBStoredProc.REMOVEPARAMETERTYPE))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@ParameterID", parameterID);
                        cmd.Parameters.AddWithValue("@ParameterValue", parameterValue);

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

        public ParameterType Get(long parameterID, string parameterValue)
        {
            ParameterType item = new ParameterType();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.SELECTPARAMETERTYPE, parameterID, parameterValue);
            item = DataTableHelper.ConvertDataTable<ParameterType>(ds.Tables[0]).FirstOrDefault();

            return item;
        }

        public List<ParameterType> ListAll(long parameterID)
        {
            List<ParameterType> item = new List<ParameterType>();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.LISTPARAMETERTYPE, parameterID);
            item = DataTableHelper.ConvertDataTable<ParameterType>(ds.Tables[0]);

            return item;
        }

        public bool Save(ParameterType item)
        {
            var result = false;

            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(dbroutine.DBStoredProc.SAVEPARAMETERTYPE))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;

                        cmd.Parameters.AddWithValue("@ParameterID", item.ParameterID);
                        cmd.Parameters.AddWithValue("@ParameterValue", item.ParameterName);
                        cmd.Parameters.AddWithValue("@IsMultipleValue", item.IsMultipleValue);

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
