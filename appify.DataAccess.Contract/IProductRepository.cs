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
        public bool DeleteProduct(long productId);

        public ProductMaster GetProduct(long productId);
        public List<ProductMaster> GetProducts(long sellerID);
        public List<ProductMaster> GetAllProducts(); 

        public List<ProductWeb> ListAll();

        public bool UpdateProductImagePrice(long productID);

        public List<ProductMaster> GetNewProductsList(long VendorID);

        public bool UpdateNewProducts(long ProductID, bool IsNew);

        public bool UpdateProductIsNewStatus(long productID, bool isNew);



        public List<NewProduct> NewProducts(long sellerID);


    }
}
