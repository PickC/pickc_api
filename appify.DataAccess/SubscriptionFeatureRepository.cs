using appify.DataAccess.Contract;
using appify.models;
using appify.utility;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;


namespace appify.DataAccess
{
    public partial class SubscriptionFeatureRepository : ISubscriptionFeatureRepository
    {
        private IConfiguration configuration;
        private string appify_connectionstring;

        public const string SELECTSUBSCRIPTIONFEATURE = "[Master].[usp_SubscriptionFeatureSelect]";
        public const string LISTSUBSCRIPTIONFEATURE = "[Master].[usp_SubscriptionFeatureList]";
        public const string SAVESUBSCRIPTIONFEATURE = "[Master].[usp_SubscriptionFeatureSave]";
        public const string REMOVESUBSCRIPTIONFEATURE = "[Master].[usp_SubscriptionFeatureDelete]";

        public SubscriptionFeatureRepository(IConfiguration config)
        {
            this.configuration = config;
            this.appify_connectionstring = config["ConnectionStrings:appify.connectionstring"].ToString();
        }

        public SubscriptionFeature GetSubscriptionFeature(short featureID)
        {
            SubscriptionFeature item = new SubscriptionFeature();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, SELECTSUBSCRIPTIONFEATURE,featureID);
                item = DataTableHelper.ConvertDataTable<SubscriptionFeature>(ds.Tables[0]).FirstOrDefault();
                con.Close();
            }

            return item;
        }

        public List<SubscriptionFeature> ListSubscriptionFeature()
        {
            List<SubscriptionFeature> items = new List<SubscriptionFeature>();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, LISTSUBSCRIPTIONFEATURE);
                items = DataTableHelper.ConvertDataTable<SubscriptionFeature>(ds.Tables[0]);
                con.Close();
            }

            return items;

        }
        public bool SaveSubscriptionFeature(SubscriptionFeature itemData)
        {

            var result = false;
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(SAVESUBSCRIPTIONFEATURE))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@FeatureID", itemData.FeatureID);
                        cmd.Parameters.AddWithValue("@FeatureName", itemData.FeatureName);
                        cmd.Parameters.AddWithValue("@Description", itemData.Description);
                        cmd.Parameters.AddWithValue("@IsActive", itemData.IsActive);
                        cmd.Parameters.AddWithValue("@CreatedBy", itemData.CreatedBy);
                        cmd.Parameters.AddWithValue("@ModifiedBy", itemData.ModifiedBy);

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

        public bool DeleteSubscriptionFeature(short featureID)
        {

            var result = false;
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(REMOVESUBSCRIPTIONFEATURE))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@FeatureID", featureID);
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
