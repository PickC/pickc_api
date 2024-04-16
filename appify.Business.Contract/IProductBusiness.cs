using appify.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appify.Business.Contract
{
    public interface IProductBusiness
    {
        public bool HasProduct(long productId);
        public ProductMaster SaveProduct(ProductMaster product);
        public ProductMaster SaveProduct(Product product);

        public bool DeleteProduct(long productId);

        public ProductMaster GetProduct(long productId);
        public List<ProductMaster> GetProducts(long sellerID);
        public List<ProductMaster> GetAllProducts();

        public List<ProductWeb> ListAll();

        public bool UpdateProductImagePrice(long productID);

        public bool UpdateProductIsNewStatus(List<NewProduct> newProducts);

        public List<NewProduct> NewProducts(long sellerID);

    }
}
