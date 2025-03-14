/*
 * Company: AppifyRetail.
 * Author: Gurjeet
 * Version: 1.1
 * Date: 2024-09-01
 * Description:
*/
using Swashbuckle.AspNetCore;
using Swashbuckle.AspNetCore.Filters;
using System.ComponentModel.DataAnnotations;


namespace appify.web.api
{
    public class ParamMemberUserID
    {
        public long userID { get; set; }

    }
    public class ParamMIDMType
    {
        public long userID { get; set; }
        public short userType {  get; set; }

    }
    public class ParamUserID
    {
        public Int32 userID { get; set; }
    }
    public class ParamCategoryID
    {
        public long userID { get; set; }
        public long categoryID { get; set; }
        public short PageNo { get; set; }
        public short Rows { get; set; }
    }
    public class ParamMemberOrder : ParamMemberUserID
    {
        public string? OrderStatus { get; set; }
        public short PageNo { get; set; }
        public short Rows { get; set; }
    }
    public class ParamProductList : ParamMemberUserID
    {
        public short PageNo { get; set; }
        public short Rows { get; set; }
    }
    public class ParamPaymentList
    {
        public short PageNo { get; set; }
        public short Rows { get; set; }
    }

    public class ParamPageNumber
    {
        public short PageNo { get; set; }
        public short Rows { get; set; }
    }
    public class ParamMemberDashboard
    {
        public long userID { get; set; }
        public DateTime dateFrom { get; set; }
        public DateTime dateTo { get; set; }
    }
    public class ParamMemberVendorID
    {
        public long userID { get; set; }

    }

    public class ParamVendorCategories : ParamMemberVendorID
    {
        public long ParentCatID { get; set; }
        public bool IsActive { get; set; }
    }
    public class ParamMemberVendorIDPagination : ParamPageNumber {
        public long userID { get; set; }

    }
    public class ParamMemberNotificationID
    {
        public long NotificationID { get; set; }

    }
    public class ParamBannerID
    {
        public long bannerID { get; set; }
    }

    public class ParamNewProductsByMember : ParamMemberUserID
    {
        public bool IsNew { get; set; }
    }
    public class ParamEventID
    {
        public long EventID { get; set; }
    }
    public class ParamNewProduct
    {
        public long ProductID { get; set; }
        public bool IsNew { get; set; }

    }

    public class ParamMemberResetPassword : ParamMemberUserID
    {
        public string password { get; set; }

    }

    public class ParamSystemConfigSetting
    {
        public string SettingKey { get; set; }
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

    public class ParamLogIn
    {
        public string emailID { get; set; }
        public string password { get; set; }

    }
    public class ParamEmail
    {
        public string emailID { get; set; }
    }
    public class ParamProduct
    {
        public long productID { get; set; }

        public bool? IsActive { get; set; }
    }

    public class ParamParent
    {
        public long parentID { get; set; }
    }

    public class ParamCatID
    {
        public long categoryID { get; set; }
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

    public class ParamAddress : ParamMemberUserID
    {
        public long addressID { get; set; }
    }

    public class ParamAppSetting : ParamMemberUserID
    {
        public string appName { get; set; }

    }


    public class ParamLookup
    {
        public long lookupID { get; set; }
    }

    public class ParamLookupCode
    {
        public string lookupCode { get; set; }
        public string category { get; set; }
    }

    public class ParamLookupCategory
    {
        public string? category { get; set; }
    }

    public class ParamLookupByMember : ParamLookupCategory
    {
        public string userID { get; set; }

    }
    public class ParamOrderStatus
    {
        public Int64 OrderID { get; set; }
        public short OrderStatus { get; set; }
        public string Remarks { get; set; }

    }

    public class ParamSettlementStatus
    {
        public Int64 OrderID { get; set; }
        public bool Status { get; set; }

    }
    public class ParamOrderForPickup
    {
        public Int64 OrderID { get; set; }

        public decimal Weight { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }


    }


    public class ParamLookupCategoryList
    {
        public List<ParamLookupCategory>? list { get; set; }
    }

    public class ParamLookupCategories
    {
        public string list { get; set; }
    }
    public class ParamOrderAWB
    {
        public Int64 OrderID { get; set; }

        public string CourierRefID { get; set; }
        public string ShipmentID { get; set; }
        public string AWB { get; set; }


    }


    public class ParamMemberTheme
    {

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
        public long ModifiedBy { get; set; }

    }

    public class ParamDiscountDetail
    {
        public long DiscountID { get; set; }
        public long productID { get; set; }
    }
    public class ParamVendorOrder
    {
        public long VendorID { get; set; }
        public long OrderID { get; set; }
    }
    public class ParamOrderItem
    {
        public Int64 OrderID { get; set; }
        public string RazorpayPaymentId { get; set; }
        public string RazorpayOrderId { get; set; }
        public string RazorpaySignature { get; set; }
    }
    public class ParamVendorPayment
    {
        public Int64 PaymentID { get; set; }
    }
    public class ParamVendor
    {
        public Int64 VendorID { get; set; }
    }
    public class ParamSMSCredentials
    {
        public Int16 SMSTemplateID { get; set; }
        public string Name {  get; set; }
        public string MobileNo { get; set; }
        public string MessageTitle { get; set; }
        public string MessageBody { get; set; }
        public string FirstName {  get; set; }
        public string OrderNo {  get; set; }
    }
    public class ParamEmailFields
    {
        [Required]
        public string ToEmail { get; set; }
        [Required]
        public string Subject { get; set; }
        [Required]
        public string Body { get; set; }

        [Required]
        public IFormFile file { get; set; }
    }
    public class ParamDownTimeAlert
    {
        public string Service { get; set; }
        public Int64 MemberID {  get; set; }
        public Int32 MemberType {  get; set; }
        public Int64 OrderID { get; set; }
        public string AppVersion {  get; set; }
        public string AppName {  get; set; }
    }

    public class ParamVerifySignature
    {
        public Int64 OrderID { get; set; }
        public string razorpayPaymentId { get; set; }
        public string razorpayOrderId { get; set; }
        public string razorpaySignature {  get; set; }
    }

    public class ParamPageView
    {
        public short PageNo { get; set; }
        public short Rows { get; set; }
    }

    public class ParamRole {
        public short RoleID { get; set; }

    }
    public class ParamSecurableID
    {
        public short SecurableID { get; set; }

    }

    public class ParamFunctionID
    {
        public short FunctionID { get; set; }
    }
    public class ParamFilter
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }


    public class ParamRoleRights {
        public short RoleID { get; set; }
        public short SecurableID { get; set; }

    }


    public class ParamCategoryParameter
    {
        public short ParameterID { get; set; }
        public short CategoryID { get; set; }

    }



}
