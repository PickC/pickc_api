using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appify.models
{
    public partial class ProductMaster
    {
        public Int64 ProductID { get; set; }

        public Int64 VendorID { get; set; }

        public string ProductName { get; set; }

        public string? Description { get; set; }

        public Int16? Category { get; set; }

        public string? Brand { get; set; }

        public string? Size { get; set; }

        public string? Color { get; set; }

        public Int16? UOM { get; set; }

        public decimal Weight { get; set; }

        public Int16 PriceID { get; set; }

        public string Currency { get; set; }

        public Int64 ImageID { get; set; }

        public bool IsActive { get; set; }

        public bool? IsAvailable { get; set; }

        public Int32? StockQty { get; set; }

        public DateTime? CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public string HSNCode { get; set; }

        public string? ImageName { get; set; }

        public bool? IsNew { get; set; }
    }


    public partial class Product : ProductMaster
    {

        public List<ProductPrice>? prices { get; set; }
        public List<ProductImage>? images { get; set; }

    }

    public partial class ProductMasterNew
    {
        public ProductMasterNew()
        {
            prices = new List<ProductPriceNew>();
            images = new List<ProductImageNew>();
        }
        public Int64 ProductID { get; set; }

        public string ProductName { get; set; }

        public string? Description { get; set; }

        public Int16? Category { get; set; }

        public string? Brand { get; set; }

        public string? Color { get; set; }

        public Int16? UOM { get; set; }

        public string Currency { get; set; }

        public bool? IsAvailable { get; set; }

        public string HSNCode { get; set; }

        public bool? IsNew { get; set; }

        public List<ProductPriceNew>? prices { get; set; }
        public List<ProductImageNew>? images { get; set; }
    }

    public partial class ProductWeb : ProductMaster
    {

        public string VendorName { get; set; }
        public string CategoryDescription { get; set; }

    }
    public partial class MemberAllDetail
    {
        public bool? WareHouse { get; set; }
        public short Products { get; set; }
        public bool? Category { get; set; }
        public bool? AppDetails { get; set; }
    }
    public partial class MemberProduct
    {

        public Int64 ProductID { get; set; }

        public Int64 VendorID { get; set; }

        public string ProductName { get; set; }
        public Int16? Category { get; set; }

        public string? Brand { get; set; }
        public decimal? Price { get; set; }
        public string? ImageName { get; set; }

        public bool? IsNew { get; set; }

        public Int16? DiscountType { get; set; }
        public decimal? DiscountValue { get; set; }

    }
    public partial class NewProduct
    {

        public Int64 ProductID { get; set; }

        public string ProductName { get; set; }
        public string Description { get; set; }
        public string Brand { get; set; }
        public decimal? Price { get; set; }
        public bool IsNew { get; set; }


    }

    public partial class ProductMasterCategories
    {
        public long CategoryID { get; set; }
        public string Category { get; set; }
    }

    public partial class HomePageProductByCategory
    {
        public HomePageProductByCategory()
        {
            categories = new List<ProductMasterCategories>();
            products = new List<MemberProduct>();
            //productdetails = new List<ProductMaster>();
        }

        public List<ProductMasterCategories>? categories { get; set; }
        public List<MemberProduct>? products { get; set; }

        //public List<ProductMaster>? productdetails { get; set; }
    }
}
