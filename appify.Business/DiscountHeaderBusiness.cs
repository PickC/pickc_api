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
        private IDiscountHeaderRepository repository;
        private IDiscountDetailRepository repositoryDetail;

        public DiscountHeaderBusiness(IDiscountHeaderRepository repository, IDiscountDetailRepository repositoryDetail)
        {
            this.repository = repository;
            this.repositoryDetail = repositoryDetail;
        }
        public DiscountHeader Get(long DiscountID)
        {

            return repository.Get(DiscountID);
        }

        public List<DiscountHeader> GetAll(long DiscountID)
        {
           return repository.GetAll(DiscountID);
        }

        public List<ProductDiscount> ListByVendor(long vendorID)
        {

            return repository.ListByVendor(vendorID);
        }


        public bool Remove(long DiscountID, long ModifiedBy)
        {
            return repository.Remove(DiscountID, ModifiedBy);
        }

        public DiscountHeader Save(DiscountHeader item)
        {
            DiscountHeader returnItem = repository.Save(item);

            if (item.DiscountDetails?.Any()==true)
            {
                
                foreach (var dt in item.DiscountDetails)
                {
                    dt.DiscountID = returnItem.DiscountID;
                    var result = repositoryDetail.Save(dt);
                }
            }

            return returnItem;
        }
    }
}
