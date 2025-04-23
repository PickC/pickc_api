using appify.DataAccess.Contract;
using appify.models;
using appify.utility;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;


namespace appify.DataAccess
{
    public partial class SubscriptionItemRepository : ISubscriptionItemRepository
    {
        private IConfiguration configuration;
        private string appify_connectionstring;

        public const string SELECTSUBSCRIPTIONITEM = "[Master].[usp_SubscriptionItemSelect]";
        public const string LISTSUBSCRIPTIONITEM = "[Master].[usp_SubscriptionItemList]";
        public const string SAVESUBSCRIPTIONITEM = "[Master].[usp_SubscriptionItemSave]";
        public const string REMOVESUBSCRIPTIONITEM = "[Master].[usp_SubscriptionItemDelete]";

        public SubscriptionItemRepository(IConfiguration config)
        {
            this.configuration = config;
            this.appify_connectionstring = config["ConnectionStrings:appify.connectionstring"].ToString();
        }

        public SubscriptionItem GetSubscriptionItem(short itemID, short planID, short featureID)
        {
            SubscriptionItem item = new SubscriptionItem();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, SELECTSUBSCRIPTIONITEM,itemID,planID,featureID);
                item = DataTableHelper.ConvertDataTable<SubscriptionItem>(ds.Tables[0]).FirstOrDefault();
                con.Close();
            }

            return item;
        }

        public List<SubscriptionItem> ListSubscriptionItem(short planID)
        {
            List<SubscriptionItem> items = new List<SubscriptionItem>();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, LISTSUBSCRIPTIONITEM,planID);
                items = DataTableHelper.ConvertDataTable<SubscriptionItem>(ds.Tables[0]);
                con.Close();
            }

            return items;

        }
        public bool SaveSubscriptionItem(SubscriptionItem itemData)
        {

            var result = false;
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(SAVESUBSCRIPTIONITEM))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@ItemID", itemData.ItemID);
                        cmd.Parameters.AddWithValue("@PlanID", itemData.PlanID);
                        cmd.Parameters.AddWithValue("@FeatureID", itemData.FeatureID);
                        cmd.Parameters.AddWithValue("@Value", itemData.Value);
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

        public bool DeleteSubscriptionItem(short itemID, short planID, short featureID)
        {

            var result = false;
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(REMOVESUBSCRIPTIONITEM))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@ItemID", itemID);
                        cmd.Parameters.AddWithValue("@PlanID", planID);
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
