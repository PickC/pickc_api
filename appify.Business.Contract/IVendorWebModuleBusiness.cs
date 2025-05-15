using appify.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appify.Business.Contract
{
    public interface IVendorWebModuleBusiness
    {
        public MemberUser SaveVendorUser(MemberUser item);
        public List<MemberUserLite> GetVendorUserList(long VendorID);
        public MemberUserLite GetVendorUser(long UserID);
        public bool UpdateVendorUser(long UserID, bool IsActive);
        public bool UpdateUserPassword(long UserID, string MobileNo, string Password);
        public MemberUser MemberLogIn(string mobileNo, string password, Int64 parentID);
        public string CheckUserMobileNo(string mobileNo);
    }
}
