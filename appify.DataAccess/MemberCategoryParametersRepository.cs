using appify.DataAccess.Contract;
using appify.models;
using appify.utility;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;


namespace appify.DataAccess
{
    public partial class MemberCategoryParametersRepository : IMemberCategoryParametersRepository
    {
        private IConfiguration configuration;
        private string appify_connectionstring;

        public const string SELECTMEMBERCATEGORYPARAMETERS = "[Operation].[usp_MemberCategoryParametersSelect]";
        public const string LISTMEMBERCATEGORYPARAMETERS = "[Operation].[usp_MemberCategoryParametersList]";
        public const string SAVEMEMBERCATEGORYPARAMETERS = "[Operation].[usp_MemberCategoryParametersSave]";
        public const string REMOVEMEMBERCATEGORYPARAMETERS = "[Operation].[usp_MemberCategoryParametersDelete]";

        public MemberCategoryParametersRepository(IConfiguration config)
        {
            this.configuration = config;
            this.appify_connectionstring = config["ConnectionStrings:appify.connectionstring"].ToString();
        }

        public MemberCategoryParameters GetMemberCategoryParameters(Int64 ID)
        {
            MemberCategoryParameters item = new MemberCategoryParameters();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, SELECTMEMBERCATEGORYPARAMETERS,ID);
                item = DataTableHelper.ConvertDataTable<MemberCategoryParameters>(ds.Tables[0]).FirstOrDefault();
                con.Close();
            }

            return item;
        }

        public List<MemberCategoryParameters> ListMemberCategoryParameters(long ProductID)
        {
            List<MemberCategoryParameters> items = new List<MemberCategoryParameters>();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, LISTMEMBERCATEGORYPARAMETERS,ProductID);
                items = DataTableHelper.ConvertDataTable<MemberCategoryParameters>(ds.Tables[0]);
                con.Close();
            }

            return items;

        }
        public List<MemberCategoryParametersLite> ListMemberCategoryParametersLite(long ProductID)
        {
            List<MemberCategoryParametersLite> items = new List<MemberCategoryParametersLite>();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, LISTMEMBERCATEGORYPARAMETERS, ProductID);
                items = DataTableHelper.ConvertDataTable<MemberCategoryParametersLite>(ds.Tables[0]);
                con.Close();
            }

            return items;

        }
        public bool SaveMemberCategoryParameters(MemberCategoryParameters itemData)
        {

            var result = false;
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(SAVEMEMBERCATEGORYPARAMETERS))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@ID", itemData.ID);
                        cmd.Parameters.AddWithValue("@UserID", itemData.UserID);
                        cmd.Parameters.AddWithValue("@ProductID", itemData.ProductID);
                        cmd.Parameters.AddWithValue("@ParameterID", itemData.ParameterID);
                        cmd.Parameters.AddWithValue("@ParameterValue", itemData.ParameterValue);
                        //cmd.Parameters.AddWithValue("@CreatedOn", itemData.CreatedOn);
                        //cmd.Parameters.AddWithValue("@IsActive", itemData.IsActive);

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

        public bool DeleteMemberCategoryParameters(Int64 ID)
        {

            var result = false;
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(REMOVEMEMBERCATEGORYPARAMETERS))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@ID", ID);
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
