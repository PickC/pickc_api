using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace appify.models
{
	public partial class Subscription
	{
		public Subscription() { }


		public Int32  SubscriptionID { get; set; }
		public string  PlanName { get; set; }
		public string  PlanDescription { get; set; }
		public decimal AppliedCommission { get; set; }
		public Int32  WarehouseCount { get; set; }
		public Int32  UserAccountCount { get; set; }
		public bool  HasEcommerceIntegration { get; set; }
		public string  EcommercePlatforms { get; set; }
		public bool  HasBulkUpload { get; set; }
		public bool  HasProductCatalog { get; set; }
		public bool  HasInvoice { get; set; }
		public bool  HasSMSService { get; set; }
		public Int32  DiscountCouponCount { get; set; }
		public bool  HasAnalytics { get; set; }
		public bool  HasStoreLocation { get; set; }
		public bool  IsWhiteLabeled { get; set; }
		public bool  HasAccountManager { get; set; }
		public Int32  ImageEnhancerCount { get; set; }
		public Int32  ProductListingCount { get; set; }
		public Int32  ProductCategoryCount { get; set; }
		public Int32  BannerCount { get; set; }
		public decimal MonthlyFee { get; set; }
		public decimal HalfYearlyFee { get; set; }
		public decimal AnnualFee { get; set; }
		public bool  IsActive { get; set; }
		public DateTime  CreatedOn { get; set; }
		public DateTime  ModifiedOn { get; set; }

        public string? Features { get; set; }
		public string? MarketingTools { get; set; }
		public string? SupportTypes { get; set; }
		public string? PaymentGateways { get; set; }
        public string? DeliveryPartners { get; set; }

    }
}




