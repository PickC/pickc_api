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

        public SubscriptionHeaderLite GetSubscriptionHeader(short planID)
        {
            SubscriptionHeaderLite item = new SubscriptionHeaderLite();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, SELECTSUBSCRIPTIONHEADER,planID);
                item = DataTableHelper.ConvertDataTable<SubscriptionHeaderLite>(ds.Tables[0]).FirstOrDefault();
                con.Close();
            }

            return item;
        }

        public List<SubscriptionHeaderLite> ListSubscriptionHeader()
        {
            List<SubscriptionHeaderLite> items = new List<SubscriptionHeaderLite>();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, LISTSUBSCRIPTIONHEADER);
                items = DataTableHelper.ConvertDataTable<SubscriptionHeaderLite>(ds.Tables[0]);
                con.Close();
            }

            return items;

        }
        public SubscriptionHeader SaveSubscriptionHeader(SubscriptionHeader itemData)
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
                        cmd.Parameters.AddWithValue("@IsActive", itemData.IsActive);
                        cmd.Parameters.AddWithValue("@CreatedBy", itemData.CreatedBy);
                        cmd.Parameters.AddWithValue("@ModifiedBy", itemData.ModifiedBy);

                        SqlParameter outPutParameter = new SqlParameter();
                        outPutParameter.ParameterName = "@NewPlanID";
                        outPutParameter.SqlDbType = SqlDbType.SmallInt;
                        outPutParameter.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(outPutParameter);

                        con.Open();
                        result = Convert.ToBoolean(cmd.ExecuteNonQuery());

                        itemData.PlanID = Convert.ToInt16(outPutParameter.Value);
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
