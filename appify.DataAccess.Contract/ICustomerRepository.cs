using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using appify.models;

namespace appify.DataAccess.Contract
{
    public interface ICustomerRepository
    {
        public List<Member> GetAllCustomersByVendor(long vendorID, int pageNo, int rows);

        public List<MemberProduct> ProductList(long vendorID);
        public MemberAllDetail GetMemberAllDetails(long userID);
        public HomePageProductByCategory GetProductListByVAUA(long userID);
        public List<MemberProduct> ProductListByCategory(long vendorID, long CategoryID, int pageNo, int rows);
    }
}
