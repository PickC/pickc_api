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

        public SubscriptionItemLite GetSubscriptionItem(short itemID, short planID, short featureID)
        {
            SubscriptionItemLite item = new SubscriptionItemLite();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, SELECTSUBSCRIPTIONITEM,itemID,planID,featureID);
                item = DataTableHelper.ConvertDataTable<SubscriptionItemLite>(ds.Tables[0]).FirstOrDefault();
                con.Close();
            }

            return item;
        }

        public List<SubscriptionItemLite> ListSubscriptionItem(short planID)
        {
            List<SubscriptionItemLite> items = new List<SubscriptionItemLite>();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, LISTSUBSCRIPTIONITEM,planID);
                items = DataTableHelper.ConvertDataTable<SubscriptionItemLite>(ds.Tables[0]);
                con.Close();
            }

            return items;

        }
        public SubscriptionItem SaveSubscriptionItem(SubscriptionItem itemData)
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
                        cmd.Parameters.AddWithValue("@IsActive", itemData.IsActive);
                        cmd.Parameters.AddWithValue("@CreatedBy", itemData.CreatedBy);
                        cmd.Parameters.AddWithValue("@ModifiedBy", itemData.ModifiedBy);

                        SqlParameter outPutParameter = new SqlParameter();
                        outPutParameter.ParameterName = "@NewItemID";
                        outPutParameter.SqlDbType = SqlDbType.SmallInt;
                        outPutParameter.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(outPutParameter);

                        con.Open();
                        result = Convert.ToBoolean(cmd.ExecuteNonQuery());
                        itemData.ItemID = Convert.ToInt16(outPutParameter.Value);

                        con.Close();
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return itemData;
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
