using appify.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appify.Business.Contract
{
    public interface IAddressBusiness
    {
        public Address SaveAddress(Address item);
        public bool DeleteAddress(long AddressID, long linkID);
        public Address GetAddress(long AddressID, long linkID);
        public List<Address> GetList(long linkID);
        public Address GetDefaultAddress(long linkID);
    }
}
