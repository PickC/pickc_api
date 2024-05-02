using appify.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appify.Business.Contract
{
    public interface IDiscountHeaderBusiness
    {
        public List<DiscountHeader> GetAll();
        public DiscountHeader Get(Int64 DiscountID);
        public DiscountHeader Save(DiscountHeader item);
        public bool Remove(Int64 DiscountID, Int64 ProductID);

        public List<ProductDiscount> ListByVendor(long vendorID);
        public List<ProductDiscountList> ListByProduct(long productID);

    }
}
