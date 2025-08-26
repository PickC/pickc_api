using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appify.models
{
    public partial class MetaProduct
    {
        public string CatalogID { get; set; }
        public string RetailerID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public string ImageUrl { get; set; }
        public string Brand { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; }
        public string Availability { get; set; }
    }
}
