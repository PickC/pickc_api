using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace appify.models
{
	public partial class SubscriptionHeader
	{
		public SubscriptionHeader() { }


		public Int16  PlanID { get; set; }
		public string  PlanName { get; set; }
		public string  Description { get; set; }
		public bool  IsActive { get; set; }
		public Int16  CreatedBy { get; set; }
		public DateTime  CreatedOn { get; set; }
		public Int16  ModifiedBy { get; set; }
		public DateTime  ModifiedOn { get; set; }
	}

    public partial class SubscriptionItem
    {
        public SubscriptionItem() { }


        public Int16 ItemID { get; set; }
        public Int16 PlanID { get; set; }
        public Int16 FeatureID { get; set; }
        public string Value { get; set; }
        public bool IsActive { get; set; }
        public Int16 CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public Int16 ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
    }

    public partial class SubscriptionPrice
    {
        public SubscriptionPrice() { }


        public Int16 PriceID { get; set; }
        public decimal Price { get; set; }
        public Int16 Term { get; set; }
        public Int16 PlanID { get; set; }
        public Int16 CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public Int16 ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
    }

    public partial class SubscriptionFeature
    {
        public SubscriptionFeature() { }


        public Int16 FeatureID { get; set; }
        public string FeatureName { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public Int16 CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public Int16 ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
    }

}




