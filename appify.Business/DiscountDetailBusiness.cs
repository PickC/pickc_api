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
    public partial class DiscountDetailBusiness : IDiscountDetailBusiness
    {
        private IDiscountDetailRepository _repository;

        public DiscountDetailBusiness(IDiscountDetailRepository repository)
        {
            this._repository = repository;
        }
        public DiscountDetail Get(long DiscountID, long ProductID)
        {
            return this._repository.Get(DiscountID, ProductID);
        }

        public List<DiscountDetail> GetAll(long DiscountID, long ProductID)
        {
            return _repository.GetAll(DiscountID, ProductID);
        }

        public bool Remove(long DiscountID, long ProductID)
        {
            return _repository.Remove(DiscountID, ProductID);
        }

        public DiscountDetail Save(DiscountDetail item)
        {
            return _repository.Save(item);
        }
    }
}
