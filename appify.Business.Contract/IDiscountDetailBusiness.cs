using appify.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appify.Business.Contract
{
    public interface IDiscountDetailBusiness
    {
        public List<DiscountDetail> GetAll(Int64 DiscountID, Int64 ProductID);
        public DiscountDetail Get(Int64 DiscountID, Int64 ProductID);
        public DiscountDetail Save(DiscountDetail item);
        public bool Remove(Int64 DiscountID, Int64 ProductID);
    }
}
