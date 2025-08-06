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

        public SubscriptionPriceLite GetSubscriptionPrice(short priceID)
        {
            SubscriptionPriceLite item = new SubscriptionPriceLite();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, SELECTSUBSCRIPTIONPRICE,priceID);
                item = DataTableHelper.ConvertDataTable<SubscriptionPriceLite>(ds.Tables[0]).FirstOrDefault();
                con.Close();
            }

            return item;
        }

        public List<SubscriptionPriceLite> ListSubscriptionPrice()
        {
            List<SubscriptionPriceLite> items = new List<SubscriptionPriceLite>();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, LISTSUBSCRIPTIONPRICE);
                items = DataTableHelper.ConvertDataTable<SubscriptionPriceLite>(ds.Tables[0]);
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
        public SubscriptionPrice SaveSubscriptionPrice(SubscriptionPrice itemData)
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
                        cmd.Parameters.AddWithValue("@IsActive", itemData.IsActive);

                        SqlParameter outPutParameter = new SqlParameter();
                        outPutParameter.ParameterName = "@NewPriceID";
                        outPutParameter.SqlDbType = SqlDbType.SmallInt;
                        outPutParameter.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(outPutParameter);

                        con.Open();
                        result = Convert.ToBoolean(cmd.ExecuteNonQuery());

                        itemData.PriceID = Convert.ToInt16(outPutParameter.Value);
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
