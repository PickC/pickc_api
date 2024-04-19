using Swashbuckle.AspNetCore;
using Swashbuckle.AspNetCore.Filters;


namespace appify.web.api
{
    public class ParamMemberUserID
    {
        public long userID { get; set; }

    }
    public class ParamNewUserID
    {
        public long ProductID { get; set; }
        public int IsNew { get; set; }

    }

    public class ParamMemberResetPassword : ParamMemberUserID
    {
        public string password { get; set; }

    }


    public class ParamDeactivateMember  
    {
        public string mobileNo { get; set; }
        public string password { get; set; }

    }


    public class ParamCheckMember
    {
        public string emailID { get; set; }
        public string mobileNo { get; set; }
        public short memberType { get; set; }
        public short vendorID { get; set; }
    }

    public class ParamSignIn
    {
        public string emailID { get; set; }
        public string mobileNo { get; set; }
        public string password { get; set; }
        public Int64 parentID { get; set; }

    }

    public class ParamProduct
    {
        public long productID { get; set; }

    }

    public class ParamProductPrice : ParamProduct
    {
        public long priceID { get; set; }
        public string size { get; set; }

    }

    public class ParamProductImage : ParamProduct
    {

        public long imageID { get; set; }
    }

    public class ParamAddress : ParamMemberUserID { 
        public long addressID { get; set; } 
    }

    public class ParamAppSetting : ParamMemberUserID
    {
        public string appName { get; set; }

    }


    public class ParamLookup
    {
        public short lookupID { get; set; }
    }

    public class ParamLookupCategory
    {
        public string? category { get; set; }
    }

    public class ParamLookupByMember : ParamLookupCategory
    {
        public string userID{ get; set; }

    }

    public class ParamOrderStatus {
        public Int64 OrderID { get; set; }
        public short OrderStatus { get; set; }
        public string Remarks { get; set;}

    }


    public class ParamOrderForPickup
    {
        public Int64 OrderID { get; set; }

        public decimal Weight { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }


    }



    public class ParamOrderAWB
    {
        public Int64 OrderID { get; set; }

        public string CourierRefID { get; set; }
        public string ShipmentID { get; set; }
        public string AWB { get; set; }


    }


    public class ParamMemberTheme {

        public Int64 MemberID { get; set; }
        public Int64 ThemeID { get; set; }
    }


    public class ParamMemberContact
    {

        public Int64 MemberID { get; set; }
        public string? MobileNo { get; set; }
    }

    public class ParamDiscount
    {
        public long DiscountID { get; set; }

    }

    public class ParamDiscountRemove
    {
        public long DiscountID { get; set; }
        public long ModifiedBy {  get; set; }

    }

    public class ParamDiscountDetail
    {
        public long DiscountID { get; set; }
        public long productID { get; set; }
    }
}
