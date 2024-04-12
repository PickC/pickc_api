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
    }
}
