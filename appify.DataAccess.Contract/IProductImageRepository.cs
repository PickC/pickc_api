using appify.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appify.DataAccess.Contract
{
    public interface IProductImageRepository
    {
        public List<ProductImage> GetProductImages(long productID);
        public ProductImage GetProductImage(long imageID,long productID);
        public bool AddProductImage(ProductImage productImage);
        public bool RemoveProductImage(long imageID, long productID);

    }
}
