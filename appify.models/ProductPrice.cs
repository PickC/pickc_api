using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appify.models
{
    public partial class ProductPrice
    {
        public Int64 PriceID { get; set; }

        public Int64 ProductID { get; set; }

        public decimal? Price { get; set; }

        public decimal? Discount { get; set; }

        public Int16? DiscountType { get; set; }

        public DateTime? EffectiveDate { get; set; }

        public bool? IsActive { get; set; }

        public string Size { get; set; }
        public short Stock { get; set; }

        public decimal? Weight{ get; set; }

    }

    public partial class ProductPriceNew
    {
        public Int64 PriceID { get; set; }

        public decimal? Price { get; set; }

        public decimal? Discount { get; set; }
        public Int16? DiscountType { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public string Size { get; set; }
        public short Stock { get; set; }

        public decimal? Weight { get; set; }

    }
}
