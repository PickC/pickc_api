using appify.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appify.DataAccess.Contract
{
    public interface IDiscountDetailRepository
    {
        public List<DiscountDetail> GetAll(Int64 DiscountID, Int64 ProductID);
        public DiscountDetail Get(Int64 DiscountID, Int64 ProductID);
        public DiscountDetail Save(DiscountDetail item);
        public bool Remove(Int64 DiscountID, Int64 ProductID);
    }
}
