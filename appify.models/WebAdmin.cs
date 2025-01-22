using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appify.models
{
    public partial class WebAdmin
    {
    }
    public partial class User
    {
        public Int32 UserID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public Int16 UserGroup { get; set; }
        public Int16 UserDesignation { get; set; }
        public string EmployeeID { get; set; }
        public string ICNo { get; set; }
        public string EmailID { get; set; }
        public string ContactNo { get; set; }
        public bool IsActive { get; set; }
        public bool IsAllowLogOn { get; set; }
        public bool IsOperational { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public Int64 BranchID { get; set; }
        public string OTPNo { get; set; }
        public bool IsOTPSent { get; set; }
        public DateTime OTPSentDate { get; set; }
        public bool IsOTPReSent { get; set; }
        public Int16 OTPSentCount { get; set; }
        public bool IsOTPVerified { get; set; }
        public string RoleCode { get; set; }
    }


    public partial class Roles
    {
        public Int16 RoleCode { get; set; }
        public string RoleDescription { get; set; }
        public bool IsActive { get; set; }
        public Int16 CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public Int16 ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }

    }
}
