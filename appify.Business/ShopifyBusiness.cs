using appify.Business.Contract;
using appify.DataAccess.Contract;
using appify.models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appify.Business
{
    public partial class ShopifyBusiness : IShopifyBusiness
    {
        private IShopifyRepository repository;

        public ShopifyBusiness(IShopifyRepository repository)
        {
            this.repository = repository;
        }
        public bool SaveShopifyProduct(Shopify shopifyProduct)
        {
            var rslt = false;
            if (shopifyProduct.variants?.Any() == true && shopifyProduct.images.Any() == true)
            {

                var item = repository.SaveShopifyProduct(shopifyProduct);
                if (item != null)
                {
                    if (shopifyProduct.variants.Any() == true)
                    {
                        //shopifyProduct.variants.ForEach(v => { v.ProductID = shopifyProduct.ProductID; });
                        foreach (var variant in shopifyProduct.variants)
                        {
                            rslt = repository.SaveShopifyProductVarient(variant);
                        }
                    }


                    if (shopifyProduct.images.Any() == true)
                    {
                        //shopifyProduct.images.ForEach(i => { i.ProductID = shopifyProduct.ProductID; });
                        foreach (var image in shopifyProduct.images)
                        {
                            rslt = repository.SaveShopifyProductVarientImage(image);
                        }
                    }
                }
            }
            return rslt;
        }
        public bool SaveShopifyProductVarient(ShopifyProductVariant item)
        {
            return repository.SaveShopifyProductVarient(item);
        }
        public bool SaveShopifyProductVarientImage(ShopifyProductVariantImage item)
        {
            return repository.SaveShopifyProductVarientImage(item);
        }

        public ShopifyConfig GetShopifyConfigByVendor(long VendorID)
        {
            return repository.GetShopifyConfigByVendor(VendorID);
        }
        public bool BulkInsertShopifyProducts(DataTable shopifyProductMaster, DataTable shopifyProductVariant, DataTable shopifyProductImage)
        {
            return repository.BulkInsertShopifyProducts(shopifyProductMaster, shopifyProductVariant, shopifyProductImage);
        }
    }
}
