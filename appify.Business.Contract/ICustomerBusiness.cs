using appify.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appify.Business.Contract
{
    public interface ICustomerBusiness
    {
        public List<Member> GetAllCustomersByVendor(long vendorID, int pageNo, int rows);

        public List<MemberProduct> ProductList(long vendorID);
        public List<MemberProduct> ProductListNew(long vendorID);
        public MemberAllDetail GetMemberAllDetails(long userID);
        public HomePageProductByCategory GetProductListByVAUA(long userID);
        public List<MemberProduct> ProductListByCategory(long vendorID, long CategoryID, int pageNo, int rows);
        public List<MemberPassword> GetMemberPasswordList();
        public bool SaveMemberPassword(long userID, string password);
    }
}
