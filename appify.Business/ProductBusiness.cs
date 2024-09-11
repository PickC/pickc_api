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

        public ProductBusiness(IProductRepository repository)
        {
            this.repository = repository;
        }

        public ProductBusiness(IProductRepository repository, IProductImageRepository imageRepository, IProductPriceRepository priceRepository)
        {
            this.repository = repository;
            this.priceRepository = priceRepository;
            this.imageRepository = imageRepository;

        }
        public bool DeleteProduct(long productId)
        {
            return repository.DeleteProduct(productId);
        }

        public bool UpdateProductImagePrice(long productID)
        {

            return repository.UpdateProductImagePrice(productID);
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

            productMaster = repository.GetProductNew(productId);
            if(productMaster!=null)
            {
                productMaster.prices = priceRepository.PriceListNew(productId);
                productMaster.images = imageRepository.GetProductImagesNew(productId);
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
    }
}
