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
    }
}
