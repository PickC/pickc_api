using appify.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appify.DataAccess.Contract
{
    public interface IAddressRepository
    {
        public Address SaveAddress(Address item);
        public bool DeleteAddress(long addressID,long linkID);
        public Address GetAddress(long addressID, long linkID);
        public List<Address> GetList(long linkID);

        public Address GetDefaultAddress(long linkID);
    }
}
