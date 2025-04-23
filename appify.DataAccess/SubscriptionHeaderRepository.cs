using appify.DataAccess.Contract;
using appify.models;
using appify.utility;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;


namespace appify.DataAccess
{
    public partial class SubscriptionHeaderRepository : ISubscriptionHeaderRepository
    {
        private IConfiguration configuration;
        private string appify_connectionstring;

        public const string SELECTSUBSCRIPTIONHEADER = "[Master].[usp_SubscriptionHeaderSelect]";
        public const string LISTSUBSCRIPTIONHEADER = "[Master].[usp_SubscriptionHeaderList]";
        public const string SAVESUBSCRIPTIONHEADER = "[Master].[usp_SubscriptionHeaderSave]";
        public const string REMOVESUBSCRIPTIONHEADER = "[Master].[usp_SubscriptionHeaderDelete]";

        public SubscriptionHeaderRepository(IConfiguration config)
        {
            this.configuration = config;
            this.appify_connectionstring = config["ConnectionStrings:appify.connectionstring"].ToString();
        }

        public SubscriptionHeader GetSubscriptionHeader(short planID)
        {
            SubscriptionHeader item = new SubscriptionHeader();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, SELECTSUBSCRIPTIONHEADER,planID);
                item = DataTableHelper.ConvertDataTable<SubscriptionHeader>(ds.Tables[0]).FirstOrDefault();
                con.Close();
            }

            return item;
        }

        public List<SubscriptionHeader> ListSubscriptionHeader()
        {
            List<SubscriptionHeader> items = new List<SubscriptionHeader>();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, LISTSUBSCRIPTIONHEADER);
                items = DataTableHelper.ConvertDataTable<SubscriptionHeader>(ds.Tables[0]);
                con.Close();
            }

            return items;

        }
        public bool SaveSubscriptionHeader(SubscriptionHeader itemData)
        {

            var result = false;
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(SAVESUBSCRIPTIONHEADER))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@PlanID", itemData.PlanID);
                        cmd.Parameters.AddWithValue("@PlanName", itemData.PlanName);
                        cmd.Parameters.AddWithValue("@Description", itemData.Description);
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

        public bool DeleteSubscriptionHeader(short planID)
        {

            var result = false;
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(REMOVESUBSCRIPTIONHEADER))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@PlanID", planID);
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
