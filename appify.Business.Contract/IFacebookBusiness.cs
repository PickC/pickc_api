using appify.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appify.Business.Contract
{
    public interface IFacebookBusiness
    {
        public List<MetaBusinessConfig> VendorBusinessProfileList(long VendorID);
        public MetaBusinessProfile SaveBusinessProfile(MetaBusinessProfile itemData);
        public bool DeleteBusinessProfile(MetaBusinessProfileDelete itemData);
        public List<MetaCatalogConfig> VendorCatalogList(long VendorID, string BusinessID);
        public MetaCatalog CreateaProductCatalog(MetaCatalog itemData);
        public bool DeleteProductCatalog(MetaCatalogDelete itemData);
        public bool DeleteCatalogProduct(MetaCatalogProuctDelete itemData);
        public bool DeleteALLCatalogProducts(long VendorID, string CatalogID);
        public MetaCatalogProduct CreateaProductToCatalog(MetaCatalogProduct itemData);
        public MetaApiConfig GetMetaApiConfig(string BusinessID, long VendorID);
        public List<MetaProduct> ProductListMeta(long VendorID, short SourceID);
    }
}
