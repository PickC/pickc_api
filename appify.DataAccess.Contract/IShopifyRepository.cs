using appify.models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appify.DataAccess.Contract
{
    public interface IShopifyRepository
    {
        public bool SaveShopifyProduct(Shopify shopifyProduct);
        public bool SaveShopifyProductVarient(ShopifyProductVariant item);
        public bool SaveShopifyProductVarientImage(ShopifyProductVariantImage item);
        public ShopifyConfig GetShopifyConfigByVendor(long VendorID);
        public bool BulkInsertShopifyProducts(DataTable shopifyProductMaster, DataTable shopifyProductVariant, DataTable shopifyProductImage);
    }
}
