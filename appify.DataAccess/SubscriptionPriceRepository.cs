using appify.DataAccess.Contract;
using appify.models;
using appify.utility;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;


namespace appify.DataAccess
{
    public partial class SubscriptionPriceRepository : ISubscriptionPriceRepository
    {
        private IConfiguration configuration;
        private string appify_connectionstring;

        public const string SELECTSUBSCRIPTIONPRICE = "[Master].[usp_SubscriptionPriceSelect]";
        public const string LISTSUBSCRIPTIONPRICE = "[Master].[usp_SubscriptionPriceList]";
        public const string SAVESUBSCRIPTIONPRICE = "[Master].[usp_SubscriptionPriceSave]";
        public const string REMOVESUBSCRIPTIONPRICE = "[Master].[usp_SubscriptionPriceDelete]";
        public const string LISTSUBSCRIPTIONPRICEBYPLAN = "[Master].[usp_SubscriptionPriceListByPlan]";



        public SubscriptionPriceRepository(IConfiguration config)
        {
            this.configuration = config;
            this.appify_connectionstring = config["ConnectionStrings:appify.connectionstring"].ToString();
        }

        public SubscriptionPrice GetSubscriptionPrice(short priceID)
        {
            SubscriptionPrice item = new SubscriptionPrice();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, SELECTSUBSCRIPTIONPRICE,priceID);
                item = DataTableHelper.ConvertDataTable<SubscriptionPrice>(ds.Tables[0]).FirstOrDefault();
                con.Close();
            }

            return item;
        }

        public List<SubscriptionPrice> ListSubscriptionPrice()
        {
            List<SubscriptionPrice> items = new List<SubscriptionPrice>();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, LISTSUBSCRIPTIONPRICE);
                items = DataTableHelper.ConvertDataTable<SubscriptionPrice>(ds.Tables[0]);
                con.Close();
            }

            return items;

        }

        public List<SubscriptionPrice> ListSubscriptionPriceByPlan(short planID) {
            List<SubscriptionPrice> items = new List<SubscriptionPrice>();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, LISTSUBSCRIPTIONPRICE,planID);
                items = DataTableHelper.ConvertDataTable<SubscriptionPrice>(ds.Tables[0]);
                con.Close();
            }

            return items;

        }
        public bool SaveSubscriptionPrice(SubscriptionPrice itemData)
        {

            var result = false;
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(SAVESUBSCRIPTIONPRICE))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@PriceID", itemData.PriceID);
                        cmd.Parameters.AddWithValue("@Price", itemData.Price);
                        cmd.Parameters.AddWithValue("@Term", itemData.Term);
                        cmd.Parameters.AddWithValue("@PlanID", itemData.PlanID);
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

        public bool DeleteSubscriptionPrice(short priceID)
        {

            var result = false;
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(REMOVESUBSCRIPTIONPRICE))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@PriceID", priceID);
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
