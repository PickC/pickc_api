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
    public partial class VendorServiceCredentials
    {
        public short ServiceID { set; get; }
        public long VendorID { set; get; }
        public string ServiceCategory { set; get; }
        public short ServiceValue { set; get; }
        public string KeyID { set; get; }
        public string SecretKey { set; get; }
        public string Token { set; get; }
        public bool IsEnabled { set; get; }
        public short CreatedBy { set; get; }
        public short ModifiedBy { set; get; }
        public bool IsActive { set; get; }
    }
    public partial class ParamService
    {
        public long ServiceID { set; get; }
        public long VendorID { set; get; }
    }
    public partial class VendorServices
    {
        public long ServiceID { set; get; }
        public long VendorID { set; get; }
        public bool OnlinePayment { set; get; }
        public bool RazorPay { set; get; }
        public bool EazeBuzz { set; get; }
        public bool Shiprocket { set; get; }
        public bool Delhivery { set; get; }
        public bool COD { set; get; }
        public bool Shopify { set; get; }
        public bool Facebook { set; get; }
        public bool Instagram { set; get; }
        public bool BulkUpload { set; get; }
        public bool SMS { set; get; }
        public bool Email { set; get; }
        public bool PushNotification { set; get; }
        public bool InApp { set; get; }
        public bool WhatsApp { set; get; }
        public bool IsActive { set; get; }
        public short CreatedBy { set; get; }
        public short ModifiedBy { set; get; }
    }
}
