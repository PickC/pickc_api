using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace appify.models
{
    public partial class MemberUser
    {
        public long UserID { set; get; }
        public long VendorID { set; get; }
        public short MemberType { set; get; }
        public string FirstName {  set; get; }
        public string LastName { set; get; }
        public string MobileNo { set; get; }
        public string? Password { get; set; }
        public short Createdby {  set; get; }
        public DateTime CreatedOn {  set; get; }
        public short ModifiedBy { set; get; }
        public DateTime ModifiedOn { set; get; }
        public bool IsActive { set; get; }
        public bool IsEmail { set; get; }
        public bool IsReset { set; get; }
    }
    public partial class MemberUserLite
    {
        public long UserID { set; get; }
        public long VendorID { set; get; }
        public short MemberType { set; get; }
        public string FirstName { set; get; }
        public string LastName { set; get; }
        public string MobileNo { set; get; }
        public bool IsActive { set; get; }
    }
    public partial class MemberUserUpdate
    {
        public long UserID { set; get; }
        public bool IsActive { set; get; }
    }
    public partial class MemberUserInvitation
    {
        public long UserID { set; get; }
        public string MobileNo { set; get; }
    }
}
