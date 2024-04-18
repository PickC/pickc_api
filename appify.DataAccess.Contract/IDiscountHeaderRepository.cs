using appify.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appify.DataAccess.Contract
{
    public interface IDiscountHeaderRepository
    {
        public List<DiscountHeader> GetAll(Int64 DiscountID);
        public DiscountHeader Get(Int64 DiscountID);
        public DiscountHeader Save(DiscountHeader item);
        public bool Remove(Int64 DiscountID, Int64 ModifiedBy);
        public List<ProductDiscount> ListByVendor(long vendorID);


    }
}
