using appify.Business.Contract;
using appify.DataAccess.Contract;
using appify.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace appify.Business
{
    public partial class DiscountHeaderBusiness : IDiscountHeaderBusiness
    {
        private IDiscountHeaderRepository _repository;

        public DiscountHeaderBusiness(IDiscountHeaderRepository repository)
        {
            this._repository = repository;
        }
        public DiscountHeader Get(long DiscountID)
        {
            return _repository.Get(DiscountID);
        }

        public List<DiscountHeader> GetAll(long DiscountID)
        {
           return _repository.GetAll(DiscountID);
        }

        public List<DiscountHeader> ListByVendor(long vendorID)
            
        {
            return _repository.ListByVendor(vendorID);
        }


        public bool Remove(long DiscountID, long ModifiedBy)
        {
            return _repository.Remove(DiscountID, ModifiedBy);
        }

        public DiscountHeader Save(DiscountHeader item)
        {
            return _repository.Save(item);
        }
    }
}
