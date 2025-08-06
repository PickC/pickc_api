using appify.Business.Contract;
using appify.DataAccess.Contract;
using appify.models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appify.Business
{
    public partial class ProductBusiness : IProductBusiness
    {

        private IProductRepository repository;
        private IProductPriceRepository priceRepository;
        private IProductImageRepository imageRepository;
        private IMemberCategoryParametersRepository parametersRepository;

        public ProductBusiness(IProductRepository repository)
        {
            this.repository = repository;
        }

        public ProductBusiness( IProductRepository repository, 
                                IProductImageRepository imageRepository, 
                                IProductPriceRepository priceRepository,
                                IMemberCategoryParametersRepository parametersRepository)
        {
            this.repository = repository;
            this.priceRepository = priceRepository;
            this.imageRepository = imageRepository;
            this.parametersRepository = parametersRepository;

        }
        public bool DeleteProduct(long productId, bool? IsActive)
        {
            return repository.DeleteProduct(productId,IsActive);
        }

        public bool UpdateProductImagePrice(long productID)
        {

            return repository.UpdateProductImagePrice(productID);
        }

        public bool UpdateBulkImportedProductRemark(long ItemID, string Remarks, string Error)
        {
            return repository.UpdateBulkImportedProductRemark(ItemID, Remarks, Error);
        }
        public ProductMaster GetProduct(long productId)
        {
            return repository.GetProduct(productId);

        }
        public ProductMasterNew GetProductNew(long productId)
        {
            ProductMasterNew productMaster = new ProductMasterNew();
            List<ProductPriceNew> prices = new List<ProductPriceNew>();
            List<ProductImageNew> images = new List<ProductImageNew>();
            List<MemberCategoryParametersLite> parameters = new List<MemberCategoryParametersLite>();

            productMaster = repository.GetProductNew(productId);
            if(productMaster!=null)
            {
                productMaster.prices = priceRepository.PriceListNew(productId);
                productMaster.images = imageRepository.GetProductImagesNew(productId);
                productMaster.parameters = parametersRepository.ListMemberCategoryParametersLite(productId);
            }
            return productMaster;
        }

        public List<ProductMaster> GetProducts(long sellerID)
        {
            return repository.GetProducts(sellerID);
        }

        public List<ProductMaster> GetAllProducts()
        {
            return repository.GetAllProducts();
        }

        public List<ProductWeb> ListAll()
        {
            return repository.ListAll();
        }
        public bool HasProduct(long productId)
        {
            throw new NotImplementedException();
        }



        public ProductMaster SaveProduct(ProductMaster product)
        {
            return repository.SaveProduct(product);
        }

        public ProductMaster SaveProduct(Product product)
        {
            var item = repository.SaveProduct(product);
            var result = false;
            if (item != null)
            {
                if (product.prices?.Any() == true)
                {
                    product.prices.ForEach(p => { p.ProductID = item.ProductID; });

                    foreach (var priceItem in product.prices)
                    {
                        result = priceRepository.SavePrice(priceItem);
                    }
                }


                if (product.images?.Any() == true)
                {
                    product.images.ForEach(p => { p.ProductID = item.ProductID; });
                    foreach (var productImage in product.images)
                    {
                        result = imageRepository.AddProductImage(productImage);
                    }
                }
            }

            return item;
        }
        public ProductMaster SaveBulkImportedProduct(ProductMaster productmaster)
        {
            return SaveBulkImportedProduct(productmaster);
        }
        public List<NewProduct> GetNewProductsList(long VendorID, bool IsNew = false)
        {
            return repository.GetNewProductsList(VendorID,IsNew);
        }

        public bool UpdateNewProducts(long ProductID, bool IsNew)
        {
            return repository.UpdateNewProducts(ProductID, IsNew);
        }
        public List<ProductMasterCategories> GetProductMasterCategories(long parentID)
        {
            return repository.GetProductMasterCategories(parentID);
        }
        public List<ProductCategories> GetCategoriesList(long parentID)
        {
            return repository.GetCategoriesList(parentID);
        }
        public List<ProductCategories> GetALLCategoriesList(long parentID, string? SearchFilter=null, short? pageNo=0, short? rows = 0)
        {
            return repository.GetALLCategoriesList(parentID, SearchFilter, pageNo, rows);
        }
        public String GetALLCategoriesListJSON(long parentID)
        {
            return repository.GetALLCategoriesListJSON(parentID);
        }
        public List<ProductCategoryName> GetCategorieName(long categoryID)
        {
            return repository.GetCategorieName(categoryID);
        }
        public ParentCategories SaveVendorCategories(ParentCategories vendorCategories)
        {
            return repository.SaveVendorCategories(vendorCategories);
        }
        public List<ParentCategories> GetVendorCategories(long VendorID)
        {
            return repository.GetVendorCategories(VendorID);
        }

        public List<ParentCategories> GetALLVendorCategories(long VendorID)
        {
            return repository.GetALLVendorCategories(VendorID);
        }
        public List<FeaturedCategories> GetFeaturedCategories(long vendorID)
        {
            return repository.GetFeaturedCategories(vendorID);
        }

        //public bool deletefeaturedcategories(long vendorid) {
        //    return repository.DeleteFeaturedCategories(vendorid);
        //}


        public bool UpdateFeaturedCategories(FeaturedCategories item)
        {
            return repository.UpdateFeaturedCategories(item);
        }

        public List<StockByPriceID> GetStockByPriceID(string PriceID)
        {
            return repository.GetStockByPriceID(PriceID);
        }

        public bool DeleteFeaturedCategories(long VendorID)
        {
            return repository.DeleteFeaturedCategories(VendorID);
        }
    }
}
