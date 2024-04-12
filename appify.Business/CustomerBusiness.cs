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
    public class CustomerBusiness : ICustomerBusiness
    {
        private ICustomerRepository repository;
        public CustomerBusiness(ICustomerRepository repository) {
            this.repository = repository;
        }
        public List<Member> GetAllCustomersByVendor(long vendorID, int pageNo, int rows)
        {
            return this.repository.GetAllCustomersByVendor(vendorID, pageNo, rows); 
        }

        public List<MemberProduct> ProductList(long vendorID) {
            return this.repository.ProductList(vendorID);
        }
    }
}
