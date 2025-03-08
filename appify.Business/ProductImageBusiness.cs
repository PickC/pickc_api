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
    public partial class ProductImageBusiness : IProductImageBusiness
    {

        private IProductImageRepository repository;

        public ProductImageBusiness(IProductImageRepository repository)
        {
            this.repository = repository;
        }

        public bool AddProductImage(ProductImage productImage)
        {
            return repository.AddProductImage(productImage);
        }

        public ProductImage GetProductImage(long imageID,long productID)
        {
            return repository.GetProductImage(imageID, productID);
        }

        public List<ProductImage> GetProductImages(long productID)
        {
            return repository.GetProductImages(productID);
        }
        public List<ProductImageNew> GetProductImagesNew(long productID)
        {
            return repository.GetProductImagesNew(productID);
        }
        public bool RemoveProductImage(long imageID,long productID)
        {
            return repository.RemoveProductImage(imageID, productID);
        }
    }
}
