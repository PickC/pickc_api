using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appify.models
{
    public partial class Subscription
    {
        public int SubscriptionID { get; set; } 
        public string PlanName { get; set; } 
        public decimal? AppifyCommission { get; set; } 
        public short? NumberOfWarehouses { get; set; } 
        public short? NumberOfStaffAccounts { get; set; } 
        public bool? EcommerceIntegration { get; set; } 
        public bool? BulkUpload { get; set; } 
        public bool? ProductCatalog { get; set; } 
        public short? PaymentGateway { get; set; } 
        public short? DeliveryPartner { get; set; } 
        public short? UserAppCustomization { get; set; } 
        public bool? InvoiceBilling { get; set; } 
        public bool?  SMSService { get; set; } 
        public short? DiscountCoupons { get; set; } 
        public short? MarketingTools { get; set; } 
        public bool? AppDownloads { get; set; } 
        public short? Analytics { get; set; } 
        public short? CustomerSupport { get; set; } 
        public bool? SellerStoreLocation { get; set; } 
        public bool? WhiteLabeling { get; set; } 
        public bool? AccountManager { get; set; } 
        public bool? AdvancedFeatures { get; set; } 
        public short? ImageEnhancer { get; set; } 
        public short? ProductListing { get; set; } 
        public short? ProductCategory { get; set; } 
        public short? Banners { get; set; } 
        public decimal? SubscriptionFee { get; set; } 
    }
}
