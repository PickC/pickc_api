using System.Reflection.Metadata;

namespace appify.dbroutine
{
    public sealed class DBStoredProc
    {

        private DBStoredProc() { }


        /// <summary>
        /// [Operation].[Member]
        /// </summary>

        public const string SELECTMEMBER = "[Operation].[usp_MemberSelect]";
        public const string LISTMEMBERS = "[Operation].[usp_MemberList]";
        public const string PAGEVIEWMEMBER = "[Operation].[usp_MemberPageView]";
        public const string RECORDCOUNTMEMBER = "[Operation].[usp_MemberRecordCount]";
        public const string SAVEMEMBER = "[Operation].[usp_MemberSave]";
        public const string SAVEMOBILEOTP = "[Operation].[usp_MobileOTPSave]";
        public const string DELETEMEMBER = "[Operation].[usp_MemberDelete]";
        public const string DELETEMEMBERBYMOBILENO = "[Operation].[usp_MemberDeleteByMobileNo]";
        public const string LISTMEMBERFORCOUNT = "[Operation].[usp_MemberListForCount]";
        public const string CHECKMEMBER = "[Operation].[usp_MemberExist]";
        public const string CHECKOTPSENT = "[Operation].[usp_GetOTPSent]";
        public const string UPDATEPASSWORD = "[Operation].[usp_MemberUpdatePassword]";
        public const string MEMBERLOGIN = "[Operation].[usp_MemberLogIn]";
        public const string MEMBERLOGOUT = "[Operation].[usp_MemberLogOut]";
        public const string MEMBERONLINEPAYMENTSTATUS = "[Operation].[usp_MemberOnlinePaymentStatus]";
        public const string MEMBERDASHBOARD = "[Operation].[usp_VendorDashboard]";
        public const string UPDATEWELCOMEEMAIL = "[Operation].[usp_MemberUpdateWelcomeEmail]";
        // Vendor Customer Details //

        public const string PAGEVIEWCUSTOMERBYMEMBER = "[Operation].[usp_CustomerPageView]";
        public const string PRODUCTSBYVENDOR = "[Operation].[usp_ProductsByVendor]";
        public const string PRODUCTSBYVENDORNEW = "[Operation].[usp_ProductsByVendorNew]";
        public const string PRODUCTSBYCATEGORY = "[Operation].[usp_ProductsByCategory]";
        public const string MEMBERALLDETAILS = "[Operation].[usp_GetMemberAllDetails]";
        public const string PRODUCTSBYCATEGORIES = "[Operation].[usp_CategoriesProductsByVendor]";

        public const string GETMEMBERPASSWORDLIST = "[Operation].[USP_GENERATEPASSWORD_SELECT]";
        public const string GENERATEMEMBERPASSWORD = "[Operation].[USP_GENERATEPASSWORD_HASHCODE]";
        /// <summary>
        /// [Operation].[MemberReturnPolicy]
        /// </summary>

        public const string SELECTMEMBERRETURNPOLICY = "[Operation].[usp_MemberReturnPolicySelect]";
        public const string LISTMEMBERRETURNPOLICY = "[Operation].[usp_MemberReturnPolicyList]";
        public const string SAVEMEMBERRETURNPOLICY = "[Operation].[usp_MemberReturnPolicySave]";
        public const string DELETEMEMBERRETURNPOLICY = "[Operation].[usp_MemberReturnPolicyDelete]";


        /// <summary>
        /// [Operation].[MemberAppSetting]
        /// </summary>

        public const string SELECTMEMBERAPPSETTING = "[Operation].[usp_MemberAppSettingSelect]";
        public const string LISTMEMBERAPPSETTING = "[Operation].[usp_MemberAppSettingList]";
        public const string SAVEMEMBERAPPSETTING = "[Operation].[usp_MemberAppSettingSave]";
        public const string DELETEMEMBERAPPSETTING = "[Operation].[usp_MemberAppSettingDelete]";

        /// <summary>
        /// [Operation].[MemberKYC]
        /// </summary>

        public const string SELECTMEMBERKYC = "[Operation].[usp_MemberKYCSelect]";
        public const string LISTMEMBERKYC = "[Operation].[usp_MemberKYCList]";
        public const string SAVEMEMBERKYC = "[Operation].[usp_MemberKYCSave]";
        public const string DELETEMEMBERKYC = "[Operation].[usp_MemberKYCDelete]";

        /// <summary>
        /// [Operation].[MemberContact]
        /// </summary>

        public const string SELECTMEMBERCONTACT = "[Operation].[usp_MemberContactSelect]";
        public const string LISTMEMBERCONTACT = "[Operation].[usp_MemberContactList]";
        public const string SAVEMEMBERCONTACT = "[Operation].[usp_MemberContactSave]";
        public const string DELETEMEMBERCONTACT = "[Operation].[usp_MemberContactDelete]";


        /// <summary>
        /// [Operation].[Address]
        /// </summary>

        public const string SELECTADDRESS = "[Operation].[usp_AddressSelect]";
        public const string SELECTDEFAULTADDRESS = "[Operation].[usp_GetDefaultAddress]";
        public const string LISTADDRESS = "[Operation].[usp_AddressList]";
        public const string SAVEADDRESS = "[Operation].[usp_AddressSave]";
        public const string DELETEADDRESS = "[Operation].[usp_AddressDelete]";
        public const string LISTALLADDRESS = "[Operation].[usp_AddressListALL]";


        /// <summary>
        /// [Operation].[ProductMaster]
        /// </summary>

        public const string SELECTPRODUCTMASTER = "[Operation].[usp_ProductMasterSelect]";
        public const string SELECTPRODUCTMASTERNEW = "[Operation].[usp_ProductMasterSelectNew]";
        public const string LISTPRODUCTMASTER = "[Operation].[usp_ProductMasterList]";
        public const string LISTPRODUCTMASTERALL = "[Operation].[usp_ProductMasterListAll]";

        public const string SAVEPRODUCTMASTER = "[Operation].[usp_ProductMasterSave]";
        public const string DELETEPRODUCTMASTER = "[Operation].[usp_ProductMasterDelete]";
        public const string LISTALLPRODUCT = "[Operation].[usp_ProductMasterListAll]";
        public const string UPDATEPRODUCTIMAGEPRICE = "[Operation].[usp_ProductMasterUpdatePriceImage]";
        public const string SELECTPRODUCTMASTERCATEGORIES = "[Master].[usp_Categories]";
        public const string CATEGORIESLISTBYID = "[Operation].[usp_CategoriesByID]";
        public const string CATEGORIESNAME = "[Operation].[usp_CategorieName]";
        public const string SAVEPARENTCATEGORIES = "[Operation].[usp_MemberCategorySave]";
        public const string PARENTCATEGORIES = "[Operation].[usp_MemberCategorySelect]";

        public const string FEATUREDCATEGORIES = "[Operation].[usp_FeaturedCategoriesList]";
        public const string SAVEFEATUREDCATEGORIES = "[Operation].[usp_MemberFeatureCategorySave]";
        /// <summary>
        /// [Operation].[ProductImage]
        /// </summary>

        public const string DELETEPRODUCTIMAGEALL = "[Operation].[usp_ProductImageDeleteAll]";
        public const string SELECTPRODUCTIMAGE = "[Operation].[usp_ProductImageSelect]";
        public const string LISTPRODUCTIMAGE = "[Operation].[usp_ProductImageList]";
        public const string LISTPRODUCTIMAGENEW = "[Operation].[usp_ProductImageListNew]";
        public const string SAVEPRODUCTIMAGE = "[Operation].[usp_ProductImageSave]";
        public const string DELETEPRODUCTIMAGE = "[Operation].[usp_ProductImageDelete]";

        /// <summary>
        /// [Operation].[ProductPrice]
        /// </summary>

        public const string SELECTPRODUCTPRICE = "[Operation].[usp_ProductPriceSelect]";
        public const string LISTPRODUCTPRICE = "[Operation].[usp_ProductPriceList]";
        public const string LISTPRODUCTPRICENEW = "[Operation].[usp_ProductPriceListNew]";
        public const string SAVEPRODUCTPRICE = "[Operation].[usp_ProductPriceSave]";
        public const string DELETEPRODUCTPRICE = "[Operation].[usp_ProductPriceDelete]";

        /// <summary>
        /// [Config].[Lookup]
        /// </summary>

        public const string SELECTLOOKUP = "[Config].[usp_LookupSelect]";
        public const string DELETELOOKUP = "[Config].[usp_LookupDelete]";
        public const string SAVELOOKUP = "[Config].[usp_LookupSave]";
        public const string LISTLOOKUPBYCATEGORY = "[Config].[usp_LookupListByCategory]";
        public const string LISTLOOKUPBYMEMBERCATEGORY = "[Config].[usp_LookupListByMemberCategory]";
        public const string LISTLOOKUP = "[Config].[usp_LookupList]";
        public const string LISTSYSTEMCONFIGSETTING = "[Config].[usp_GetSettingbyKey]";
        public const string LISTLOOKUPBYCATEGORYSTARTUP = "[Config].[usp_LookupListByCategoryStartUp]";

        /// <summary>
        /// [Operation].[OrderHeader]
        /// </summary>

        public const string SELECTORDERHEADER = "[Operation].[usp_OrderHeaderSelect]";
        public const string LISTORDERHEADER = "[Operation].[usp_OrderHeaderList]";
        public const string PAGEVIEWORDERHEADER = "[Operation].[usp_OrderHeaderPageView]";
        public const string RECORDCOUNTORDERHEADER = "[Operation].[usp_OrderHeaderRecordCount]";
        public const string ORDERCOUNTBYCUSTOMER = "[Operation].[usp_OrderCountByCustomer]";
        public const string ORDERCOUNTBYVENDOR = "[Operation].[usp_OrderCountByVendor]";

        public const string SAVEORDERHEADER = "[Operation].[usp_OrderHeaderSave]";
        public const string DELETEORDERHEADER = "[Operation].[usp_OrderHeaderDelete]";
        public const string LISTORDERSUMMARYBYSELLER = "[Operation].[usp_OrderSummaryListBySeller]";
        public const string LISTORDERBYCUSTOMER = "[Operation].[usp_CustomerOrderPageView]";
        public const string LISTORDERHEADERBYSELLER = "[Operation].[usp_OrderHeaderListBySeller]";
        public const string SELECTORDERHEADERBYORDERNO = "[Operation].[usp_OrderHeaderSelectByOrderNo]";
        public const string ORDERSTATUSUPDATE = "[Operation].[usp_OrderStatusUpdate]";
        //public const string LISTORDERBYVENDOR = "[Operation].[usp_VendorOrdersList]";
        public const string LISTORDERBYVENDOR = "[Operation].[usp_VendorOrderPageView]";
        public const string UPDATEORDERPICKUP = "[Operation].[usp_UpdateOrderForPickup]";
        public const string GETVENDORDETAILS = "[Operation].[usp_VendorDetails]";

        public const string SELECTORDERHEADERBYORDERID = "[Operation].[usp_OrderHeaderItem]";

        public const string ORDERBYVENDORDETAIL = "[Operation].[usp_VendorOrdersDetail]";

        public const string UPDATEORDERTRACKINGSTATUS = "[Operation].[usp_UpdateOrderStatusShiprocket]";

        public const string DAILYORDERSUMMARY = "[Operation].[usp_DailyOrderSummary]";

        public const string EMAILSERVERALERT = "[Utility].[usp_EmailAlertSetting]";
        //DELIVERY
        public const string UPDATEORDERTRACKINGSTATUSDELHIVERY = "[Operation].[usp_UpdateOrderStatusDelhivery]";
        public const string ORDERDELIVERYHEADER = "[Operation].[usp_OrderHeaderDelivery]";
        public const string ORDERDELIVERYDETAILS = "[Operation].[usp_OrderDetailDelivery]";
        public const string ORDERUPDATEAWB = "[Operation].[usp_OrderUpdateAWB]";
        public const string ORDERDELIVERYTRACKINGDETAILS = "[Operation].[usp_OrderDeliveryTrack]";

        /// <summary>
        /// [Operation].[OrderDetail]
        /// </summary>

        public const string SELECTORDERDETAIL = "[Operation].[usp_OrderDetailSelect]";
        public const string LISTORDERDETAIL = "[Operation].[usp_OrderDetailList]";
        public const string PAGEVIEWORDERDETAIL = "[Operation].[usp_OrderDetailPageView]";
        public const string RECORDCOUNTORDERDETAIL = "[Operation].[usp_OrderDetailRecordCount]";
        public const string SAVEORDERDETAIL = "[Operation].[usp_OrderDetailSave]";
        public const string DELETEORDERDETAIL = "[Operation].[usp_OrderDetailDelete]";

        public const string PRINTINVOICEHEADER = "[Report].[usp_InvoiceHeader]";
        public const string PRINTINVOICEDETAIL = "[Report].[usp_InvoiceDetail]";

        public const string PRINTVENDORRECEIPT = "[Billing].[usp_VendorPaymentReceipt]";

        ///<summary>
        /// [Operation].[OrderPayment]
        /// </summary>
        /// 
        public const string SAVEORDERPAYMENT = "[Operation].[usp_OrderPaymentSave]";


        /// <summary>
        /// [Master].[Theme]
        /// </summary>

        public const string SELECTTHEME = "[Master].[usp_ThemeSelect]";
        public const string LISTTHEME = "[Master].[usp_ThemeList]";
        public const string SAVETHEME = "[Master].[usp_ThemeSave]";
        public const string DELETETHEME = "[Master].[usp_ThemeDelete]";


        /// <summary>
        /// [Billing].[InvoiceHeader]
        /// </summary>
        public const string SELECTINVOICEHEADER = "[Billing].[usp_InvoiceHeaderSelect]";
        public const string LISTINVOICEHEADER = "[Billing].[usp_InvoiceHeaderList]";
        public const string LISTINVOICEHEADERBYSELLER = "[Billing].[usp_InvoiceHeaderListBySeller]";
        public const string LISTINVOICEHEADERBYMEMBER = "[Billing].[usp_InvoiceHeaderListByMember]";
        public const string SAVEINVOICEHEADER = "[Billing].[usp_InvoiceHeaderSave]";
        public const string DELETEINVOICEHEADER = "[Billing].[usp_InvoiceHeaderDelete]";

        /// <summary>
        /// [Billing].[InvoiceDetail]
        /// </summary>

        public const string SELECTINVOICEDETAIL = "[Billing].[usp_InvoiceDetailSelect]";
        public const string LISTINVOICEDETAIL = "[Billing].[usp_InvoiceDetailList]";
        public const string SAVEINVOICEDETAIL = "[Billing].[usp_InvoiceDetailSave]";
        public const string DELETEINVOICEDETAIL = "[Billing].[usp_InvoiceDetailDelete]";


        /// <summary>
        /// [Config].[Lookup]
        /// </summary>

        public const string SELECTMEMBERTHEME = "[Operation].[usp_MemberThemeSelect]";
        public const string DELETEMEMBERTHEME = "[Operation].[usp_MemberThemeDelete]";
        public const string SAVEMEMBERTHEME = "[Operation].[usp_MemberThemeSave]";
        public const string LISTMEMBERTHEME = "[Operation].[usp_MemberThemeList]";


        /// <summary>
        /// [Operation].[DiscountHeader]
        /// </summary>

        public const string DELETEDISCOUNTHEADER = "[Operation].[usp_DiscountHeaderDelete]";
        public const string SELECTDISCOUNTHEADER = "[Operation].[usp_DiscountHeaderSelect]";
        public const string SAVEDISCOUNTHEADER = "[Operation].[usp_DiscountHeaderSave]";
        public const string LISTDISCOUNTHEADER = "[Operation].[usp_DiscountHeaderList]";
        public const string LISTDISCOUNTBYVENDOR = "[Operation].[usp_DiscountHeaderListByVendor]";
        public const string LISTVENDORPRODUCTDISCOUNTS = "[Operation].[usp_ProductDiscountList]";
        public const string LISTDISCOUNTBYPRODUCT = "[Operation].[usp_DiscountListByProduct]";
        



        ///<summary>
        /// [Operation].[DiscountDetail]
        /// </summary>
        /// 
        public const string DELETEDISCOUNTDETAIL = "[Operation].[usp_DiscountDetailDelete]";
        public const string LISTDISCOUNTDETAIL = "[Operation].[usp_DiscountDetailList]";
        public const string SAVEDISCOUNTDETAIL = "[Operation].[usp_DiscountDetailSave]";
        public const string SELECTDISCOUNTDETAIL = "[Operation].[usp_DiscountDetailSelect]";

        ///<summary>
        /// [Operation].[ProductMaster]
        /// </summary>
        /// 
        public const string LISTNEWPRODUCTS = "[Operation].[usp_ProductMasterIsNewStatus]";
        public const string UPDATENEWPRODUCTS = "[Operation].[usp_ProductMasterUpdateIsNew]";

        ///<summary>
        /// [Operation].[MemberBanner]
        /// </summary>
        /// 
        public const string DELETEMEMBERBANNER = "[Operation].[usp_MemberBannerDelete]";
        public const string LISTMEMBERBANNER = "[Operation].[usp_MemberBannerList]";
        public const string LISTMEMBERBANNERBYVENDOR = "[Operation].[usp_MemberBannerListByVendor]";
        public const string SAVEMEMBERBANNER = "[Operation].[usp_MemberBannerSave]";
        public const string SELECTMEMBERBANNER = "[Operation].[usp_MemberBannerSelect]";
        public const string SELECTMEMBERSMSSETTING = "[Operation].[usp_MemberSmsSetting]";
        /// <summary>
        /// [Audit].[EventLog]
        /// </summary>

        public const string DELETEEVENTLOG = "[Audit].[usp_EventLogDelete]";
        public const string LISTEVENTLOG = "[Audit].[usp_EventLogList]";
        public const string LISTEVENTLOGBYCUSTOMER = "[Audit].[usp_EventLogListByCustomer]";
        public const string LISTEVENTLOGBYVENDOR = "[Audit].[usp_EventLogListByVendor]";
        public const string SAVEEVENTLOG = "[Audit].[usp_EventLogSave]";
        public const string SELECTEVENTLOG = "[Audit].[usp_EventLogSelect]";

        
        /// <summary>
        /// Notification History
        /// </summary>

        /// Notification History
        public const string LISTNOTIFICATIONBYVENDOR = "[Utility].[usp_NotificationByVendor]";
        public const string LISTNOTIFICATIONBYCUSTOMER = "[Utility].[usp_NotificationByCustomer]";

        public const string LISTNOTIFICATIONBYVENDORPAGEVIEW = "[Utility].[usp_NotificationByVendorPageView]";
        public const string LISTNOTIFICATIONBYCUSTOMERPAGEVIEW = "[Utility].[usp_NotificationByCustomerPageView]";


        public const string SELECTNOTIFICATION = "[Utility].[usp_NotificationSelect]";
        public const string ISREADNOtifICATION = "[Utility].[usp_NotificationIsRead]";
        public const string SAVENOTIFICATION = "[Utility].[usp_NotificationSave]";
        public const string UNREADNOTIFICATION = "[Utility].[usp_NotificationIsUnReadCount]";
        
        //// Notification Template
        public const string SELECTNOTIFICATIONTEMPLATE = "[Utility].[usp_NotificationTemplateSelect]";
        public const string SELECTSMSNOTIFICATIONTEMPLATE = "[Utility].[usp_SMSNotificationTemplateSelect]";
        //// Email Notification Template
        public const string SELECTEMAILNOTIFICATIONTEMPLATE = "[Utility].[usp_EmailNotificationTemplateSelect]";
        public const string SELECTORDERUPDATEDETAIL = "[Operation].[usp_OrderUpdateDetail]";
        public const string SELECTSMSCONFIGSETTING = "[Utility].[usp_SMSConfigSetting]";
        public const string SELECTEMAILCONFIGSETTING = "[Utility].[usp_EmailConfigSetting]";
        //// Email Notification - Get Details
        public const string GETMEMBERDETAILS = "[Operation].[usp_MemberDetails]";


        ///// Vendor Payment
        public const string SELECTVENDORPAYMENT = "[Billing].[usp_VendorPaymentSelect]";
        public const string LISTALLVENDORPAYMENT = "[Billing].[usp_VendorPaymentList]";
        public const string LISTALLROWSVENDORPAYMENT = "[Billing].[usp_VendorPaymentPageView]";
        public const string SAVEVENDORPAYMENT = "[Billing].[usp_VendorPaymentSave]";
        public const string REMOVEVENDORPAYMENT = "[Billing].[usp_VendorPaymentDelete]";

        public const string VENDORPAYMENTSTATUS = "[Billing].[usp_VendorPaymentStatus]";
        public const string UPDATEVENDORPAYMENTREFERENCENO = "[Billing].[usp_UpdateVendorPaymentReference]";
        public const string LISTALLBYVENDOR = "[Billing].[usp_PaymentListByVendor]";

        ///// Order Discount
        public const string SELECTORDERDISCOUNT = "[Operation].[usp_OrderDiscountSelect]";
        public const string LISTORDERDISCOUNTBYVENDOR = "[Operation].[usp_OrderDiscountList]";
        public const string LISTORDERDISCOUNTBYVENDORPAGEVIEW = "[Operation].[usp_OrderDiscountPageView]";
        public const string ROWCOUNTORDERDISCOUNT = "[Operation].[usp_OrderDiscountRecordCount]";
        public const string SAVEORDERDISCOUNT = "[Operation].[usp_OrderDiscountSave]";
        public const string REMOVEORDERDISCOUNT = "[Operation].[usp_OrderDiscountDelete]";
        public const string GETORDERDISCOUNTSBYVENDOR = "[Operation].[usp_OrderDiscountByVendor]";
        public const string UPDATESMSALERT = "[Utility].[usp_SMSAlertUpdate]";



        /// <summary>
        /// Web Admin User
        /// </summary>

        //////////// WebAdmin 
        public const string DELETEUSER = "[Security].[usp_UserDelete]";
        public const string LISTUSER = "[Security].[usp_UserList]";
        public const string SAVEUSER = "[Security].[usp_UserSave]";
        public const string SELECTUSER = "[Security].[usp_UserSelect]";
        public const string ROWCOUNTUSER = "[Security].[usp_UsersRecordCount]";
        public const string PAGEVIEWLISTUSER = "[Security].[usp_UsersPageView]";
        public const string LOGINUSER = "[Security].[usp_UserLogIn]";
        public const string LOGOUTUSER = "[Security].[usp_UserLogOut]";
        public const string UPDATEPASSWORDUSER = "[Security].[usp_UserUpdatePassword]";
        public const string CHECKDUSER = "[Security].[usp_UserCheck]";

        public const string LISTSELLER = "[Security].[usp_SellerAppList]";



        /// <summary>
        /// Roles
        /// </summary>

        public const string SELECTROLE  = "[Security].[usp_RolesSelect]";
        public const string LISTROLE = "[Security].[usp_RolesList]";
        public const string DELETEROLE = "[Security].[usp_RolesDelete]";
        public const string SAVEROLE = "[Security].[usp_RolesSave]";
        public const string PAGEVIEWLISTROLE = "[Security].[usp_RolesPageView]";
        public const string ROWCOUNTROLE = "[Security].[usp_RolesRecordCount]";





        /// <summary>
        /// RoleRights
        /// </summary>

        public const string LISTROLERIGHT = "[Security].[usp_RoleRightsList]";
        public const string SELECTROLERIGHT = "[Security].[usp_RoleRightsSelect]";
        public const string SAVEROLERIGHT = "[Security].[usp_RoleRightsSave]";
        public const string DELETEROLERIGHT = "[Security].[usp_RoleRightsDelete]";




        
        public const string ROLEACCESSTYPE = "[Config].[usp_LookupListByCategory]";
        public const string GETUSERDETAILS = "[Security].[usp_UserDetails]";

        public const string LISTPRODUCTMASTERNEW = "[Security].[usp_ProductMasterList]";
        public const string LISTORDER = "[Operation].[usp_OrderListSelect]";

        public const string SELECTORDERSBYORDERID = "[Operation].[usp_OrderHeaderItemNew]";


        /// <summary>
        /// Securables
        /// </summary>

        public const string SELECTSECURABLE= "[Security].[usp_SecurablesSelect]";
        public const string LISTSECURABLE = "[Security].[usp_SecurablesList]";
        public const string SAVESECURABLE = "[Security].[usp_SecurablesSave]";
        public const string REMOVESECURABLE = "[Security].[usp_SecurablesDelete]";

        public const string SELECTSECURABLEFUNCTION = "[Security].[usp_SecurableFunctionSelect]";
        public const string LISTSECURABLEFUNCTION = "[Security].[usp_SecurableFunctionList]";
        public const string LISTALLSECURABLEFUNCTION = "[Security].[usp_SecurableFunctionListAll]";
        public const string SAVESECURABLEFUNCTION = "[Security].[usp_SecurableFunctionSave]";
        public const string REMOVESECURABLEFUNCTION = "[Security].[usp_SecurableFunctionDelete]";

        public const string SELLERORDERLIST = "[Operation].[usp_SellerOrderList]";

        public const string SELLETMENTSTATUSUPDATE = "[Operation].[usp_OrderSettlementStatusUpdate]";


        /// <summary>
        /// Web Admin Dashboard
        /// </summary>

        public const string LISTMANAGEMENTDASHBOARDSUMMARY = "[Operation].[usp_ManagementDashboardMetrics]";
        public const string LISTDASHBOARDTOPPRODUCTS = "[Operation].[usp_DashboardTopProducts]";
        public const string LISTDASHBOARDTOPVENDORS = "[Operation].[usp_DashboardTopVendors]";
        public const string LISTDASHBOARDTOPORDERSBYCITY = "[Operation].[usp_DashboardTopOrderByCity]";
        public const string LISTDASHBOARDTOPORDERSCHARGES = "[Operation].[usp_DashboardOrderDeliveryCharges]";
        public const string LISTDASHBOARDTOTALREVENUE = "[Operation].[usp_DashboardTotalRevenue]";
        public const string LISTDASHBOARDMONTHLYSALES= "[Operation].[usp_DashboardMonthlySales]";
        public const string LISTDASHBOARDONBOARDVENDORS = "[Operation].[usp_DashboardOnBoardVendors]";
        public const string LISTDASHBOARDORDERSTATUS = "[Operation].[usp_DashboardOrderStatus]";
        public const string LISTOPERATIONDASHBOARDSUMMARY = "[Operation].[usp_OperationDashboardMetrics]";
        public const string LISTDASHBOARDTOPVENDORSPRODUCTS= "[Operation].[usp_DashboardTopVendorsProducts]";



        /// <summary>
        /// Category Parameter
        /// </summary>
        public const string SELECTCATEGORYPARAMETER = "[Master].[usp_CategoryParameterSelect]";
        public const string LISTCATEGORYPARAMETER= "[Master].[usp_CategoryParameterList]";
        public const string SAVECATEGORYPARAMETER= "[Master].[usp_CategoryParameterSave]";
        public const string REMOVECATEGORYPARAMETER = "[Master].[usp_CategoryParameterDelete]";


        /// <summary>
        /// Parameter Type
        /// </summary>
        public const string SELECTPARAMETERTYPE = "[Master].[usp_ParameterTypeSelect]";
        public const string LISTPARAMETERTYPE = "[Master].[usp_ParameterTypeList]";
        public const string SAVEPARAMETERTYPE = "[Master].[usp_ParameterTypeSave]";
        public const string REMOVEPARAMETERTYPE = "[Master].[usp_ParameterTypeDelete]";

        /// <summary>
        /// Subscription
        /// </summary>
        public const string SELECTSUBSCRIPTION = "[Master].[usp_SubscriptionSelect]";
        public const string LISTSUBSCRIPTION = "[Master].[usp_SubscriptionList]";
        public const string SAVESUBSCRIPTION = "[Master].[usp_SubscriptionSave]";
        public const string REMOVESUBSCRIPTION = "[Master].[usp_SubscriptionDelete]";

        public const string GETAPPLINKSBYUSER = "[Operation].[usp_GetAppLinksByUserID]";
    }
}