using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appify.models
{
    public partial class DiscountHeader
    {
        // Constructor 
        public DiscountHeader() {

            DiscountDetails = new List<DiscountDetail>();
        
        }

        // Public Members 


        public Int64 DiscountID { get; set; }
        public Int64 VendorID { get; set; }

        public Int16 DiscountType { get; set; }

        public decimal DiscountValue {  get; set; }
        public DateTime EffectiveDate { get; set; }

        public DateTime ExpiryDate { get; set; }

        public bool IsCancel { get; set; }

        public Int64 CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public Int64 ModifiedBy { get; set; }

        public DateTime ModifiedOn { get; set; }

        public List<DiscountDetail>? DiscountDetails { get; set; }


    }

    public partial class DiscountDetail {

        public DiscountDetail()
        {
                
        }

        public Int64 DiscountID { get; set; }
        public Int64 ProductID { get; set; }
        public bool IsActive { get; set; }
    }
}
