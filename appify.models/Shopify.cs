using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appify.models
{
    public partial class ShopifyProduct
    {
        public short ReferenceID { get; set; }
        public string ProductID { get; set; }
        public string Title { get; set; }
        public string Description {  get; set; }
        public string Handle { get; set; }
        public string Status { get; set; }
        public string Vendor {  get; set; }
        public long VendorID { get; set; }
        public string ProductType {  get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? PublishedAt { get; set; }
        public string LegacyResourceId { get; set; }
        public Int16 TotalInventory { get; set; }
        public bool IsActive { get; set; }
    }
    public partial class ShopifyProductVariant
    {
        public short ReferenceID { get; set; }
        public string VariantID { get; set; }
        public string ProductID { get; set; }
        public string Title { get; set; }
        public string SKU { get; set; }
        public decimal Price { get; set; }
        public Int16 Position { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public string Barcode { get; set; }
        public Int16 Weight { get; set; }
        public string WeightUnit { get; set; }
        public Int16 InventoryQuantity { get; set; }
        public string InventoryItemID { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; }
    }
    public partial class ShopifyProductVariantImage
    {
        public short ReferenceID { get; set; }
        public string ImageID { get; set; }
        public string ProductID { get; set; }
        public string ALT { get; set; }
        public Int16 Width { get; set; }
        public Int16 Height { get; set; }
        public string SRC { get; set; }
        public bool IsActive { get; set; }
    }

    public partial class Shopify : ShopifyProduct
    {
        public Shopify() {

            variants = new List<ShopifyProductVariant>();
            images = new List<ShopifyProductVariantImage>();
        }
        public List<ShopifyProductVariant>? variants { get; set; }
        public List<ShopifyProductVariantImage>? images { get; set; }
    }
    public partial class ShopifyConfig
    {
        public string StoreURL { get; set; }
        public string AccessToken { get; set; }
        public string APIVersion { get; set; }

    }
    public partial class ShopifyProductStock
    {
        public long VendorID { get; set; }
        public string ProductID { get; set; }
        public Int16 InventoryQuantity { get; set; }
    }
}
