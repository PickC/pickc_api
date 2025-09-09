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
    public partial class FacebookBusiness : IFacebookBusiness
    {
        private IFacebookRepository repository;
        public FacebookBusiness(IFacebookRepository repository)
        {
            this.repository = repository;
        }
        public List<MetaCatalogConfig> VendorCatalogList(long VendorID, string BusinessID)
        {
            return repository.VendorCatalogList(VendorID, BusinessID);
        }
        public List<MetaBusinessConfig> VendorBusinessProfileList(long VendorID)
        {
            return repository.VendorBusinessProfileList(VendorID);
        }
        public MetaBusinessProfile SaveBusinessProfile(MetaBusinessProfile itemData)
        {
            return repository.SaveBusinessProfile(itemData);
        }
        public bool DeleteBusinessProfile(MetaBusinessProfileDelete itemData)
        {
            return repository.DeleteBusinessProfile(itemData);
        }
        public MetaCatalog CreateaProductCatalog(MetaCatalog itemData)
        {
            return repository.CreateaProductCatalog(itemData);
        }
        public bool DeleteProductCatalog(MetaCatalogDelete itemData)
        {
            return repository.DeleteProductCatalog(itemData);
        }
        public bool DeleteCatalogProduct(MetaCatalogProuctDelete itemData)
        {
            return repository.DeleteCatalogProduct(itemData);
        }
        public bool DeleteALLCatalogProducts(long VendorID, string CatalogID)
        {
            return repository.DeleteALLCatalogProducts(VendorID, CatalogID);
        }
        public MetaCatalogProduct CreateaProductToCatalog(MetaCatalogProduct itemData)
        {
            return repository.CreateaProductToCatalog(itemData);
        }
        public MetaApiConfig GetMetaApiConfig(string BusinessID, long VendorID)
        {
             return repository.GetMetaApiConfig(BusinessID, VendorID);
        }
        public List<MetaProduct> ProductListMeta(long VendorID, short SourceID)
        {
            return repository.ProductListMeta(VendorID, SourceID);
        }
    }
}
