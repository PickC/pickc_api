using appify.Business.Contract;
using appify.DataAccess.Contract;
using appify.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appify.Business
{
    public partial class ProductPriceBusiness : IProductPriceBusiness
    {
        private IProductPriceRepository repository;

        public ProductPriceBusiness(IProductPriceRepository repository)
        {
            this.repository = repository;
        }

        public ProductPrice GetPrice(long priceID,long productID,string size)
        {
            return repository.GetPrice(priceID, productID,size);
        }

        public List<ProductPrice> PriceList(long productID)
        {
            return repository.PriceList(productID);
        }
        public List<ProductPriceNew> PriceListNew(long productID)
        {
            return repository.PriceListNew(productID);
        }

        public bool RemovePrice(long priceID,long productID,string size)
        {
            return repository.RemovePrice(priceID, productID,size);
        }

        public bool SavePrice(ProductPrice price)
        {
            return repository.SavePrice (price);
        }
        public bool SaveBulkPrice(ProductPrice productprice)
        {
            return repository.SaveBulkPrice(productprice);
        }
    }
}
