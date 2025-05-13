using appify.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appify.Business.Contract
{
    public interface IProductPriceBusiness
    {
        public List<ProductPrice> PriceList(long productID);
        public List<ProductPriceNew> PriceListNew(long productID);
        public ProductPrice GetPrice(long priceID, long productID, string size);
        public bool RemovePrice(long priceID, long productID, string size);
        public bool SavePrice(ProductPrice price);
        public bool SaveBulkPrice(ProductPrice productprice);
    }
}
