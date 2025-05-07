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
    public class VendorWebModuleBusiness : IVendorWebModuleBusiness
    {
        private IVendorWebModuleRepository repository;
        public VendorWebModuleBusiness(IVendorWebModuleRepository repository) { 
        this.repository = repository;
        }
        public MemberUser SaveVendorUser(MemberUser item)
        {
            return repository.SaveVendorUser(item);
        }
        public List<MemberUserLite> GetVendorUserList(long VendorID)
        {
            return repository.GetVendorUserList(VendorID);
        }
        public MemberUserLite GetVendorUser(long UserID)
        {
            return repository.GetVendorUser(UserID);
        }
        public bool UpdateVendorUser(long UserID, bool IsActive)
        {
            return repository.UpdateVendorUser(UserID, IsActive);
        }
    }
}
