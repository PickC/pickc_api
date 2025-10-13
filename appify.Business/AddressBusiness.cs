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
    public partial class AddressBusiness : IAddressBusiness
    {

        private IAddressRepository repository;

        public AddressBusiness(IAddressRepository repository)
        {
            this.repository = repository;
        }

        public bool DeleteAddress(long AddressID, long linkID)
        {
            return repository.DeleteAddress(AddressID, linkID);  
        }

        public Address GetAddress(long AddressID, long linkID)
        {
            return repository.GetAddress(AddressID, linkID);
        }

        public Address GetDefaultAddress(long linkID)
        {
            return repository.GetDefaultAddress(linkID);
        }

        public List<Address> GetList(long linkID)
        {
            return repository.GetList(linkID);
        }

        public Address SaveAddress(Address item)
        {
            return repository.SaveAddress(item);
        }
        public List<Address> GetAddressList()
        {
            return repository.GetAddressList();
        }
    }

}
