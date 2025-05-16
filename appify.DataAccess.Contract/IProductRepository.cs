using appify.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appify.DataAccess.Contract
{
    public interface IProductRepository
    {
        public bool HasProduct(long productId);
        public ProductMaster SaveProduct(ProductMaster product);
        public ProductMaster SaveBulkImportedProduct(ProductMaster productmaster);
        public bool DeleteProduct(long productId, bool? IsActive);

        public ProductMaster GetProduct(long productId);
        public ProductMasterNew GetProductNew(long productId);
        public List<ProductMaster> GetProducts(long sellerID);
        public List<ProductMaster> GetAllProducts();

        public List<ProductWeb> ListAll();

        public bool UpdateProductImagePrice(long productID);
        public bool UpdateBulkImportedProductRemark(long ItemID, string Remarks, string Error);
        public List<NewProduct> GetNewProductsList(long VendorID, bool IsNew = false);

        public bool UpdateNewProducts(long ProductID, bool IsNew);
        public List<ProductMasterCategories> GetProductMasterCategories(long parentID);
        public List<ProductCategories> GetCategoriesList(long parentID);
        public List<ProductCategories> GetALLCategoriesList(long parentID, string? SearchFilter, short? pageNo, short? rows);
        public String GetALLCategoriesListJSON(long parentID);
        public List<ProductCategoryName> GetCategorieName(long categoryID);
        public ParentCategories SaveVendorCategories(ParentCategories vendorCategories);
        public List<ParentCategories> GetVendorCategories(long VendorID);

        public List<ParentCategories> GetALLVendorCategories(long VendorID);
        public List<FeaturedCategories> GetFeaturedCategories(long VendorID);

        public bool UpdateFeaturedCategories(FeaturedCategories item);
        public bool DeleteFeaturedCategories(long VendorID);

        public List<StockByPriceID> GetStockByPriceID(string PriceID);
    }
}
