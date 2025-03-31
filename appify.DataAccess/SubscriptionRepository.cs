using appify.DataAccess.Contract;
using appify.models;
using appify.utility;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appify.DataAccess
{
    public partial class SubscriptionRepository: ISubscriptionRepository
    {
        private IConfiguration _configuration;
        private string appify_connectionstring;


        public const string SELECTSUBSCRIPTION = "[Master].[usp_SubscriptionSelect]";
        public const string LISTSUBSCRIPTION = "[Master].[usp_SubscriptionList]";
        public const string SAVESUBSCRIPTION = "[Master].[usp_SubscriptionSave]";
        public const string REMOVESUBSCRIPTION = "[Master].[usp_SubscriptionDelete]";


        public SubscriptionRepository(IConfiguration configuration)
        {
            this._configuration = configuration;
            this.appify_connectionstring = configuration["ConnectionStrings:appify.connectionstring"].ToString();
        }

        public bool Delete(int subscriptionID)
        {
            var result = false;
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(REMOVESUBSCRIPTION))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@SubscriptionID", subscriptionID);

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

        public Subscription Get(int subscriptionID)
        {
            Subscription item = new Subscription();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, SELECTSUBSCRIPTION, subscriptionID);
            item = DataTableHelper.ConvertDataTable<Subscription>(ds.Tables[0]).FirstOrDefault();

            return item;
        }


        public List<Subscription> List()
        {
            List<Subscription> item = new List<Subscription>();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, LISTSUBSCRIPTION);
            item = DataTableHelper.ConvertDataTable<Subscription>(ds.Tables[0]);

            return item;
        }

 



        public bool Save(Subscription item)
        {
            var result = false;

            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(SAVESUBSCRIPTION))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;

                        cmd.Parameters.AddWithValue("@SubscriptionID", item.SubscriptionID);
                        cmd.Parameters.AddWithValue("@PlanName", item.PlanName);
                        cmd.Parameters.AddWithValue("@PlanDescription", item.PlanDescription);
                        cmd.Parameters.AddWithValue("@AppifyCommission", item.AppliedCommission);
                        cmd.Parameters.AddWithValue("@WarehouseCount", item.WarehouseCount);
                        cmd.Parameters.AddWithValue("@UserAccountCount", item.UserAccountCount);
                        cmd.Parameters.AddWithValue("@HasEcommerceIntegration", item.HasEcommerceIntegration);
                        cmd.Parameters.AddWithValue("@EcommercePlatforms", item.EcommercePlatforms);
                        cmd.Parameters.AddWithValue("@HasBulkUpload", item.HasBulkUpload);
                        cmd.Parameters.AddWithValue("@HasProductCatalog", item.HasProductCatalog);
                        cmd.Parameters.AddWithValue("@HasInvoice", item.HasInvoice);
                        cmd.Parameters.AddWithValue("@HasSMSService", item.HasSMSService);
                        cmd.Parameters.AddWithValue("@DiscountCouponCount", item.DiscountCouponCount);
                        cmd.Parameters.AddWithValue("@HasAnalytics", item.HasAnalytics);
                        cmd.Parameters.AddWithValue("@HasStoreLocation", item.HasStoreLocation);
                        cmd.Parameters.AddWithValue("@IsWhiteLabeled", item.IsWhiteLabeled);
                        cmd.Parameters.AddWithValue("@HasAccountManager", item.HasAccountManager);
                        cmd.Parameters.AddWithValue("@ImageEnhancerCount", item.ImageEnhancerCount);
                        cmd.Parameters.AddWithValue("@ProductListingCount", item.ProductListingCount);
                        cmd.Parameters.AddWithValue("@ProductCategoryCount", item.ProductCategoryCount);
                        cmd.Parameters.AddWithValue("@BannerCount", item.BannerCount);
                        cmd.Parameters.AddWithValue("@MonthlyFee", item.MonthlyFee);
                        cmd.Parameters.AddWithValue("@HalfYearlyFee", item.HalfYearlyFee);
                        cmd.Parameters.AddWithValue("@AnnualFee", item.AnnualFee);

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
