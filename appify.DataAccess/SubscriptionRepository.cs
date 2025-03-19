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
                    using (SqlCommand cmd = new SqlCommand(dbroutine.DBStoredProc.REMOVESUBSCRIPTION))
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
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.SELECTSUBSCRIPTION, subscriptionID);
            item = DataTableHelper.ConvertDataTable<Subscription>(ds.Tables[0]).FirstOrDefault();

            return item;
        }


        public List<Subscription> List()
        {
            List<Subscription> item = new List<Subscription>();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.LISTSUBSCRIPTION);
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
                    using (SqlCommand cmd = new SqlCommand(dbroutine.DBStoredProc.SAVEROLE))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;

                        cmd.Parameters.AddWithValue("@SubscriptionID", item.SubscriptionID);
                        cmd.Parameters.AddWithValue("@PlanName", item.PlanName);
                        cmd.Parameters.AddWithValue("@AppifyCommission", item.AppifyCommission );
                        cmd.Parameters.AddWithValue("@NumberOfWarehouses", item.NumberOfWarehouses);
                        cmd.Parameters.AddWithValue("@NumberOfStaffAccounts", item.NumberOfStaffAccounts);
                        cmd.Parameters.AddWithValue("@EcommerceIntegration",item.EcommerceIntegration );
                        cmd.Parameters.AddWithValue("@BulkUpload", item.BulkUpload );
                        cmd.Parameters.AddWithValue("@ProductCatalog", item.ProductCatalog );
                        cmd.Parameters.AddWithValue("@PaymentGateway", item.PaymentGateway );
                        cmd.Parameters.AddWithValue("@DeliveryPartner", item.DeliveryPartner );
                        cmd.Parameters.AddWithValue("@UserAppCustomization", item.UserAppCustomization );
                        cmd.Parameters.AddWithValue("@InvoiceBilling", item.InvoiceBilling );
                        cmd.Parameters.AddWithValue("@SMSService", item.SMSService );
                        cmd.Parameters.AddWithValue("@DiscountCoupons", item.DiscountCoupons );
                        cmd.Parameters.AddWithValue("@MarketingTools", item.MarketingTools );
                        cmd.Parameters.AddWithValue("@AppDownloads", item.AppDownloads );
                        cmd.Parameters.AddWithValue("@Analytics", item.Analytics );
                        cmd.Parameters.AddWithValue("@CustomerSupport", item.CustomerSupport );
                        cmd.Parameters.AddWithValue("@SellerStoreLocation", item.SellerStoreLocation );
                        cmd.Parameters.AddWithValue("@WhiteLabeling", item.WhiteLabeling );
                        cmd.Parameters.AddWithValue("@AccountManager", item.AccountManager );
                        cmd.Parameters.AddWithValue("@AdvancedFeatures", item.AdvancedFeatures );
                        cmd.Parameters.AddWithValue("@ImageEnhancer", item.ImageEnhancer);
                        cmd.Parameters.AddWithValue("@ProductListing", item.ProductListing );
                        cmd.Parameters.AddWithValue("@ProductCategory", item.ProductCategory );
                        cmd.Parameters.AddWithValue("@Banners", item.Banners );
                        cmd.Parameters.AddWithValue("@SubscriptionFee", item.SubscriptionFee );


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
