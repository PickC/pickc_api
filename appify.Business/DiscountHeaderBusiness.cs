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
        public List<OrderDiscount> GetDiscountByVendor(Int64 VendorID)
        {
            return repository.GetDiscountByVendor(VendorID);
        }
        public List<OrderDiscount> GetDiscountListbyVendorRows(long vendorID, int pageNo, int rows)
        {
            return repository.GetDiscountListbyVendorRows(vendorID, pageNo, rows);
        }
        public DiscountHeader Get(long DiscountID)
        {

            return repository.Get(DiscountID);
        }
        public OrderDiscount GetDiscount(Int64 DiscountID)
        {
            return repository.GetDiscount(DiscountID); 
        }

        public List<OrderDiscountDetail> GetOrderDiscountByVendor(Int64 VendorID)
        {
            return repository.GetOrderDiscountByVendor (VendorID);
        }
        public Int64 GetDiscountCount(Int64 VendorID)
        {
            return repository.GetDiscountCount(VendorID);
        }
        public List<DiscountHeader> GetAll()
        {
           return repository.GetAll();
        }

        public List<ProductDiscount> ListByVendor(long vendorID)
        {

            return repository.ListByVendor(vendorID);
        }

        public List<ProductDiscountList> ListByProduct(long productID) { 
            return repository.ListByProduct(productID);
        }



        public bool Remove(long DiscountID, long ProductID)
        {
            return repository.Remove(DiscountID, ProductID);
        }

        public DiscountHeader Save(DiscountHeader item)
        {
            DiscountHeader returnItem = repository.Save(item);

            //if (item.DiscountDetails?.Any()==true)
            //{
                
            //    foreach (var dt in item.DiscountDetails)
            //    {
            //        dt.DiscountID = returnItem.DiscountID;
            //        var result = repositoryDetail.Save(dt);
            //    }
            //}

            return returnItem;
        }
        public OrderDiscount DiscountSave(OrderDiscount item)
        {
            return repository.DiscountSave(item);
        }
        public bool DiscountRemove(Int64 DiscountID)
        {
            return repository.DiscountRemove(DiscountID);
        }
    }
}
