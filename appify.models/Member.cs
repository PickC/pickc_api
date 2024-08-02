using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appify.models
{
    public partial class Member
    {

        // Constructor 
        public Member() { }

        // Public Members 

        public Int64 UserID { get; set; }

        public string EmailID { get; set; }

        public string MobileNo { get; set; }

        public string Password { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public Int16 MemberType { get; set; }

        public string OTP { get; set; }

        public bool IsOTPSent { get; set; }

        public DateTime? OTPSentDate { get; set; }

        public bool? IsResendOTP { get; set; }

        public bool? IsOTPVerified { get; set; }

        public bool? IsEmailVerified { get; set; }

        public bool? IsActive { get; set; }

        public DateTime? CreatedOn { get; set; }

        public string? ProfilePhoto { get; set; }

        public string? Token { get; set; }
        public Int16 PlatformType { get; set; }

        public Int64? ParentID { get; set; }

        public bool? IsRegisteredByMobile { get; set; }

        public bool? IsOnlinePaymentEnabled { get; set; }
        public bool? IsEnterprise { get; set; }
        public bool? IsEcommerce { get; set; }



    }

    public partial class MemberReturnPolicy 
    {
        // Constructor 
        public MemberReturnPolicy() { }

		// Public Members 

		public Int64 MemberID { get; set; }

        public Int16 MaxReturnDays { get; set; }

        public bool IsProductDamaged { get; set; }
        public bool IsDeliveryDelay { get; set; }

        public bool IsWrongSize { get; set; }

        public bool InCompatible { get; set; }
        public bool IsQualityIssue { get; set; }
        public bool IsDifferentProduct { get; set; }
        public bool IsNotNeeded { get; set; }

        public bool IsOthers { get; set; }

        public bool IsImage { get; set; }

        public bool IsVideo { get; set; }

        public string? Remarks { get; set; }

        public bool IsActive { get; set; }
    }


    public class MemberAppSetting  
    {
        
		// Public Members 

		public Int64 UserID { get; set; }

        public string AppName { get; set; }
        public string? AppName1 { get; set; }
        public string? AppName2 { get; set; }

        public string? Description { get; set; }

        public string? Logo { get; set; }

        public string? PlayStoreID { get; set; }

        public string? AppStoreID { get; set; }
        public string? AppIcon { get; set; }


    }

    public partial class MemberDashboard {

        
        public long DeliveredOrders { get; set; }
        public long TotalRevenue { get; set; }
        public long PendingOrders { get; set; }
        public int TotalProducts { get; set; }
        public int AdTotalInstalls { get; set; }
        public int AdTotalSpends { get; set;}

        public int AdTotalConversions { get; set; }
        public int AdROAS { get; set; }

        public List<DashboardProducts>? Products { get; set; }
        public class DashboardProducts {

            public string? ProductName { get; set; }
            public int TotalOrders { get; set; }
        }

    }
    public partial class MemberDashboardLite
    {
        public long VendorID { get; set; }
        public decimal TotalRevenue { get; set; }
        public Int32 PendingOrder { get; set; }
        public Int32 CompletedOrder { get; set; }
        public Int32 ActiveOrder {  get; set; }
    }
    public partial class MemberKYC {

        public Int64 MemberID { get; set; }
        public string? PAN { get; set; }
        public string? GST { get; set; }
        public string? AadharNo { get; set; }
        public string? BankName { get; set; }
        public string? BankAccountNo { get; set; }
        public string? IFSC { get; set; }
        public short? BankAccountType { get; set; }
        public string? ChequeImage { get; set; }
        public string? PANImage { get; set; }
        public string? GSTImage { get; set; }
        public string? AadharImage { get; set; }
        public string? AadharImage2 { get; set; }
        public string? KVICNo { get; set; }
        public string? Address { get; set; }
        public string? AddressImage { get; set; }


    }


    public partial class MemberContact
    {

        public Int64 MemberID { get; set; }

        public string MobileNo { get; set; }
        public string ContactName { get; set; }

        public string? EmailID { get; set; }





    }

    public partial class MemberBanner
    {
        public Int64 BannerID { get; set; }
        public Int64 MemberID { get; set; }
        public string BannerName { get; set; }
        public string ImageName { get; set; }
        public Int16 BannerType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsCancel { get; set; }
    }

}
